import { HttpErrorResponse } from '@angular/common/http';
import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

import { mensagemDeErro } from '@core/api';
import { ListarPlanosUseCase } from '../../data-access/listar-planos.use-case';
import { Plano } from '../../models/plano.model';

@Component({
  selector: 'app-planos-lista',
  templateUrl: './planos-lista.component.html',
  styleUrl: './planos-lista.component.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class PlanosListaComponent {
  private readonly listarPlanos = inject(ListarPlanosUseCase);

  protected readonly planos = signal<Plano[]>([]);
  protected readonly carregando = signal(true);
  protected readonly erro = signal<string | null>(null);

  constructor() {
    this.carregar();
  }

  protected carregar(): void {
    this.carregando.set(true);
    this.erro.set(null);

    this.listarPlanos
      .executarAsync()
      .pipe(takeUntilDestroyed())
      .subscribe({
        next: (planos) => {
          this.planos.set(planos);
          this.carregando.set(false);
        },
        error: (resposta: HttpErrorResponse) => {
          this.erro.set(mensagemDeErro(resposta));
          this.carregando.set(false);
        }
      });
  }
}
