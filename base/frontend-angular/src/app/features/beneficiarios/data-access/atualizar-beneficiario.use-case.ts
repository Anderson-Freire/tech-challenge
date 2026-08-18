import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

import { API_BASE } from '@core/api';
import { AtualizarBeneficiarioRequest, AtualizarBeneficiarioResponse } from '../models/beneficiario.model';

@Injectable({ providedIn: 'root' })
export class AtualizarBeneficiarioUseCase {
  private readonly http = inject(HttpClient);
  private readonly base = inject(API_BASE);

  executarAsync(
    id: string,
    corpo: AtualizarBeneficiarioRequest
  ): Observable<AtualizarBeneficiarioResponse> {
    return this.http.put<AtualizarBeneficiarioResponse>(`${this.base}/beneficiarios/${id}`, corpo);
  }
}
