import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

import { API_BASE } from '@core/api';
import { ListarPlanosResponse } from '../models/plano.model';

@Injectable({ providedIn: 'root' })
export class ListarPlanosUseCase {
  private readonly http = inject(HttpClient);
  private readonly base = inject(API_BASE);

  executarAsync(): Observable<ListarPlanosResponse[]> {
    return this.http.get<ListarPlanosResponse[]>(`${this.base}/planos`);
  }
}
