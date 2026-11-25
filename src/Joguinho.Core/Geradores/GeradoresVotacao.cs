namespace Joguinho.Core.Geradores;

/// <summary>Leitura de tabelas com número de votos e bolinhas.</summary>
public sealed class GeradorTabela : GeradorBase
{
    public const int Variantes = 6;
    public override Topico Topico => Topico.Tabela;
    protected override int SegmentosNucleo => 4;

    internal static int Itens(int nivel) => nivel == 2 ? 5 : 4;
    internal static int MaxVotos(int nivel) => nivel == 2 ? 12 : 9;

    // chave: tabela|{tema}|{variante}|{nivel}|{pergunta}
    public override IEnumerable<string> Chaves(int nivel)
    {
        foreach (var tema in Catalogo.Temas)
            for (int v = 0; v < Variantes; v++)
                foreach (var tipo in PerguntasVotacao.Tipos(Itens(nivel), tabela: true))
                    yield return $"tabela|{tema.Id}|{v}|{nivel}|{tipo}";
    }

    public override Exercicio Criar(string chave, Random rnd)
    {
        var p = Partes(chave);
        var tema = Catalogo.TemaPorId(p[1]);
        int nivel = int.Parse(p[3]);
        var dados = Votacao.Gerar(Nucleo(chave), tema, Itens(nivel), MaxVotos(nivel));
        var r = PerguntasVotacao.Criar(p[4], tema, dados, nivel, rnd);
        return new Exercicio(chave, Topico, r.Enunciado, Array.Empty<Figura>(), r.Opcoes, r.Correto,
            Tabela: PerguntasVotacao.MontarTabela(tema, dados, r));
    }
}

/// <summary>Leitura de gráficos de barras.</summary>
public sealed class GeradorGrafico : GeradorBase
{
    public const int Variantes = 6;
    public override Topico Topico => Topico.Grafico;
    protected override int SegmentosNucleo => 4;

    // chave: grafico|{tema}|{variante}|{nivel}|{pergunta}
    public override IEnumerable<string> Chaves(int nivel)
    {
        foreach (var tema in Catalogo.Temas)
            for (int v = 0; v < Variantes; v++)
                foreach (var tipo in PerguntasVotacao.Tipos(GeradorTabela.Itens(nivel), tabela: false))
                    yield return $"grafico|{tema.Id}|{v}|{nivel}|{tipo}";
    }

    public override Exercicio Criar(string chave, Random rnd)
    {
        var p = Partes(chave);
        var tema = Catalogo.TemaPorId(p[1]);
        int nivel = int.Parse(p[3]);
        int max = GeradorTabela.MaxVotos(nivel);
        var dados = Votacao.Gerar(Nucleo(chave), tema, GeradorTabela.Itens(nivel), max);
        var r = PerguntasVotacao.Criar(p[4], tema, dados, nivel, rnd);
        var grafico = new Grafico(tema.Titulo, "Turma do 1º ano", max, dados.Select(d => new Barra(d.Nome, d.Votos)).ToList());
        return new Exercicio(chave, Topico, r.Enunciado, Array.Empty<Figura>(), r.Opcoes, r.Correto, Grafico: grafico);
    }
}

/// <summary>Construir o gráfico: pintar um quadradinho para cada voto da tabela.</summary>
public sealed class GeradorPinteGrafico : GeradorBase
{
    public const int Variantes = 10;
    public override Topico Topico => Topico.PinteGrafico;
    protected override int SegmentosNucleo => 4;

    // chave: pinte|{tema}|{variante}|{nivel}
    public override IEnumerable<string> Chaves(int nivel)
    {
        foreach (var tema in Catalogo.Temas)
            for (int v = 0; v < Variantes; v++)
                yield return $"pinte|{tema.Id}|{v}|{nivel}";
    }

    public override Exercicio Criar(string chave, Random rnd)
    {
        var p = Partes(chave);
        var tema = Catalogo.TemaPorId(p[1]);
        int nivel = int.Parse(p[3]);
        int max = nivel == 2 ? 8 : 6;
        var dados = Votacao.Gerar(chave, tema, nivel == 2 ? 5 : 4, max);
        var tabela = new Tabela(tema.Titulo, tema.Coluna,
            dados.Select(d => new LinhaTabela(d.Nome, d.Votos, Votacao.Lugar(dados, d) + "º", EsconderClassificacao: true)).ToList());
        var grafico = new Grafico(tema.Titulo, "Turma do 1º ano", max, dados.Select(d => new Barra(d.Nome, d.Votos)).ToList());
        return new Exercicio(chave, Topico,
            "Construa o gráfico: pinte um quadradinho para cada voto da tabela.",
            Array.Empty<Figura>(), Array.Empty<Opcao>(), -1, tabela, grafico);
    }
}
