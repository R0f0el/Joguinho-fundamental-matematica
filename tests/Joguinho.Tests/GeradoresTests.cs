using Joguinho.Core;

namespace Joguinho.Tests;

public class GeradoresTests
{
    private static readonly BancoDeExercicios Banco = new();

    public static IEnumerable<object[]> TopicosENiveis() =>
        Enum.GetValues<Topico>().SelectMany(t => new[] { new object[] { t, 1 }, new object[] { t, 2 } });

    [Theory]
    [MemberData(nameof(TopicosENiveis))]
    public void Todas_as_chaves_sao_unicas_e_geram_exercicios_validos(Topico topico, int nivel)
    {
        var gerador = Banco.Gerador(topico);
        var chaves = gerador.Chaves(nivel).ToList();

        Assert.NotEmpty(chaves);
        Assert.Equal(chaves.Count, chaves.Distinct().Count());

        var rnd = new Random(42);
        foreach (var chave in chaves)
        {
            var ex = gerador.Criar(chave, rnd);
            Assert.Equal(chave, ex.Chave);
            Assert.Equal(topico, ex.Topico);
            Assert.False(string.IsNullOrWhiteSpace(ex.Enunciado));

            if (topico == Topico.PinteGrafico)
            {
                Assert.Empty(ex.Opcoes);
                Assert.Equal(-1, ex.IndiceCorreto);
                Assert.NotNull(ex.Grafico);
                Assert.All(ex.Grafico!.Barras, b => Assert.InRange(b.Valor, 1, ex.Grafico.Maximo));
                continue;
            }

            Assert.InRange(ex.Opcoes.Count, 2, 5);
            Assert.InRange(ex.IndiceCorreto, 0, ex.Opcoes.Count - 1);
            // Alternativas diferentes entre si (texto ou quantidade de desenhos).
            var assinaturas = ex.Opcoes.Select(o => o.Texto + "#" + (o.Figuras?.Count ?? -1)).ToList();
            Assert.Equal(assinaturas.Count, assinaturas.Distinct().Count());
        }
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    public void Respostas_de_adicao_e_subtracao_estao_certas(int nivel)
    {
        var rnd = new Random(7);
        foreach (var chave in Banco.Gerador(Topico.Adicao).Chaves(nivel))
        {
            var p = chave.Split('|');
            int a = int.Parse(p[1]), b = int.Parse(p[2]);
            var ex = Banco.Gerador(Topico.Adicao).Criar(chave, rnd);
            int esperado = p[3] == "falta" ? b : a + b;
            Assert.Equal(esperado.ToString(), ex.Opcoes[ex.IndiceCorreto].Texto);
        }

        foreach (var chave in Banco.Gerador(Topico.Subtracao).Chaves(nivel))
        {
            var p = chave.Split('|');
            int a = int.Parse(p[1]), b = int.Parse(p[2]);
            var ex = Banco.Gerador(Topico.Subtracao).Criar(chave, rnd);
            int esperado = p[3] == "falta" ? b : a - b;
            Assert.Equal(esperado.ToString(), ex.Opcoes[ex.IndiceCorreto].Texto);
            if (p[3] == "conta") Assert.Equal(b, ex.Figuras.Count(f => f.Riscado));
        }
    }

    [Fact]
    public void Contagem_mostra_a_quantidade_certa_de_desenhos()
    {
        var gerador = Banco.Gerador(Topico.Contagem);
        var rnd = new Random(3);
        foreach (var chave in gerador.Chaves(2))
        {
            int n = int.Parse(chave.Split('|')[1]);
            var ex = gerador.Criar(chave, rnd);
            if (chave.Contains("|quantos|"))
            {
                Assert.Equal(n, ex.Figuras.Count);
                Assert.Equal(n.ToString(), ex.Opcoes[ex.IndiceCorreto].Texto);
            }
            else
            {
                Assert.Equal(n, ex.Opcoes[ex.IndiceCorreto].Figuras!.Count);
            }
        }
    }

    [Fact]
    public void Tabela_e_sempre_a_mesma_para_a_mesma_chave()
    {
        var gerador = Banco.Gerador(Topico.Tabela);
        var chave = gerador.Chaves(1).First();
        var a = gerador.Criar(chave, new Random(1)).Tabela!;
        var b = gerador.Criar(chave, new Random(999)).Tabela!;
        Assert.Equal(a.Linhas.Select(l => (l.Nome, l.Votos)), b.Linhas.Select(l => (l.Nome, l.Votos)));
    }

    [Fact]
    public void Ha_muitos_exercicios_diferentes()
    {
        int nivel1 = Banco.Topicos.Sum(t => Banco.Total(t, 1));
        int nivel2 = Banco.Topicos.Sum(t => Banco.Total(t, 2));
        Assert.True(nivel1 > 1000, $"nível 1 tem só {nivel1}");
        Assert.True(nivel2 > nivel1);
    }
}
