// Desenhos (SVG 40x40) usados nos exercícios. A chave é o campo "desenho" vindo da API.
window.FIGURAS = {
  maca: '<g stroke="#2b2b2b" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M20 11 q2 -6 7 -7" fill="none"/><ellipse cx="27" cy="7" rx="5" ry="2.8" fill="#4f8f3f"/><circle cx="20" cy="24" r="13" fill="#e74c3c"/></g>',
  estrela: '<polygon points="20,3 24.9,14.2 37,15 27.8,23 30.5,35 20,28.7 9.5,35 12.2,23 3,15 15.1,14.2" fill="#f7c948" stroke="#2b2b2b" stroke-width="2" stroke-linejoin="round"/>',
  balao: '<g stroke="#2b2b2b" stroke-width="2" stroke-linejoin="round"><ellipse cx="20" cy="15" rx="11" ry="13" fill="#4a90d9"/><path d="M20 28 q-4 5 0 10" fill="none"/></g>',
  bola: '<g stroke="#2b2b2b" stroke-width="2" stroke-linejoin="round"><circle cx="20" cy="20" r="15" fill="#ffffff"/><path d="M20 5 A15 15 0 0 1 35 20 L20 20 Z" fill="#e74c3c"/><path d="M20 35 A15 15 0 0 1 5 20 L20 20 Z" fill="#4a90d9"/></g>',
  flor: '<g stroke="#2b2b2b" stroke-width="2" stroke-linejoin="round"><path d="M20 24 V38" stroke="#4f8f3f" stroke-width="3" fill="none"/><circle cx="20" cy="8" r="6" fill="#f28cb1"/><circle cx="28.5" cy="14" r="6" fill="#f28cb1"/><circle cx="25.5" cy="24" r="6" fill="#f28cb1"/><circle cx="14.5" cy="24" r="6" fill="#f28cb1"/><circle cx="11.5" cy="14" r="6" fill="#f28cb1"/><circle cx="20" cy="17" r="5" fill="#f7c948"/></g>',
  peixe: '<g stroke="#2b2b2b" stroke-width="2" stroke-linejoin="round"><path d="M28 20 L38 11 L38 29 Z" fill="#4a90d9"/><ellipse cx="17" cy="20" rx="13" ry="9" fill="#6fb1e8"/><circle cx="10" cy="18" r="1.8" fill="#2b2b2b"/></g>',
  carrinho: '<g stroke="#2b2b2b" stroke-width="2" stroke-linejoin="round"><path d="M11 17 L15 9 H26 L30 17 Z" fill="#e74c3c"/><rect x="3" y="17" width="34" height="11" rx="3" fill="#e74c3c"/><path d="M16 11 H25 L27 16 H14 Z" fill="#d6ecff"/><circle cx="12" cy="29" r="4.5" fill="#333333"/><circle cx="28" cy="29" r="4.5" fill="#333333"/></g>',
  coracao: '<path d="M20 34 C6 24 3 14 10 9.5 C14.5 7 19 9.5 20 14 C21 9.5 25.5 7 30 9.5 C37 14 34 24 20 34 Z" fill="#e2557a" stroke="#2b2b2b" stroke-width="2" stroke-linejoin="round"/>',
  lapis: '<g stroke="#2b2b2b" stroke-width="2" stroke-linejoin="round"><rect x="15" y="3" width="10" height="5" fill="#f28cb1"/><rect x="15" y="8" width="10" height="21" fill="#f7c948"/><path d="M15 29 L20 38 L25 29 Z" fill="#f3d9b1"/><path d="M18.5 35 L20 38 L21.5 35 Z" fill="#2b2b2b"/></g>',
  borboleta: '<g stroke="#2b2b2b" stroke-width="2" stroke-linejoin="round"><ellipse cx="11.5" cy="13" rx="8" ry="7" fill="#8e5bb5"/><ellipse cx="28.5" cy="13" rx="8" ry="7" fill="#8e5bb5"/><ellipse cx="13" cy="26" rx="6" ry="6" fill="#c39be0"/><ellipse cx="27" cy="26" rx="6" ry="6" fill="#c39be0"/><ellipse cx="20" cy="20" rx="2.5" ry="12" fill="#2b2b2b"/></g>',
  biscoito: '<g stroke="#2b2b2b" stroke-width="2"><circle cx="20" cy="20" r="15" fill="#d9a35b"/><g fill="#6b4423" stroke="none"><circle cx="14" cy="14" r="2.2"/><circle cx="25" cy="13" r="2"/><circle cx="20" cy="22" r="2.2"/><circle cx="12" cy="25" r="2"/><circle cx="27" cy="26" r="2.2"/></g></g>',
  patinho: '<g stroke="#2b2b2b" stroke-width="2" stroke-linejoin="round"><ellipse cx="23" cy="27" rx="13" ry="9" fill="#f7d046"/><circle cx="13" cy="14" r="7.5" fill="#f7d046"/><path d="M6 13 L0.5 15.5 L6 18 Z" fill="#f08a24"/><circle cx="12" cy="12" r="1.6" fill="#2b2b2b"/></g>'
};

window.RISCO = '<svg class="risco" viewBox="0 0 40 40" width="34" height="34" aria-hidden="true"><path d="M6 6 L34 34 M34 6 L6 34" stroke="#c0392b" stroke-width="4" stroke-linecap="round"/></svg>';

window.desenhoSvg = function (nome, tamanho) {
  var corpo = window.FIGURAS[nome] || window.FIGURAS.estrela;
  return '<svg viewBox="0 0 40 40" width="' + tamanho + '" height="' + tamanho + '" aria-hidden="true">' + corpo + '</svg>';
};
