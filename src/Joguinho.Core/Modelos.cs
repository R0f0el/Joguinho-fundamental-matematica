using System.Text.Json.Serialization;

namespace Joguinho.Core;

[JsonConverter(typeof(JsonStringEnumConverter<Topico>))]
public enum Topico
{
    Livro,
    Contagem,
    Adicao,
    Subtracao,
    Comparacao,
    Numeros,
    Tabela,
    Grafico,
    PinteGrafico
}

/// <summary>Um desenho (maçã, estrela...) ou um sinal (+, −, =, ?) mostrado no exercício.</summary>
public sealed record Figura(string? Desenho, string? Simbolo = null, bool Riscado = false)
{
    public static Figura De(string desenho, bool riscado = false) => new(desenho, null, riscado);
    public static Figura Sinal(string simbolo) => new(null, simbolo);
}

/// <summary>Uma alternativa. Pode ter só texto ou um grupo de desenhos.</summary>
public sealed record Opcao(string Texto, IReadOnlyList<Figura>? Figuras = null);

public sealed record LinhaTabela(
    string Nome,
    int Votos,
    string Classificacao,
    bool EsconderNumero = false,
    bool EsconderClassificacao = false);

public sealed record Tabela(string Titulo, string Coluna, IReadOnlyList<LinhaTabela> Linhas);

public sealed record Barra(string Nome, int Valor);

public sealed record Grafico(string Titulo, string Fonte, int Maximo, IReadOnlyList<Barra> Barras);

/// <summary>
/// Um exercício pronto para o jogo. <see cref="Chave"/> identifica o exercício de forma única
/// (usada para evitar repetição). Em "Pinte o gráfico" não há alternativas: a resposta são
/// os valores das barras e <see cref="IndiceCorreto"/> vale -1.
/// </summary>
public sealed record Exercicio(
    string Chave,
    Topico Topico,
    string Enunciado,
    IReadOnlyList<Figura> Figuras,
    IReadOnlyList<Opcao> Opcoes,
    int IndiceCorreto,
    Tabela? Tabela = null,
    Grafico? Grafico = null);

public sealed record PedidoRodada(
    IReadOnlyList<Topico>? Topicos = null,
    int Quantidade = 10,
    int Nivel = 1,
    IReadOnlyList<string>? Vistos = null,
    int? Semente = null);

/// <param name="Disponiveis">Total de exercícios dos tópicos pedidos, no nível pedido.</param>
/// <param name="Novos">Quantos desses a criança ainda não viu.</param>
/// <param name="Recomecou">Verdadeiro se algum tópico já tinha sido visto por inteiro e recomeçou.</param>
public sealed record Rodada(IReadOnlyList<Exercicio> Exercicios, int Disponiveis, int Novos, bool Recomecou);
