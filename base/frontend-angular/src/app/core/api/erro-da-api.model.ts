export interface ErroDaApi {
  erro: string;
  mensagem: string;
  detalhes: { campo: string; regra: string }[];
}
