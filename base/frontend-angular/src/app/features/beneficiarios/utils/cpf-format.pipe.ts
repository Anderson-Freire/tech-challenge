import { Pipe, PipeTransform } from '@angular/core';

import { formatarCpf } from './cpf.util';

@Pipe({
  name: 'cpfFormat'
})
export class CpfFormatPipe implements PipeTransform {
  transform(cpf: string): string {
    return formatarCpf(cpf);
  }
}
