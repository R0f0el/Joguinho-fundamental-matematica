using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Joguinho.Tests;

public class ApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _cliente;

    public ApiTests(WebApplicationFactory<Program> fabrica) => _cliente = fabrica.CreateClient();

    [Fact]
    public async Task Lista_os_topicos()
    {
        var json = await _cliente.GetFromJsonAsync<JsonElement>("/api/topicos");
        Assert.Equal(9, json.GetArrayLength());
        var primeiro = json[0];
        Assert.Equal(JsonValueKind.String, primeiro.GetProperty("topico").ValueKind);
        Assert.True(primeiro.GetProperty("nivel1").GetInt32() > 0);
    }

    [Fact]
    public async Task Sorteia_rodada_sem_os_vistos()
    {
        var pedido = new
        {
            topicos = new[] { "Adicao", "Tabela" },
            quantidade = 6,
            nivel = 1,
            vistos = new[] { "adicao|1|1|conta" },
        };
        var resposta = await _cliente.PostAsJsonAsync("/api/rodada", pedido);
        resposta.EnsureSuccessStatusCode();
        var json = await resposta.Content.ReadFromJsonAsync<JsonElement>();

        var exercicios = json.GetProperty("exercicios");
        Assert.Equal(6, exercicios.GetArrayLength());
        foreach (var ex in exercicios.EnumerateArray())
        {
            Assert.NotEqual("adicao|1|1|conta", ex.GetProperty("chave").GetString());
            Assert.False(string.IsNullOrEmpty(ex.GetProperty("enunciado").GetString()));
            Assert.Contains(ex.GetProperty("topico").GetString(), new[] { "Adicao", "Tabela" });
        }
    }

    [Fact]
    public async Task Serve_o_jogo()
    {
        var html = await _cliente.GetStringAsync("/");
        Assert.Contains("Meu Joguinho de Matemática", html);
    }
}
