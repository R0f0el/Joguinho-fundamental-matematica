namespace Joguinho.Core.Geradores;

public sealed record Voto(string Nome, int Votos);

/// <summary>Dados de uma pesquisa da turma, sempre os mesmos para a mesma semente.</summary>
public static class Votacao
{
    public static List<Voto> Gerar(string semente, Tema tema, int itens, int maxVotos)
    {
        var rnd = new Random(Util.Semente(semente));
        var nomes = Util.Embaralhar(tema.Opcoes, rnd).Take(itens).ToList();
        var votos = Util.Embaralhar(Enumerable.Range(1, maxVotos), rnd).Take(itens).ToList();
        return nomes.Select((n, i) => new Voto(n, votos[i])).ToList();
    }

    public static int Lugar(IReadOnlyList<Voto> dados, Voto v) => dados.Count(d => d.Votos > v.Votos) + 1;
}

/// <summary>Perguntas de leitura de tabela e de gráfico.</summary>
public static class PerguntasVotacao
{
    public static IEnumerable<string> Tipos(int itens, bool tabela)
    {
        yield return "mais";
        yield return "menos";
        yield return "diferenca";
        yield return "juntos";
        yield return "maisque";
        for (int i = 0; i < itens; i++) yield return $"quantos{i}";
        if (!tabela) yield break;
        for (int i = 0; i < itens; i++) yield return $"lugar{i}";
        yield return "conte";
    }

    public sealed record Resultado(
        string Enunciado,
        List<Opcao> Opcoes,
        int Correto,
        int EsconderNumero = -1,
        int EsconderLugar = -1,
        bool EsconderTodosLugares = false);

    private static string Aspas(string nome) => $"“{nome}”";

    public static Resultado Criar(string tipo, Tema tema, List<Voto> dados, int nivel, Random rnd)
    {
        int q = nivel == 2 ? 4 : 3;
        var ordenado = dados.OrderByDescending(v => v.Votos).ToList();
        var nomes = dados.Select(v => v.Nome).ToList();
        int maxVotos = ordenado[0].Votos;

        Resultado Num(string enunciado, int resposta, int esconderNumero = -1, int esconderLugar = -1)
        {
            var (o, c) = Util.OpcoesNumericas(resposta, 0, Math.Max(resposta + 4, maxVotos), q, rnd);
            return new Resultado(enunciado, o, c, esconderNumero, esconderLugar);
        }

        Resultado Txt(string enunciado, string correta, IEnumerable<string> todas, int esconderLugar = -1, bool esconderTodos = false)
        {
            var (o, c) = Util.OpcoesTexto(correta, todas, Math.Min(q, todas.Count()), rnd);
            return new Resultado(enunciado, o, c, -1, esconderLugar, esconderTodos);
        }

        if (tipo == "mais")
            return Txt($"Qual foi {tema.Artigo} {tema.Substantivo} {tema.Votado(true)}?", ordenado[0].Nome, nomes, esconderTodos: true);
        if (tipo == "menos")
            return Txt($"Qual foi {tema.Artigo} {tema.Substantivo} {tema.Votado(false)}?", ordenado[^1].Nome, nomes, esconderTodos: true);
        if (tipo == "diferenca")
            return Num($"Qual é a diferença de votos entre {Aspas(ordenado[0].Nome)} (1º lugar) e {Aspas(ordenado[^1].Nome)} (último lugar)?",
                ordenado[0].Votos - ordenado[^1].Votos);
        if (tipo == "juntos")
        {
            var par = Util.Embaralhar(dados, rnd).Take(2).ToList();
            return Num($"Quantos votos {Aspas(par[0].Nome)} e {Aspas(par[1].Nome)} tiveram juntos?", par[0].Votos + par[1].Votos);
        }
        if (tipo == "maisque")
        {
            var par = Util.Embaralhar(dados, rnd).Take(2).OrderByDescending(v => v.Votos).ToList();
            return Num($"Quantos votos {Aspas(par[0].Nome)} teve a mais que {Aspas(par[1].Nome)}?", par[0].Votos - par[1].Votos);
        }
        if (tipo == "conte")
        {
            int i = rnd.Next(dados.Count);
            return Num($"Conte as bolinhas: quantos votos {Aspas(dados[i].Nome)} teve?", dados[i].Votos, esconderNumero: i);
        }
        if (tipo.StartsWith("quantos"))
        {
            int i = int.Parse(tipo.Substring("quantos".Length));
            return Num($"Quantos votos {Aspas(dados[i].Nome)} recebeu?", dados[i].Votos);
        }
        if (tipo.StartsWith("lugar"))
        {
            int i = int.Parse(tipo.Substring("lugar".Length));
            var lugares = Enumerable.Range(1, dados.Count).Select(n => n + "º").ToList();
            return Txt($"Em que lugar ficou {Aspas(dados[i].Nome)}?", Votacao.Lugar(dados, dados[i]) + "º", lugares, esconderLugar: i);
        }
        throw new ArgumentException($"Pergunta desconhecida: {tipo}");
    }

    public static Tabela MontarTabela(Tema tema, List<Voto> dados, Resultado r) =>
        new(tema.Titulo, tema.Coluna, dados.Select((v, i) => new LinhaTabela(
            v.Nome,
            v.Votos,
            Votacao.Lugar(dados, v) + "º",
            EsconderNumero: r.EsconderNumero == i,
            EsconderClassificacao: r.EsconderTodosLugares || r.EsconderLugar == i)).ToList());
}
