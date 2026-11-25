namespace Joguinho.Core;

public static class Util
{
    /// <summary>Semente estável a partir de um texto (string.GetHashCode muda a cada execução).</summary>
    public static int Semente(string texto)
    {
        unchecked
        {
            uint h = 2166136261;
            foreach (char c in texto)
            {
                h ^= c;
                h *= 16777619;
            }
            return (int)(h & 0x7FFFFFFF);
        }
    }

    public static List<T> Embaralhar<T>(IEnumerable<T> itens, Random rnd)
    {
        var lista = itens.ToList();
        for (int i = lista.Count - 1; i > 0; i--)
        {
            int j = rnd.Next(i + 1);
            (lista[i], lista[j]) = (lista[j], lista[i]);
        }
        return lista;
    }

    /// <summary>Resposta + vizinhos próximos (distratores), já embaralhados.</summary>
    public static List<int> ValoresProximos(int resposta, int minimo, int maximo, int quantidade, Random rnd)
    {
        var valores = new List<int> { resposta };
        var deltas = Embaralhar(new[] { -1, 1, -2, 2, -3, 3, -4, 4 }, rnd).OrderBy(d => Math.Abs(d));
        foreach (int d in deltas)
        {
            if (valores.Count >= quantidade) break;
            int v = resposta + d;
            if (v >= minimo && v <= maximo && !valores.Contains(v)) valores.Add(v);
        }
        int extra = 5;
        while (valores.Count < quantidade)
        {
            int v = resposta + extra++;
            if (!valores.Contains(v)) valores.Add(v);
        }
        return Embaralhar(valores, rnd);
    }

    public static (List<Opcao> Opcoes, int Correto) OpcoesNumericas(int resposta, int minimo, int maximo, int quantidade, Random rnd)
    {
        var valores = ValoresProximos(resposta, minimo, maximo, quantidade, rnd);
        return (valores.Select(v => new Opcao(v.ToString())).ToList(), valores.IndexOf(resposta));
    }

    public static (List<Opcao> Opcoes, int Correto) OpcoesTexto(string correta, IEnumerable<string> erradas, int quantidade, Random rnd)
    {
        var valores = new List<string> { correta };
        valores.AddRange(Embaralhar(erradas.Where(e => e != correta).Distinct(), rnd).Take(quantidade - 1));
        var embaralhados = Embaralhar(valores, rnd);
        return (embaralhados.Select(t => new Opcao(t)).ToList(), embaralhados.IndexOf(correta));
    }

    public static IEnumerable<Figura> Repetir(string desenho, int n, int riscadosNoFim = 0) =>
        Enumerable.Range(0, n).Select(i => Figura.De(desenho, i >= n - riscadosNoFim));

    private static readonly string[] NumerosPorExtenso =
    {
        "zero", "um", "dois", "três", "quatro", "cinco", "seis", "sete", "oito", "nove", "dez",
        "onze", "doze", "treze", "catorze", "quinze", "dezesseis", "dezessete", "dezoito", "dezenove", "vinte"
    };

    public static string Extenso(int n) =>
        n >= 0 && n < NumerosPorExtenso.Length ? NumerosPorExtenso[n] : n.ToString();
}
