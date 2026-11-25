namespace Joguinho.Core.Geradores;

public interface IGerador
{
    Topico Topico { get; }

    /// <summary>Todas as chaves possíveis deste tópico no nível (1 = até 10, 2 = até 20).</summary>
    IEnumerable<string> Chaves(int nivel);

    /// <summary>
    /// "Núcleo" da chave: a parte matemática do exercício (ex.: "adicao|3|4").
    /// Exercícios com o mesmo núcleo são parecidos; o seletor prefere núcleos inéditos.
    /// </summary>
    string Nucleo(string chave);

    Exercicio Criar(string chave, Random rnd);
}

public abstract class GeradorBase : IGerador
{
    public abstract Topico Topico { get; }
    protected virtual int SegmentosNucleo => 3;

    public abstract IEnumerable<string> Chaves(int nivel);
    public abstract Exercicio Criar(string chave, Random rnd);

    public string Nucleo(string chave) => string.Join("|", chave.Split('|').Take(SegmentosNucleo));

    protected static int Maximo(int nivel) => nivel == 2 ? 20 : 10;

    protected static int Opcoes(int nivel) => nivel == 2 ? 4 : 3;

    protected static Item ItemAleatorio(Random rnd) => Catalogo.Itens[rnd.Next(Catalogo.Itens.Count)];

    protected static (Pessoa A, Pessoa B) DuasPessoas(Random rnd)
    {
        var lista = Util.Embaralhar(Catalogo.Pessoas, rnd);
        return (lista[0], lista[1]);
    }

    protected Exercicio Novo(
        string chave,
        string enunciado,
        IEnumerable<Figura> figuras,
        (List<Opcao> Opcoes, int Correto) opcoes,
        Tabela? tabela = null,
        Grafico? grafico = null) =>
        new(chave, Topico, enunciado, figuras.ToList(), opcoes.Opcoes, opcoes.Correto, tabela, grafico);

    protected static string[] Partes(string chave) => chave.Split('|');
}
