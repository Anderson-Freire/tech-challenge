export type StatusBeneficiario = 'ATIVO' | 'INATIVO';

export interface Beneficiario {
  id: string;
  nome_completo: string;
  cpf: string;
  data_nascimento: string;
  status: StatusBeneficiario;
  plano_id: string;
  data_cadastro: string;
}

export type CriarBeneficiarioResponse = Beneficiario;
export type AtualizarBeneficiarioResponse = Beneficiario;

export interface ListarBeneficiariosResponse {
  dados: Beneficiario[];
  pagina: number;
  tamanho: number;
  total: number;
}

export interface CriarBeneficiarioRequest {
  nome_completo: string;
  cpf: string;
  data_nascimento: string;
  plano_id: string;
}

export interface AtualizarBeneficiarioRequest {
  nome_completo: string;
  data_nascimento: string;
  plano_id: string;
  status: StatusBeneficiario;
}

export interface ListarBeneficiariosRequest {
  pagina: number;
  tamanho: number;
  status: StatusBeneficiario | '';
  plano_id: string;
}
