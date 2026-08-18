export function apenasDigitos(valor: string): string {
  return (valor ?? '').replace(/\D/g, '');
}

export function cpfEhValido(cpf: string): boolean {
  const numeros = apenasDigitos(cpf);

  if (numeros.length !== 11) {
    return false;
  }

  if (/^(\d)\1{10}$/.test(numeros)) {
    return false;
  }

  const digitos = numeros.split('').map((d) => Number(d));
  const primeiro = calcularDigito(digitos, 9);
  const segundo = calcularDigito([...digitos.slice(0, 9), primeiro], 10);

  return digitos[9] === primeiro && digitos[10] === segundo;
}

export function formatarCpf(cpf: string): string {
  const numeros = apenasDigitos(cpf).padEnd(11, '').slice(0, 11);

  if (numeros.length !== 11) {
    return cpf;
  }

  return `${numeros.slice(0, 3)}.${numeros.slice(3, 6)}.${numeros.slice(6, 9)}-${numeros.slice(9)}`;
}

function calcularDigito(digitos: number[], quantidade: number): number {
  let soma = 0;

  for (let i = 0; i < quantidade; i++) {
    soma += digitos[i] * (quantidade + 1 - i);
  }

  const resto = soma % 11;
  return resto < 2 ? 0 : 11 - resto;
}
