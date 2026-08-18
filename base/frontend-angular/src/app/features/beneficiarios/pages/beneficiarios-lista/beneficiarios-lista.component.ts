import { HttpErrorResponse } from '@angular/common/http';
import { ChangeDetectionStrategy, Component, DestroyRef, computed, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormsModule } from '@angular/forms';

import { mensagemDeErro } from '@core/api';
import { ListarPlanosUseCase } from '@features/planos/data-access/listar-planos.use-case';
import type { Plano } from '@features/planos/models/plano.model';
import { ExcluirBeneficiarioUseCase } from '../../data-access/excluir-beneficiario.use-case';
import { ListarBeneficiariosUseCase } from '../../data-access/listar-beneficiarios.use-case';
import { Beneficiario, ListarBeneficiariosRequest, StatusBeneficiario } from '../../models/beneficiario.model';
import { BeneficiarioFormularioComponent } from '../../ui/beneficiario-formulario/beneficiario-formulario.component';
import { CpfFormatPipe } from '../../utils/cpf-format.pipe';

@Component({
  selector: 'app-beneficiarios-lista',
  imports: [FormsModule, BeneficiarioFormularioComponent, CpfFormatPipe],
  templateUrl: './beneficiarios-lista.component.html',
  styleUrl: './beneficiarios-lista.component.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class BeneficiariosListaComponent {
  private readonly listarBeneficiarios = inject(ListarBeneficiariosUseCase);
  private readonly excluirBeneficiario = inject(ExcluirBeneficiarioUseCase);
  private readonly listarPlanos = inject(ListarPlanosUseCase);
  private readonly destroyRef = inject(DestroyRef);

  protected readonly beneficiarios = signal<Beneficiario[]>([]);
  protected readonly planos = signal<Plano[]>([]);
  protected readonly pagina = signal(1);
  protected readonly tamanho = signal(10);
  protected readonly total = signal(0);
  protected readonly status = signal<StatusBeneficiario | ''>('');
  protected readonly planoId = signal('');
  protected readonly carregando = signal(true);
  protected readonly erro = signal<string | null>(null);
  protected readonly mensagem = signal<string | null>(null);
  protected readonly beneficiarioEmEdicao = signal<Beneficiario | null | undefined>(undefined);

  protected readonly totalPaginas = computed(() =>
    Math.max(1, Math.ceil(this.total() / this.tamanho()))
  );

  protected readonly planosPorId = computed(() => {
    const nomes = new Map<string, string>();

    for (const plano of this.planos()) {
      nomes.set(plano.id, plano.nome);
    }

    return nomes;
  });

  constructor() {
    this.carregarPlanos();
    this.carregar();
  }

  protected nomeDoPlano(planoId: string): string {
    return this.planosPorId().get(planoId) ?? planoId;
  }

  protected aplicarFiltros(): void {
    this.pagina.set(1);
    this.carregar();
  }

  protected limparFiltros(): void {
    this.status.set('');
    this.planoId.set('');
    this.pagina.set(1);
    this.carregar();
  }

  protected irPara(pagina: number): void {
    if (pagina < 1 || pagina > this.totalPaginas()) {
      return;
    }

    this.pagina.set(pagina);
    this.carregar();
  }

  protected novo(): void {
    this.beneficiarioEmEdicao.set(null);
    this.erro.set(null);
    this.mensagem.set(null);
  }

  protected editar(beneficiario: Beneficiario): void {
    this.beneficiarioEmEdicao.set(beneficiario);
    this.erro.set(null);
    this.mensagem.set(null);
  }

  protected fecharFormulario(): void {
    this.beneficiarioEmEdicao.set(undefined);
  }

  protected aoSalvar(): void {
    this.beneficiarioEmEdicao.set(undefined);
    this.mensagem.set('Beneficiário salvo.');
    this.carregar();
  }

  protected excluir(beneficiario: Beneficiario): void {
    if (!confirm(`Excluir ${beneficiario.nome_completo}?`)) {
      return;
    }

    this.erro.set(null);
    this.mensagem.set(null);

    this.excluirBeneficiario
      .executarAsync(beneficiario.id)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: () => {
          this.mensagem.set('Beneficiário excluído.');
          this.carregar();
        },
        error: (resposta: HttpErrorResponse) => {
          this.erro.set(mensagemDeErro(resposta));
        }
      });
  }

  protected carregar(): void {
    this.carregando.set(true);
    this.erro.set(null);

    const filtro: ListarBeneficiariosRequest = {
      pagina: this.pagina(),
      tamanho: this.tamanho(),
      status: this.status(),
      plano_id: this.planoId()
    };

    this.listarBeneficiarios
      .executarAsync(filtro)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (resposta) => {
          this.beneficiarios.set(resposta.dados);
          this.pagina.set(resposta.pagina);
          this.tamanho.set(resposta.tamanho);
          this.total.set(resposta.total);
          this.carregando.set(false);
        },
        error: (resposta: HttpErrorResponse) => {
          this.erro.set(mensagemDeErro(resposta));
          this.carregando.set(false);
        }
      });
  }

  private carregarPlanos(): void {
    this.listarPlanos
      .executarAsync()
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (planos) => this.planos.set(planos),
        error: (resposta: HttpErrorResponse) => this.erro.set(mensagemDeErro(resposta))
      });
  }
}
