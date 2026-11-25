namespace Joguinho.Core.Geradores;

public sealed class GeradorAdicao : GeradorBase
{
    public override Topico Topico => Topico.Adicao;

    private static readonly Func<Pessoa, Pessoa, Item, int, int, string>[] Historias =
    {
        (p, q, i, a, b) => $"{p.Nome} tinha {a} {i.Forma(a)}. Ganhou mais {b}. {i.Quantos} {i.Plural} {p.Pronome} tem agora?",
        (p, q, i, a, b) => $"Na caixa havia {a} {i.Forma(a)}. {p.Nome} colocou mais {b}. {i.Quantos} {i.Plural} há na caixa agora?",
        (p, q, i, a, b) => $"{p.Nome} tem {a} {i.Forma(a)} e {q.Nome} tem {b}. {i.Quantos} {i.Plural} {(p.Feminino && q.Feminino ? "elas têm juntas" : "eles têm juntos")}?",
        (p, q, i, a, b) => $"{p.Nome} ganhou {a} {i.Forma(a)} de manhã e {b} à tarde. {i.Quantos} {i.Plural} {p.Pronome} ganhou no dia?",
        (p, q, i, a, b) => $"A professora deu {a} {i.Forma(a)} para {p.Nome} e {b} para {q.Nome}. {i.Quantos} {i.Plural} ela deu ao todo?",
        (p, q, i, a, b) => $"Em cima da mesa havia {a} {i.Forma(a)}. {p.Nome} trouxe mais {b}. {i.Quantos} {i.Plural} ficaram na mesa?",
    };

    // chave: adicao|{a}|{b}|{formato}
    public override IEnumerable<string> Chaves(int nivel)
    {
        int max = Maximo(nivel);
        for (int a = 1; a < max; a++)
        {
            for (int b = 1; a + b <= max; b++)
            {
                yield return $"adicao|{a}|{b}|conta";
                yield return $"adicao|{a}|{b}|falta";
                for (int h = 0; h < Historias.Length; h++) yield return $"adicao|{a}|{b}|historia{h}";
            }
        }
    }

    public override Exercicio Criar(string chave, Random rnd)
    {
        var p = Partes(chave);
        int a = int.Parse(p[1]), b = int.Parse(p[2]), soma = a + b;
        int nivel = soma > 10 ? 2 : 1;
        var item = ItemAleatorio(rnd);
        string formato = p[3];

        if (formato == "falta")
        {
            var figurasFalta = Util.Repetir(item.Desenho, a)
                .Append(Figura.Sinal("+")).Append(Figura.Sinal("?")).Append(Figura.Sinal("="))
                .Concat(Util.Repetir(item.Desenho, soma));
            return Novo(chave, $"Complete: {a} + ? = {soma}", figurasFalta,
                Util.OpcoesNumericas(b, 0, soma + 2, Opcoes(nivel), rnd));
        }

        var figuras = Util.Repetir(item.Desenho, a)
            .Append(Figura.Sinal("+"))
            .Concat(Util.Repetir(item.Desenho, b))
            .Append(Figura.Sinal("="))
            .Append(Figura.Sinal("?"));

        string enunciado;
        if (formato == "conta")
        {
            enunciado = $"Quanto é {a} + {b}?";
        }
        else
        {
            int h = int.Parse(formato.Substring("historia".Length));
            var (p1, p2) = DuasPessoas(rnd);
            enunciado = Historias[h](p1, p2, item, a, b);
        }

        return Novo(chave, enunciado, figuras, Util.OpcoesNumericas(soma, 0, soma + 4, Opcoes(nivel), rnd));
    }
}
