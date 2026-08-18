import { HttpErrorResponse } from '@angular/common/http';

import { ErroDaApi } from './erro-da-api.model';

export function mensagemDeErro(resposta: HttpErrorResponse): string {
  if (resposta.status === 0) {
    return 'Não foi possível falar com a API. Confira se ela está no ar.';
  }

  const corpo = resposta.error as ErroDaApi | null;

  if (!corpo?.mensagem) {
    return `A API respondeu ${resposta.status}.`;
  }

  if (corpo.detalhes?.length) {
    const campos = corpo.detalhes.map((detalhe) => `${detalhe.campo} (${detalhe.regra})`).join(', ');
    return `${corpo.mensagem}: ${campos}`;
  }

  return corpo.mensagem;
}
