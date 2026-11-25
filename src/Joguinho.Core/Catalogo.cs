namespace Joguinho.Core;

/// <summary>Coisa que aparece desenhada e contada nos exercícios.</summary>
public sealed record Item(string Desenho, string Singular, string Plural, bool Feminino)
{
    public string Forma(int n) => n == 1 ? Singular : Plural;
    public string Quantos => Feminino ? "Quantas" : "Quantos";
    public string QuantosMinusculo => Feminino ? "quantas" : "quantos";
}

public sealed record Pessoa(string Nome, bool Feminino)
{
    public string Pronome => Feminino ? "ela" : "ele";
}

/// <summary>Tema de pesquisa da turma, usado nas tabelas e gráficos.</summary>
public sealed record Tema(string Id, string Titulo, string Coluna, string Substantivo, bool Feminino, IReadOnlyList<string> Opcoes)
{
    public string Artigo => Feminino ? "a" : "o";
    public string Votado(bool mais) => (mais ? "mais " : "menos ") + (Feminino ? "votada" : "votado");
}

public static class Catalogo
{
    public static readonly IReadOnlyList<Item> Itens = new List<Item>
    {
        new("maca", "maçã", "maçãs", true),
        new("estrela", "estrela", "estrelas", true),
        new("balao", "balão", "balões", false),
        new("bola", "bola", "bolas", true),
        new("flor", "flor", "flores", true),
        new("peixe", "peixe", "peixes", false),
        new("carrinho", "carrinho", "carrinhos", false),
        new("coracao", "coração", "corações", false),
        new("lapis", "lápis", "lápis", false),
        new("borboleta", "borboleta", "borboletas", true),
        new("biscoito", "biscoito", "biscoitos", false),
        new("patinho", "patinho", "patinhos", false),
    };

    public static readonly IReadOnlyList<Pessoa> Pessoas = new List<Pessoa>
    {
        new("Ana", true), new("Lia", true), new("Bia", true), new("Gabi", true), new("Malu", true),
        new("Nina", true), new("Duda", true), new("Lara", true), new("Giovana", true), new("Sofia", true),
        new("Téo", false), new("Davi", false), new("Léo", false), new("Caio", false), new("Rafa", false),
        new("Enzo", false), new("Pedro", false), new("Lucas", false), new("Miguel", false), new("Arthur", false),
    };

    public static readonly IReadOnlyList<Tema> Temas = new List<Tema>
    {
        new("brincadeira", "Brincadeira preferida da turma", "Brincadeira", "brincadeira", true,
            new[] { "Pique-esconde", "Amarelinha", "Coelho sai da toca", "Passa-anel", "Pega-pega", "Pular corda", "Batata quente", "Ciranda" }),
        new("fruta", "Fruta preferida da turma", "Fruta", "fruta", true,
            new[] { "Maçã", "Banana", "Uva", "Morango", "Laranja", "Melancia", "Abacaxi", "Manga" }),
        new("animal", "Animal de estimação preferido", "Animal", "animal", false,
            new[] { "Cachorro", "Gato", "Peixe", "Passarinho", "Coelho", "Tartaruga", "Hamster" }),
        new("cor", "Cor preferida da turma", "Cor", "cor", true,
            new[] { "Azul", "Vermelho", "Amarelo", "Verde", "Rosa", "Roxo", "Laranja" }),
        new("esporte", "Esporte preferido da turma", "Esporte", "esporte", false,
            new[] { "Futebol", "Vôlei", "Natação", "Basquete", "Corrida", "Ginástica" }),
        new("brinquedo", "Brinquedo preferido da turma", "Brinquedo", "brinquedo", false,
            new[] { "Bola", "Boneca", "Carrinho", "Pipa", "Peteca", "Quebra-cabeça", "Bambolê" }),
        new("transporte", "Meio de transporte para ir à escola", "Transporte", "meio de transporte", false,
            new[] { "A pé", "Ônibus", "Bicicleta", "Van", "Carro", "Moto" }),
        new("sobremesa", "Sobremesa preferida da turma", "Sobremesa", "sobremesa", true,
            new[] { "Sorvete", "Bolo", "Gelatina", "Pudim", "Brigadeiro", "Picolé" }),
        new("lanche", "Lanche preferido da turma", "Lanche", "lanche", false,
            new[] { "Sanduíche", "Pipoca", "Pão de queijo", "Biscoito", "Fruta", "Bolo de cenoura" }),
        new("instrumento", "Instrumento musical preferido", "Instrumento", "instrumento", false,
            new[] { "Violão", "Tambor", "Flauta", "Pandeiro", "Piano", "Chocalho" }),
    };

    public static Tema TemaPorId(string id) => Temas.First(t => t.Id == id);
}
