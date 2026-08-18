import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

import { API_BASE } from '@core/api';

@Injectable({ providedIn: 'root' })
export class ExcluirBeneficiarioUseCase {
  private readonly http = inject(HttpClient);
  private readonly base = inject(API_BASE);

  executarAsync(id: string): Observable<void> {
    return this.http.delete<void>(`${this.base}/beneficiarios/${id}`);
  }
}
