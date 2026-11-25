using Joguinho.Core.Geradores;

namespace Joguinho.Core;

/// <summary>
/// Sorteia rodadas de exercícios evitando repetição.
/// Ordem de preferência: (1) exercício nunca visto com núcleo matemático inédito,
/// (2) exercício nunca visto, (3) exercício já visto (só quando acabaram os novos).
/// Na mesma rodada nunca aparecem dois exercícios com o mesmo núcleo.
/// </summary>
public sealed class BancoDeExercicios
{
    public const int MaximoPorRodada = 30;

    private readonly Dictionary<Topico, IGerador> _geradores;

    public BancoDeExercicios() : this(new IGerador[]
    {
        new GeradorLivro(),
        new GeradorContagem(),
        new GeradorAdicao(),
        new GeradorSubtracao(),
        new GeradorComparacao(),
        new GeradorNumeros(),
        new GeradorTabela(),
        new GeradorGrafico(),
        new GeradorPinteGrafico(),
    })
    {
    }

    public BancoDeExercicios(IEnumerable<IGerador> geradores)
    {
        _geradores = geradores.ToDictionary(g => g.Topico);
    }

    public IReadOnlyCollection<Topico> Topicos => _geradores.Keys;

    public IGerador Gerador(Topico topico) => _geradores[topico];

    public int Total(Topico topico, int nivel) => _geradores[topico].Chaves(NivelValido(nivel)).Count();

    private static int NivelValido(int nivel) => nivel == 2 ? 2 : 1;

    public Rodada Sortear(PedidoRodada pedido)
    {
        int nivel = NivelValido(pedido.Nivel);
        int quantidade = Math.Clamp(pedido.Quantidade, 1, MaximoPorRodada);
        var rnd = pedido.Semente is int s ? new Random(s) : new Random();
        var vistos = new HashSet<string>(pedido.Vistos ?? Array.Empty<string>());

        var topicos = (pedido.Topicos ?? Array.Empty<Topico>()).Where(_geradores.ContainsKey).Distinct().ToList();
        if (topicos.Count == 0) topicos = _geradores.Keys.ToList();

        int disponiveis = 0, novos = 0;
        bool recomecou = false;
        var filas = new List<Queue<string>>();

        foreach (var topico in topicos)
        {
            var g = _geradores[topico];
            var chaves = g.Chaves(nivel).ToList();
            var ineditas = chaves.Where(c => !vistos.Contains(c)).ToList();
            disponiveis += chaves.Count;
            novos += ineditas.Count;

            List<string> ordem;
            if (ineditas.Count == 0)
            {
                recomecou = true;
                ordem = Util.Embaralhar(chaves, rnd);
            }
            else
            {
                var nucleosVistos = new HashSet<string>(chaves.Where(vistos.Contains).Select(g.Nucleo));
                var embaralhadas = Util.Embaralhar(ineditas, rnd);
                ordem = embaralhadas.Where(c => !nucleosVistos.Contains(g.Nucleo(c)))
                    .Concat(embaralhadas.Where(c => nucleosVistos.Contains(g.Nucleo(c))))
                    .ToList();
                if (ordem.Count < quantidade)
                    ordem.AddRange(Util.Embaralhar(chaves.Where(vistos.Contains), rnd));
            }
            filas.Add(new Queue<string>(ordem));
        }

        // Rodízio entre os tópicos para misturar os conteúdos.
        var escolhidos = new List<Exercicio>();
        var nucleosDaRodada = new HashSet<string>();
        var ordemTopicos = Util.Embaralhar(Enumerable.Range(0, topicos.Count), rnd);
        bool avancou = true;
        while (escolhidos.Count < quantidade && avancou)
        {
            avancou = false;
            foreach (int i in ordemTopicos)
            {
                if (escolhidos.Count >= quantidade) break;
                var g = _geradores[topicos[i]];
                var fila = filas[i];
                while (fila.Count > 0)
                {
                    var chave = fila.Dequeue();
                    if (!nucleosDaRodada.Add(g.Nucleo(chave))) continue;
                    escolhidos.Add(g.Criar(chave, rnd));
                    avancou = true;
                    break;
                }
            }
        }

        return new Rodada(escolhidos, disponiveis, novos, recomecou);
    }
}
