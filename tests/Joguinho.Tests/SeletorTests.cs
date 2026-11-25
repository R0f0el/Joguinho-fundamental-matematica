using Joguinho.Core;

namespace Joguinho.Tests;

public class SeletorTests
{
    private readonly BancoDeExercicios _banco = new();

    [Theory]
    [InlineData(Topico.Livro)]
    [InlineData(Topico.Adicao)]
    [InlineData(Topico.Numeros)]
    [InlineData(Topico.PinteGrafico)]
    public void Nao_repete_ate_acabarem_os_exercicios(Topico topico)
    {
        int total = _banco.Total(topico, 1);
        var vistos = new HashSet<string>();
        int semente = 1;

        while (vistos.Count < total)
        {
            var rodada = _banco.Sortear(new PedidoRodada(new[] { topico }, 10, 1, vistos.ToList(), semente++));
            Assert.False(rodada.Recomecou);
            int novosNaRodada = rodada.Exercicios.Count(e => !vistos.Contains(e.Chave));
            int esperado = Math.Min(10, total - vistos.Count);
            // Enquanto há novos suficientes, a rodada inteira é de exercícios novos.
            Assert.True(novosNaRodada >= Math.Min(esperado, rodada.Exercicios.Count),
                $"rodada trouxe repetidos com {total - vistos.Count} novos disponíveis");
            foreach (var e in rodada.Exercicios) vistos.Add(e.Chave);
            Assert.True(semente < 2000, "não terminou");
        }

        var depois = _banco.Sortear(new PedidoRodada(new[] { topico }, 10, 1, vistos.ToList(), semente));
        Assert.True(depois.Recomecou);
        Assert.NotEmpty(depois.Exercicios);
    }

    [Fact]
    public void Rodada_nao_tem_dois_exercicios_com_a_mesma_conta()
    {
        for (int s = 0; s < 50; s++)
        {
            var rodada = _banco.Sortear(new PedidoRodada(new[] { Topico.Adicao, Topico.Subtracao }, 20, 1, null, s));
            var nucleos = rodada.Exercicios.Select(e => _banco.Gerador(e.Topico).Nucleo(e.Chave)).ToList();
            Assert.Equal(nucleos.Count, nucleos.Distinct().Count());
            Assert.Equal(20, rodada.Exercicios.Count);
        }
    }

    [Fact]
    public void Prefere_contas_ineditas()
    {
        // Depois de ver "3 + 4" em forma de conta, a próxima rodada não deve trazer 3 + 4 de novo
        // (em outro formato) enquanto houver contas inéditas.
        var vistos = new List<string> { "adicao|3|4|conta" };
        for (int s = 0; s < 30; s++)
        {
            var rodada = _banco.Sortear(new PedidoRodada(new[] { Topico.Adicao }, 10, 1, vistos, s));
            Assert.DoesNotContain(rodada.Exercicios, e => e.Chave.StartsWith("adicao|3|4|"));
        }
    }

    [Fact]
    public void Mistura_os_assuntos_pedidos()
    {
        var topicos = new[] { Topico.Contagem, Topico.Tabela, Topico.Grafico };
        var rodada = _banco.Sortear(new PedidoRodada(topicos, 9, 2, null, 5));
        Assert.Equal(9, rodada.Exercicios.Count);
        foreach (var t in topicos) Assert.Equal(3, rodada.Exercicios.Count(e => e.Topico == t));
    }

    [Fact]
    public void Sem_topicos_usa_todos()
    {
        var rodada = _banco.Sortear(new PedidoRodada(null, 30, 1, null, 2));
        Assert.Equal(30, rodada.Exercicios.Count);
        Assert.True(rodada.Exercicios.Select(e => e.Topico).Distinct().Count() >= 8);
    }
}
