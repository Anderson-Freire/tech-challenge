import { HttpErrorResponse } from '@angular/common/http';
import {
  ChangeDetectionStrategy,
  Component,
  DestroyRef,
  OnInit,
  computed,
  inject,
  input,
  output,
  signal
} from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';

import { mensagemDeErro } from '@core/api';
import type { Plano } from '@features/planos/models/plano.model';
import { AtualizarBeneficiarioUseCase } from '../../data-access/atualizar-beneficiario.use-case';
import { CriarBeneficiarioUseCase } from '../../data-access/criar-beneficiario.use-case';
import {
  AtualizarBeneficiarioRequest,
  Beneficiario,
  CriarBeneficiarioRequest,
  StatusBeneficiario
} from '../../models/beneficiario.model';
import { apenasDigitos } from '../../utils/cpf.util';
import { validadorDataPassada, validadorDeCpf } from '../../validators/beneficiario.validators';

@Component({
  selector: 'app-beneficiario-formulario',
  imports: [ReactiveFormsModule],
  templateUrl: './beneficiario-formulario.component.html',
  styleUrl: './beneficiario-formulario.component.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class BeneficiarioFormularioComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly criarBeneficiario = inject(CriarBeneficiarioUseCase);
  private readonly atualizarBeneficiario = inject(AtualizarBeneficiarioUseCase);
  private readonly destroyRef = inject(DestroyRef);

  readonly planos = input.required<Plano[]>();
  readonly beneficiario = input<Beneficiario | null>(null);

  readonly salvo = output<void>();
  readonly cancelado = output<void>();

  protected readonly enviando = signal(false);
  protected readonly erro = signal<string | null>(null);
  protected readonly ehEdicao = computed(() => this.beneficiario() !== null);

  protected readonly formulario = this.fb.nonNullable.group({
    nome_completo: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(120)]],
    cpf: ['', [Validators.required, validadorDeCpf]],
    data_nascimento: ['', [Validators.required, validadorDataPassada]],
    plano_id: ['', Validators.required],
    status: this.fb.nonNullable.control<StatusBeneficiario>('ATIVO')
  });

  ngOnInit(): void {
    const atual = this.beneficiario();

    if (!atual) {
      return;
    }

    this.formulario.patchValue({
      nome_completo: atual.nome_completo,
      cpf: atual.cpf,
      data_nascimento: atual.data_nascimento,
      plano_id: atual.plano_id,
      status: atual.status
    });
    this.formulario.controls.cpf.disable();
  }

  protected enviar(): void {
    this.formulario.markAllAsTouched();

    if (this.formulario.invalid) {
      this.erro.set('Corrija os campos destacados antes de enviar.');
      return;
    }

    this.enviando.set(true);
    this.erro.set(null);

    const valores = this.formulario.getRawValue();
    const requisicao$ = this.ehEdicao()
      ? this.atualizarBeneficiario.executarAsync(this.beneficiario()!.id, {
          nome_completo: valores.nome_completo.trim(),
          data_nascimento: valores.data_nascimento,
          plano_id: valores.plano_id,
          status: valores.status
        } satisfies AtualizarBeneficiarioRequest)
      : this.criarBeneficiario.executarAsync({
          nome_completo: valores.nome_completo.trim(),
          cpf: apenasDigitos(valores.cpf),
          data_nascimento: valores.data_nascimento,
          plano_id: valores.plano_id
        } satisfies CriarBeneficiarioRequest);

    requisicao$.pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
      next: () => {
        this.enviando.set(false);
        this.salvo.emit();
      },
      error: (resposta: HttpErrorResponse) => {
        this.enviando.set(false);
        this.erro.set(mensagemDeErro(resposta));
      }
    });
  }
}
