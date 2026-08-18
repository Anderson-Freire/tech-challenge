import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

import { API_BASE } from '@core/api';
import { ListarBeneficiariosRequest, ListarBeneficiariosResponse } from '../models/beneficiario.model';

@Injectable({ providedIn: 'root' })
export class ListarBeneficiariosUseCase {
  private readonly http = inject(HttpClient);
  private readonly base = inject(API_BASE);

  executarAsync(filtro: ListarBeneficiariosRequest): Observable<ListarBeneficiariosResponse> {
    let params = new HttpParams()
      .set('pagina', filtro.pagina)
      .set('tamanho', filtro.tamanho);

    if (filtro.status) {
      params = params.set('status', filtro.status);
    }

    if (filtro.plano_id) {
      params = params.set('plano_id', filtro.plano_id);
    }

    return this.http.get<ListarBeneficiariosResponse>(`${this.base}/beneficiarios`, {
      params
    });
  }
}
