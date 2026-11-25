using Joguinho.Core;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<BancoDeExercicios>();

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

// Lista os tópicos com quantos exercícios existem em cada nível.
app.MapGet("/api/topicos", (BancoDeExercicios banco) =>
    banco.Topicos.Select(t => new
    {
        Topico = t,
        Nivel1 = banco.Total(t, 1),
        Nivel2 = banco.Total(t, 2),
    }));

// Sorteia uma rodada. O jogo manda as chaves já vistas para não repetir.
app.MapPost("/api/rodada", (PedidoRodada pedido, BancoDeExercicios banco) => banco.Sortear(pedido));

app.Run();

public partial class Program { }
