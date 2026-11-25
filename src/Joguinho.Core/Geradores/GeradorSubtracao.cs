namespace Joguinho.Core.Geradores;

public sealed class GeradorSubtracao : GeradorBase
{
    public override Topico Topico => Topico.Subtracao;

    private static readonly Func<Pessoa, Pessoa, Item, int, int, string>[] Historias =
    {
        (p, q, i, a, b) => $"{p.Nome} tinha {a} {i.Forma(a)}. Deu {b} para {q.Nome}. {i.Quantos} {i.Plural} sobraram?",
        (p, q, i, a, b) => $"Havia {a} {i.Forma(a)} na caixa. {p.Nome} tirou {b}. {i.Quantos} {i.Plural} ficaram na caixa?",
        (p, q, i, a, b) => $"{p.Nome} tinha {a} {i.Forma(a)} e perdeu {b}. Com {i.QuantosMinusculo} {i.Plural} {p.Pronome} ficou?",
        (p, q, i, a, b) => $"Na mesa havia {a} {i.Forma(a)}. {p.Nome} guardou {b}. {i.Quantos} {i.Plural} ainda estão na mesa?",
        // Comparação: só quando b < a.
        (p, q, i, a, b) => $"{p.Nome} tem {a} {i.Forma(a)} e {q.Nome} tem {b}. {i.Quantos} {i.Plural} {p.Nome} tem a mais que {q.Nome}?",
    };

    private const int HistoriaComparacao = 4;

    // chave: subtracao|{a}|{b}|{formato}
    public override IEnumerable<string> Chaves(int nivel)
    {
        int max = Maximo(nivel);
        for (int a = 1; a <= max; a++)
        {
            for (int b = 1; b <= a; b++)
            {
                yield return $"subtracao|{a}|{b}|conta";
                yield return $"subtracao|{a}|{b}|falta";
                for (int h = 0; h < Historias.Length; h++)
                {
                    if (h == HistoriaComparacao && b == a) continue;
                    yield return $"subtracao|{a}|{b}|historia{h}";
                }
            }
        }
    }

    public override Exercicio Criar(string chave, Random rnd)
    {
        var p = Partes(chave);
        int a = int.Parse(p[1]), b = int.Parse(p[2]), resto = a - b;
        int nivel = a > 10 ? 2 : 1;
        var item = ItemAleatorio(rnd);
        string formato = p[3];
        var riscados = Util.Repetir(item.Desenho, a, b);

        if (formato == "conta")
        {
            return Novo(chave, $"Quanto é {a} − {b}?", riscados,
                Util.OpcoesNumericas(resto, 0, a, Opcoes(nivel), rnd));
        }

        if (formato == "falta")
        {
            return Novo(chave, $"Complete: {a} − ? = {resto}", riscados,
                Util.OpcoesNumericas(b, 0, a + 2, Opcoes(nivel), rnd));
        }

        int h = int.Parse(formato.Substring("historia".Length));
        var (p1, p2) = DuasPessoas(rnd);
        var figuras = h == HistoriaComparacao
            ? Util.Repetir(item.Desenho, a).Append(Figura.Sinal("e")).Concat(Util.Repetir(item.Desenho, b))
            : riscados;
        return Novo(chave, Historias[h](p1, p2, item, a, b), figuras,
            Util.OpcoesNumericas(resto, 0, a, Opcoes(nivel), rnd));
    }
}
