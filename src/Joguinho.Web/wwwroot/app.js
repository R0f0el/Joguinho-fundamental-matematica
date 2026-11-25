(function () {
  'use strict';

  var NOMES_TOPICOS = {
    Livro: 'Do livro (pág. 23 e 24)',
    Contagem: 'Contando',
    Adicao: 'Adição',
    Subtracao: 'Subtração',
    Comparacao: 'Mais ou menos',
    Numeros: 'Números',
    Tabela: 'Tabelas',
    Grafico: 'Gráficos',
    PinteGrafico: 'Pinte o gráfico'
  };
  var ELOGIOS = ['Muito bem!', 'Isso mesmo!', 'Acertou!', 'Que legal!', 'Excelente!', 'Mandou bem!'];
  var CHAVE_VISTOS = 'joguinho-matematica.vistos';
  var CHAVE_PREFS = 'joguinho-matematica.prefs';
  var POR_RODADA = 10;

  var estado = {
    topicos: [],          // tópicos escolhidos
    todosTopicos: [],     // [{topico, nivel1, nivel2}]
    nivel: 1,
    vistos: carregarVistos(),
    rodada: [],
    feitos: 0
  };

  // ---------- utilidades ----------
  function $(id) { return document.getElementById(id); }

  function el(tag, classe, texto) {
    var e = document.createElement(tag);
    if (classe) e.className = classe;
    if (texto !== undefined && texto !== null) e.textContent = texto;
    return e;
  }

  function lerStorage(chave, padrao) {
    try { var v = localStorage.getItem(chave); return v ? JSON.parse(v) : padrao; } catch (e) { return padrao; }
  }
  function gravarStorage(chave, valor) {
    try { localStorage.setItem(chave, JSON.stringify(valor)); } catch (e) { /* sem armazenamento: segue sem lembrar */ }
  }
  function carregarVistos() { return new Set(lerStorage(CHAVE_VISTOS, [])); }
  function marcarVisto(chave) {
    estado.vistos.add(chave);
    gravarStorage(CHAVE_VISTOS, Array.from(estado.vistos));
  }

  function falar(texto) {
    try {
      var t = String(texto)
        .replace(/(^|\s)\?(?=\s|$)/g, '$1 quanto ')
        .replace(/\+/g, ' mais ')
        .replace(/−/g, ' menos ')
        .replace(/=/g, ' igual a ')
        .replace(/[“”]/g, '');
      var u = new SpeechSynthesisUtterance(t);
      u.lang = 'pt-BR';
      u.rate = 0.9;
      window.speechSynthesis.cancel();
      window.speechSynthesis.speak(u);
    } catch (e) { /* navegador sem voz */ }
  }

  // Chama o C#: no GitHub Pages ele roda no navegador (WebAssembly); no servidor, via API.
  function chamar(metodo, dados) {
    if (window.JOGUINHO_WASM) {
      return window.JOGUINHO_WASM.then(function () {
        return dados === undefined
          ? DotNet.invokeMethodAsync('Joguinho.Pages', metodo)
          : DotNet.invokeMethodAsync('Joguinho.Pages', metodo, dados);
      });
    }
    var url = metodo === 'Topicos' ? 'api/topicos' : 'api/rodada';
    var opcoes = dados === undefined ? undefined : {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(dados)
    };
    return fetch(url, opcoes).then(function (r) { if (!r.ok) throw new Error('HTTP ' + r.status); return r.json(); });
  }

  function mensagem(texto) { $('mensagem').textContent = texto; }

  function atualizarPlacar() {
    if ($('tela-jogo').hidden) { $('placar').textContent = ''; return; }
    $('placar').textContent = estado.feitos + ' / ' + estado.rodada.length;
  }

  // ---------- menu ----------
  function montarMenu() {
    var prefs = lerStorage(CHAVE_PREFS, null);
    estado.nivel = prefs && prefs.nivel === 2 ? 2 : 1;
    var caixa = $('topicos');
    caixa.innerHTML = '';
    var escolhidos = prefs && prefs.topicos && prefs.topicos.length ? prefs.topicos : estado.todosTopicos.map(function (t) { return t.topico; });
    estado.topicos = escolhidos.filter(function (t) { return NOMES_TOPICOS[t]; });

    estado.todosTopicos.forEach(function (t) {
      var b = el('button', 'chip', NOMES_TOPICOS[t.topico] || t.topico);
      b.type = 'button';
      b.setAttribute('aria-pressed', 'false');
      b.addEventListener('click', function () {
        var i = estado.topicos.indexOf(t.topico);
        if (i >= 0) estado.topicos.splice(i, 1); else estado.topicos.push(t.topico);
        pintarMenu();
      });
      b.dataset.topico = t.topico;
      caixa.appendChild(b);
    });

    Array.prototype.forEach.call(document.querySelectorAll('#niveis .chip'), function (b) {
      b.onclick = function () { estado.nivel = Number(b.dataset.nivel); pintarMenu(); };
    });
    pintarMenu();
  }

  function pintarMenu() {
    Array.prototype.forEach.call(document.querySelectorAll('#topicos .chip'), function (b) {
      var ativo = estado.topicos.indexOf(b.dataset.topico) >= 0;
      b.classList.toggle('ativo', ativo);
      b.setAttribute('aria-pressed', String(ativo));
    });
    Array.prototype.forEach.call(document.querySelectorAll('#niveis .chip'), function (b) {
      var ativo = Number(b.dataset.nivel) === estado.nivel;
      b.classList.toggle('ativo', ativo);
      b.setAttribute('aria-pressed', String(ativo));
    });
    var total = 0;
    estado.todosTopicos.forEach(function (t) {
      if (estado.topicos.indexOf(t.topico) >= 0) total += estado.nivel === 2 ? t.nivel2 : t.nivel1;
    });
    $('progresso').textContent = total
      ? 'Há ' + total.toLocaleString('pt-BR') + ' exercícios diferentes nesses assuntos. Você já fez ' + estado.vistos.size.toLocaleString('pt-BR') + ' no total.'
      : 'Escolha pelo menos um assunto.';
    $('comecar').disabled = estado.topicos.length === 0;
    gravarStorage(CHAVE_PREFS, { nivel: estado.nivel, topicos: estado.topicos });
  }

  // ---------- rodada ----------
  function novaRodada() {
    $('tela-menu').hidden = true;
    $('tela-jogo').hidden = false;
    $('parabens').hidden = true;
    var lista = $('exercicios');
    lista.innerHTML = '';
    lista.appendChild(el('p', 'ajuda', 'Carregando exercícios...'));
    mensagem('Preparando exercícios novos...');

    chamar('Sortear', {
      topicos: estado.topicos,
      quantidade: POR_RODADA,
      nivel: estado.nivel,
      vistos: Array.from(estado.vistos)
    })
      .then(function (rodada) {
        estado.rodada = rodada.exercicios || [];
        mostrarRodada();
        mensagem(rodada.recomecou
          ? 'Você já fez todos de algum assunto! Vamos recomeçar esse.'
          : 'Toque no alto-falante para ouvir e escolha a resposta certa.');
        atualizarPlacar();
        window.scrollTo(0, 0);
      })
      .catch(function () {
        lista.innerHTML = '';
        var erro = el('div', 'erro', 'Não consegui preparar os exercícios. Recarregue a página.');
        lista.appendChild(erro);
        mensagem('Ops! Algo deu errado.');
      });
  }

  function mostrarRodada() {
    var lista = $('exercicios');
    estado.feitos = 0;
    lista.innerHTML = '';
    $('parabens').hidden = true;
    estado.rodada.forEach(function (ex) { lista.appendChild(cartao(ex)); });
    atualizarPlacar();
  }

  function repetirRodada() {
    if (!estado.rodada.length) return;
    mostrarRodada();
    mensagem('Vamos fazer a mesma rodada de novo!');
    window.scrollTo(0, 0);
  }

  function refazer(ex, caixa) {
    var novo = cartao(ex);
    caixa.replaceWith(novo);
    estado.feitos = Math.max(0, estado.feitos - 1);
    $('parabens').hidden = true;
    atualizarPlacar();
    mensagem('Tente de novo esse exercício!');
  }

  function concluir(ex, caixa) {
    if (caixa.classList.contains('feito')) return;
    caixa.classList.add('feito');
    caixa.appendChild(el('p', 'acertou', 'Acertou!'));
    var de_novo = el('button', 'botao contorno pequeno', 'Fazer de novo');
    de_novo.type = 'button';
    de_novo.addEventListener('click', function () { refazer(ex, caixa); });
    caixa.appendChild(de_novo);
    marcarVisto(ex.chave);
    estado.feitos++;
    atualizarPlacar();
    var elogio = ELOGIOS[estado.feitos % ELOGIOS.length];
    mensagem(elogio);
    falar(elogio);
    if (estado.feitos === estado.rodada.length) {
      $('parabens').hidden = false;
      mensagem('Rodada completa! Parabéns!');
    }
  }

  // ---------- cartão de exercício ----------
  function cartao(ex) {
    var caixa = el('article', 'cartao');
    caixa.appendChild(el('span', 'etiqueta', NOMES_TOPICOS[ex.topico] || ex.topico));

    var linha = el('div', 'pergunta');
    var btn = el('button', 'falar');
    btn.type = 'button';
    btn.setAttribute('aria-label', 'Ouvir a pergunta');
    btn.innerHTML = '<svg viewBox="0 0 24 24" width="22" height="22" aria-hidden="true" fill="none" stroke="currentColor" stroke-width="2.2" stroke-linecap="round" stroke-linejoin="round"><path d="M4 9v6h4l5 4V5L8 9z" fill="currentColor"/><path d="M16.5 8.5a5 5 0 0 1 0 7"/><path d="M19 6a8.5 8.5 0 0 1 0 12"/></svg>';
    btn.addEventListener('click', function () { falar(ex.enunciado); });
    linha.appendChild(btn);
    linha.appendChild(el('p', null, ex.enunciado));
    caixa.appendChild(linha);

    if (ex.tabela) caixa.appendChild(tabela(ex.tabela));
    if (ex.grafico && ex.topico !== 'PinteGrafico') caixa.appendChild(grafico(ex.grafico));
    if (ex.figuras && ex.figuras.length) caixa.appendChild(figuras(ex.figuras));

    if (ex.topico === 'PinteGrafico') {
      caixa.appendChild(pinteGrafico(ex, caixa));
    } else {
      caixa.appendChild(opcoes(ex, caixa));
    }
    return caixa;
  }

  function figuras(lista) {
    var caixa = el('div', 'figuras');
    lista.forEach(function (f) {
      if (f.simbolo) {
        caixa.appendChild(el('span', 'sinal', f.simbolo));
        return;
      }
      var s = el('span', 'fig');
      s.innerHTML = window.desenhoSvg(f.desenho, 34) + (f.riscado ? window.RISCO : '');
      caixa.appendChild(s);
    });
    return caixa;
  }

  function opcoes(ex, caixa) {
    var area = el('div', 'opcoes');
    var botoes = [];
    ex.opcoes.forEach(function (o, i) {
      var b = el('button', 'opcao');
      b.type = 'button';
      if (o.figuras && o.figuras.length) {
        b.classList.add('grupo');
        var mini = el('span', 'mini');
        mini.innerHTML = o.figuras.map(function (f) { return window.desenhoSvg(f.desenho, 26); }).join('');
        b.appendChild(mini);
        b.appendChild(el('span', null, o.texto));
        b.setAttribute('aria-label', o.texto + ': ' + o.figuras.length + ' desenhos');
      } else {
        b.textContent = o.texto;
      }
      b.addEventListener('click', function () {
        if (caixa.classList.contains('feito')) { falar(o.texto); return; }
        if (i === ex.indiceCorreto) {
          botoes.forEach(function (x, j) { x.classList.toggle('apagada', j !== i); });
          b.classList.add('certa');
          concluir(ex, caixa);
        } else {
          b.classList.add('errada');
          mensagem('Ops! Tente de novo.');
          falar('Tente de novo');
          setTimeout(function () { b.classList.remove('errada'); }, 900);
        }
      });
      botoes.push(b);
      area.appendChild(b);
    });
    return area;
  }

  function tabela(t) {
    var bloco = el('div', 'bloco');
    bloco.appendChild(el('p', 'tabela-titulo', t.titulo));
    var caixa = el('div', 'tabela-caixa');
    var tab = el('table');
    var cab = el('thead');
    var tr = el('tr');
    [t.coluna, 'Classificação', 'Votos', 'Quantidade'].forEach(function (c) { tr.appendChild(el('th', null, c)); });
    cab.appendChild(tr);
    tab.appendChild(cab);
    var corpo = el('tbody');
    t.linhas.forEach(function (l) {
      var r = el('tr');
      r.appendChild(el('td', null, l.nome));
      r.appendChild(el('td', null, l.esconderClassificacao ? '?' : l.classificacao));
      r.appendChild(el('td', 'votos', l.esconderNumero ? '?' : String(l.votos)));
      var td = el('td');
      var bol = el('span', 'bolinhas');
      for (var i = 0; i < l.votos; i++) bol.appendChild(el('span', 'bolinha'));
      td.appendChild(bol);
      r.appendChild(td);
      corpo.appendChild(r);
    });
    tab.appendChild(corpo);
    caixa.appendChild(tab);
    bloco.appendChild(caixa);
    return bloco;
  }

  function colunasGrade(n) { return '22px repeat(' + n + ', minmax(0, 1fr))'; }

  function grafico(g) {
    var caixa = el('div', 'grafico');
    caixa.appendChild(el('p', 'tabela-titulo', g.titulo));
    var grade = el('div', 'grafico-grade');
    grade.style.gridTemplateColumns = colunasGrade(g.barras.length);
    grade.appendChild(eixo(g.maximo, 22));
    g.barras.forEach(function (b) {
      var col = el('div', 'coluna');
      for (var nivel = g.maximo; nivel >= 1; nivel--) {
        var c = el('span', 'celula' + (b.valor >= nivel ? ' cheia' : ''));
        c.style.height = '22px';
        col.appendChild(c);
      }
      grade.appendChild(col);
    });
    caixa.appendChild(grade);
    caixa.appendChild(nomes(g.barras.map(function (b) { return b.nome; })));
    if (g.fonte) caixa.appendChild(el('p', 'fonte', 'Fonte: ' + g.fonte));
    return caixa;
  }

  function eixo(maximo, altura) {
    var e = el('div', 'eixo');
    for (var n = maximo; n >= 1; n--) {
      var s = el('span', null, String(n));
      s.style.height = altura + 'px';
      e.appendChild(s);
    }
    return e;
  }

  function nomes(lista) {
    var linha = el('div', 'nomes');
    linha.style.gridTemplateColumns = colunasGrade(lista.length);
    linha.appendChild(el('span'));
    var spans = lista.map(function (n) { var s = el('span', null, n); linha.appendChild(s); return s; });
    linha.spans = spans;
    return linha;
  }

  var CORES_BARRAS = ['#e74c3c', '#f0a500', '#8e5bb5', '#2e9e5b', '#4a90d9'];

  function pinteGrafico(ex, cartaoEl) {
    var g = ex.grafico;
    var alturas = g.barras.map(function () { return 0; });
    var area = el('div', 'bloco');
    var caixa = el('div', 'grafico');
    caixa.appendChild(el('p', 'tabela-titulo', 'Pinte aqui: ' + g.titulo));
    var grade = el('div', 'grafico-grade');
    grade.style.gridTemplateColumns = colunasGrade(g.barras.length);
    grade.appendChild(eixo(g.maximo, 36));
    var colunas = g.barras.map(function (b, i) {
      var col = el('div', 'coluna');
      var celulas = [];
      for (var nivel = g.maximo; nivel >= 1; nivel--) {
        (function (nv) {
          var c = el('button', 'celula');
          c.type = 'button';
          c.style.height = '36px';
          c.setAttribute('aria-label', b.nome + ': pintar até ' + nv);
          c.addEventListener('click', function () {
            if (cartaoEl.classList.contains('feito')) return;
            alturas[i] = alturas[i] === nv ? nv - 1 : nv;
            pintar();
            mensagem(b.nome + ': ' + alturas[i] + (alturas[i] === 1 ? ' quadradinho.' : ' quadradinhos.'));
          });
          celulas.push({ botao: c, nivel: nv });
          col.appendChild(c);
        })(nivel);
      }
      grade.appendChild(col);
      return celulas;
    });
    caixa.appendChild(grade);
    var linhaNomes = nomes(g.barras.map(function (b) { return b.nome; }));
    caixa.appendChild(linhaNomes);
    area.appendChild(caixa);

    function pintar() {
      colunas.forEach(function (celulas, i) {
        celulas.forEach(function (c) {
          c.botao.style.background = alturas[i] >= c.nivel ? CORES_BARRAS[i % CORES_BARRAS.length] : '#ffffff';
        });
        linhaNomes.spans[i].classList.remove('ok', 'nao');
      });
    }

    var botoes = el('div', 'botoes');
    var conferir = el('button', 'botao', 'Conferir');
    conferir.type = 'button';
    conferir.addEventListener('click', function () {
      if (cartaoEl.classList.contains('feito')) return;
      var tudoCerto = true;
      g.barras.forEach(function (b, i) {
        var ok = alturas[i] === b.valor;
        if (!ok) tudoCerto = false;
        linhaNomes.spans[i].classList.toggle('ok', ok);
        linhaNomes.spans[i].classList.toggle('nao', !ok);
      });
      if (tudoCerto) {
        conferir.disabled = true;
        concluir(ex, cartaoEl);
      } else {
        mensagem('Quase! Confira as colunas com nome em vermelho.');
        falar('Quase! Confira as colunas com nome em vermelho.');
      }
    });
    botoes.appendChild(conferir);
    area.appendChild(botoes);
    return area;
  }

  // ---------- início ----------
  $('comecar').addEventListener('click', novaRodada);
  $('proxima').addEventListener('click', novaRodada);
  $('repetir').addEventListener('click', repetirRodada);
  $('voltar').addEventListener('click', function () {
    $('tela-jogo').hidden = true;
    $('tela-menu').hidden = false;
    mensagem('Escolha o que quer treinar.');
    atualizarPlacar();
    pintarMenu();
  });

  chamar('Topicos')
    .then(function (lista) { estado.todosTopicos = lista; montarMenu(); mensagem('Escolha o que quer treinar.'); })
    .catch(function () {
      $('progresso').textContent = 'Não consegui carregar o jogo. Recarregue a página.';
      $('comecar').disabled = true;
    });
})();
