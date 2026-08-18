import { AbstractControl, ValidationErrors } from '@angular/forms';

import { cpfEhValido } from '../utils/cpf.util';

export function validadorDeCpf(controle: AbstractControl): ValidationErrors | null {
  const valor = String(controle.value ?? '');

  if (!valor) {
    return null;
  }

  return cpfEhValido(valor) ? null : { cpfInvalido: true };
}

export function validadorDataPassada(controle: AbstractControl): ValidationErrors | null {
  const valor = String(controle.value ?? '');

  if (!valor) {
    return null;
  }

  return valor < new Date().toISOString().slice(0, 10) ? null : { dataFutura: true };
}
