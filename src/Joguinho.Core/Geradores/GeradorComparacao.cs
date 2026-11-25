namespace Joguinho.Core.Geradores;

/// <summary>Comparar quantidades: qual grupo tem mais, menos, e quantos a mais.</summary>
public sealed class GeradorComparacao : GeradorBase
{
    public override Topico Topico => Topico.Comparacao;
    protected override int SegmentosNucleo => 4;

    // chave: comparacao|{x}|{y}|{pergunta}
    public override IEnumerable<string> Chaves(int nivel)
    {
        int max = nivel == 2 ? 15 : 10;
        for (int x = 1; x <= max; x++)
        {
            for (int y = 1; y <= max; y++)
            {
                if (x == y) continue;
                yield return $"comparacao|{x}|{y}|mais";
                yield return $"comparacao|{x}|{y}|menos";
                if (x > y) yield return $"comparacao|{x}|{y}|diferenca";
            }
        }
    }

    public override Exercicio Criar(string chave, Random rnd)
    {
        var p = Partes(chave);
        int x = int.Parse(p[1]), y = int.Parse(p[2]);
        var item = ItemAleatorio(rnd);
        var grupoA = Util.Repetir(item.Desenho, x).ToList();
        var grupoB = Util.Repetir(item.Desenho, y).ToList();

        if (p[3] == "diferenca")
        {
            var figuras = grupoA.Append(Figura.Sinal("e")).Concat(grupoB);
            return Novo(chave,
                $"O primeiro grupo tem {x} {item.Forma(x)} e o segundo tem {y}. {item.Quantos} {item.Plural} o primeiro grupo tem a mais?",
                figuras,
                Util.OpcoesNumericas(x - y, 0, x, Opcoes(x > 10 ? 2 : 1), rnd));
        }

        bool mais = p[3] == "mais";
        var opcoes = new List<Opcao> { new("Grupo A", grupoA), new("Grupo B", grupoB) };
        int correto = mais ? (x > y ? 0 : 1) : (x < y ? 0 : 1);
        return Novo(chave,
            $"Qual grupo tem {(mais ? "mais" : "menos")} {item.Plural}?",
            Array.Empty<Figura>(),
            (opcoes, correto));
    }
}
