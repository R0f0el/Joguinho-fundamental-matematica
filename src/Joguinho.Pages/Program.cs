using Joguinho.Core;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.JSInterop;

// Versão para GitHub Pages: o mesmo núcleo C# roda dentro do navegador (WebAssembly).
// O jogo (app.js) chama os métodos abaixo em vez da API do servidor.
var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<Vazio>("#blazor");
await builder.Build().RunAsync();

/// <summary>Componente vazio: o Blazor só é usado para rodar o C#; a tela é feita em app.js.</summary>
public sealed class Vazio : ComponentBase
{
}

public static class JogoApi
{
    private static readonly BancoDeExercicios Banco = new();

    [JSInvokable]
    public static object[] Topicos() =>
        Banco.Topicos
            .Select(t => (object)new { Topico = t, Nivel1 = Banco.Total(t, 1), Nivel2 = Banco.Total(t, 2) })
            .ToArray();

    [JSInvokable]
    public static Rodada Sortear(PedidoRodada pedido) => Banco.Sortear(pedido);
}
