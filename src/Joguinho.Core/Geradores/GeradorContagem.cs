namespace Joguinho.Core.Geradores;

/// <summary>Quantificação: contar desenhos e achar o grupo com a quantidade pedida.</summary>
public sealed class GeradorContagem : GeradorBase
{
    public override Topico Topico => Topico.Contagem;

    // chave: contagem|{n}|{formato}|{desenho}
    public override IEnumerable<string> Chaves(int nivel)
    {
        int max = Maximo(nivel);
        foreach (var item in Catalogo.Itens)
        {
            for (int n = 1; n <= max; n++)
            {
                yield return $"contagem|{n}|quantos|{item.Desenho}";
                if (n >= 2) yield return $"contagem|{n}|grupo|{item.Desenho}";
            }
        }
    }

    public override Exercicio Criar(string chave, Random rnd)
    {
        var p = Partes(chave);
        int n = int.Parse(p[1]);
        var item = Catalogo.Itens.First(i => i.Desenho == p[3]);
        int nivel = n > 10 ? 2 : 1;

        if (p[2] == "quantos")
        {
            return Novo(chave,
                $"{item.Quantos} {item.Plural} você vê?",
                Util.Repetir(item.Desenho, n),
                Util.OpcoesNumericas(n, 0, n + 4, Opcoes(nivel), rnd));
        }

        var valores = Util.ValoresProximos(n, 1, n + 4, 3, rnd);
        var opcoes = valores
            .Select((v, i) => new Opcao($"Grupo {(char)('A' + i)}", Util.Repetir(item.Desenho, v).ToList()))
            .ToList();
        return Novo(chave,
            $"Qual grupo tem {n} {item.Forma(n)}?",
            Array.Empty<Figura>(),
            (opcoes, valores.IndexOf(n)));
    }
}
