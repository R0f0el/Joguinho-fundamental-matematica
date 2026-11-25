namespace Joguinho.Core.Geradores;

/// <summary>Exercícios das páginas 23 e 24 do livro (tabela das brincadeiras e gráfico dos transportes).</summary>
public sealed class GeradorLivro : GeradorBase
{
    public override Topico Topico => Topico.Livro;
    protected override int SegmentosNucleo => 2;

    private static readonly Tabela TabelaBrincadeiras = new(
        "Brincadeiras mais votadas pelos alunos da turma da Giovana",
        "Brincadeira",
        new List<LinhaTabela>
        {
            new("Pique-esconde", 8, "1º"),
            new("Amarelinha", 4, "3º"),
            new("Coelho sai da toca", 6, "2º"),
            new("Passa-anel", 2, "4º"),
        });

    // Números de exemplo: no livro eles vêm da pesquisa feita pela própria turma.
    private static readonly Grafico GraficoTransportes = new(
        "Meio de transporte utilizado pelos colegas da turma para ir à escola",
        "1º ano A",
        8,
        new List<Barra> { new("A pé", 3), new("Ônibus", 5), new("Bicicleta", 2), new("Van", 4), new("Carro", 8) });

    private sealed record Def(string Id, bool UsaTabela, string Enunciado, string Correta, string[] Outras);

    private static readonly Def[] Defs =
    {
        new("t1a", true, "Qual foi a brincadeira mais votada?", "Pique-esconde", new[] { "Amarelinha", "Coelho sai da toca", "Passa-anel" }),
        new("t1b", true, "Qual foi a brincadeira menos votada?", "Passa-anel", new[] { "Amarelinha", "Coelho sai da toca", "Pique-esconde" }),
        new("t1c", true, "Qual é a diferença de votos entre a 1ª e a 4ª brincadeira mais votada?", "6", new[] { "4", "8", "10" }),
        new("t1d", true, "Quantos votos a Amarelinha recebeu?", "4", new[] { "2", "3", "6" }),
        new("t1e", true, "Quantos votos o Coelho sai da toca teve a mais que a Amarelinha?", "2", new[] { "1", "3", "4" }),
        new("t1f", true, "Quantos alunos votaram ao todo?", "20", new[] { "18", "19", "22" }),
        new("g1", false, "Qual é o meio de locomoção mais utilizado pelos alunos no trajeto de casa para a escola?", "Carro", new[] { "Ônibus", "Van", "A pé" }),
        new("g2", false, "Qual é o meio de transporte menos utilizado?", "Bicicleta", new[] { "A pé", "Van", "Ônibus" }),
        new("g3", false, "Quantos colegas vão de ônibus?", "5", new[] { "3", "4", "6" }),
        new("g4", false, "Quantos colegas vão de carro a mais do que de van?", "4", new[] { "2", "3", "6" }),
        new("g5", false, "Quantos colegas vão a pé ou de bicicleta, juntando os dois?", "5", new[] { "4", "6", "7" }),
    };

    public override IEnumerable<string> Chaves(int nivel) => Defs.Select(d => $"livro|{d.Id}");

    public override Exercicio Criar(string chave, Random rnd)
    {
        var id = Partes(chave)[1];
        var d = Defs.First(x => x.Id == id);
        var (opcoes, correto) = Util.OpcoesTexto(d.Correta, d.Outras, 3, rnd);
        return new Exercicio(chave, Topico, d.Enunciado, Array.Empty<Figura>(), opcoes, correto,
            d.UsaTabela ? TabelaBrincadeiras : null,
            d.UsaTabela ? null : GraficoTransportes);
    }
}
