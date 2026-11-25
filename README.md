# Joguinho de Matemática — 1º ano

Jogo de matemática para crianças do 1º ano do Ensino Fundamental.
A criança ouve a pergunta (botão de alto-falante) e toca na resposta certa.

## Conteúdos

- Tabelas e gráficos (inclui os exercícios das páginas 23 e 24 do livro)
- Pintar gráfico de barras (um quadradinho para cada voto)
- Contas de adição
- Contas de subtração
- Quantificação e representação com números e desenhos
- Comparação (quem tem mais / menos) e números (antes, depois, por extenso)

Os exercícios são **gerados em C#**: cerca de 2.700 diferentes no nível
Fácil (até 10) e 5.700 no Desafio (até 20). O jogo guarda no navegador quais já
apareceram e pede ao servidor só exercícios novos, então eles se repetem o
mínimo possível. Se a criança quiser treinar de novo, pode usar
**Repetir rodada** (refaz a rodada inteira) ou **Fazer de novo** (refaz um
exercício já acertado). Quando todos de um tema já foram vistos, o ciclo recomeça.

## Jogar online

https://r0f0el.github.io/Joguinho-fundamental-matematica/

A cada push na `main`, o GitHub Actions publica o jogo no GitHub Pages.
Lá o C# roda dentro do navegador (Blazor WebAssembly), sem servidor.

## Como rodar no computador

Precisa do [.NET 8 SDK](https://dotnet.microsoft.com/download).

```bash
dotnet run --project src/Joguinho.Web
```

Abra o endereço que aparecer no terminal (ex.: http://localhost:5000).

## Testes

```bash
dotnet test tests/Joguinho.Tests
```

## Estrutura

```
src/Joguinho.Core   geradores de exercícios e seletor sem repetição
src/Joguinho.Web    API (ASP.NET Core) + jogo (wwwroot)
src/Joguinho.Pages  versão WebAssembly para o GitHub Pages
tests/Joguinho.Tests  testes xUnit
```
