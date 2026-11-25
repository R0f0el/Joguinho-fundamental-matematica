namespace Joguinho.Core.Geradores;

/// <summary>Representação de números: antes, depois, entre e por extenso.</summary>
public sealed class GeradorNumeros : GeradorBase
{
    public override Topico Topico => Topico.Numeros;

    // chave: numeros|{tipo}|{n}
    public override IEnumerable<string> Chaves(int nivel)
    {
        int max = Maximo(nivel);
        for (int n = 0; n <= max; n++)
        {
            if (n < max) yield return $"numeros|depois|{n}";
            if (n > 0) yield return $"numeros|antes|{n}";
            if (n > 0 && n < max) yield return $"numeros|entre|{n}";
            yield return $"numeros|extenso|{n}";
            yield return $"numeros|algarismo|{n}";
        }
    }

    public override Exercicio Criar(string chave, Random rnd)
    {
        var p = Partes(chave);
        int n = int.Parse(p[2]);
        int nivel = n >= 10 ? 2 : 1;
        int q = Opcoes(nivel);

        switch (p[1])
        {
            case "depois":
                return Novo(chave, $"Que número vem depois do {n}?",
                    new[] { Figura.Sinal(n.ToString()), Figura.Sinal("?") },
                    Util.OpcoesNumericas(n + 1, 0, 21, q, rnd));
            case "antes":
                return Novo(chave, $"Que número vem antes do {n}?",
                    new[] { Figura.Sinal("?"), Figura.Sinal(n.ToString()) },
                    Util.OpcoesNumericas(n - 1, 0, 21, q, rnd));
            case "entre":
                return Novo(chave, $"Que número fica entre {n - 1} e {n + 1}?",
                    new[] { Figura.Sinal((n - 1).ToString()), Figura.Sinal("?"), Figura.Sinal((n + 1).ToString()) },
                    Util.OpcoesNumericas(n, 0, 21, q, rnd));
            case "extenso":
                return Novo(chave, $"Qual é o número {Util.Extenso(n)}?",
                    Array.Empty<Figura>(),
                    Util.OpcoesNumericas(n, 0, 20, q, rnd));
            default:
                var valores = Util.ValoresProximos(n, 0, 20, q, rnd);
                var opcoes = valores.Select(v => new Opcao(Util.Extenso(v))).ToList();
                return Novo(chave, $"Como se escreve o número {n} com letras?",
                    new[] { Figura.Sinal(n.ToString()) },
                    (opcoes, valores.IndexOf(n)));
        }
    }
}
