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

Os exercícios são **gerados em C#**. O jogo guarda no navegador quais já
apareceram e pede ao servidor só exercícios novos, então eles se repetem o
mínimo possível. Quando todos de um tema já foram vistos, o ciclo recomeça.

## Como rodar

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
tests/Joguinho.Tests  testes xUnit
```
