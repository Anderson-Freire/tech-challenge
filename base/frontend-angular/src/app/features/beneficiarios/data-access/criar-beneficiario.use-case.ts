import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

import { API_BASE } from '@core/api';
import { CriarBeneficiarioRequest, CriarBeneficiarioResponse } from '../models/beneficiario.model';

@Injectable({ providedIn: 'root' })
export class CriarBeneficiarioUseCase {
  private readonly http = inject(HttpClient);
  private readonly base = inject(API_BASE);

  executarAsync(corpo: CriarBeneficiarioRequest): Observable<CriarBeneficiarioResponse> {
    return this.http.post<CriarBeneficiarioResponse>(`${this.base}/beneficiarios`, corpo);
  }
}
