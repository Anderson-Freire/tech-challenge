namespace Desafio.Domain.Compartilhado.Validadores;

public static class ValidadorCpf
{
    private const int TamanhoCpf = 11;

    public static bool EhValido(string? cpf)
    {
        if (string.IsNullOrWhiteSpace(cpf))
        {
            return false;
        }

        if (cpf.Length != TamanhoCpf)
        {
            return false;
        }

        Span<int> digitos = stackalloc int[TamanhoCpf];

        for (var i = 0; i < TamanhoCpf; i++)
        {
            var caractere = cpf[i];

            if (!char.IsDigit(caractere))
            {
                return false;
            }

            digitos[i] = caractere - '0';
        }

        if (TodosDigitosIguais(digitos))
        {
            return false;
        }

        var primeiroDigitoVerificador = CalcularDigitoVerificador(digitos, 9);
        var segundoDigitoVerificador = CalcularDigitoVerificador(digitos, 10);

        return digitos[9] == primeiroDigitoVerificador &&
               digitos[10] == segundoDigitoVerificador;
    }

    private static bool TodosDigitosIguais(Span<int> digitos)
    {
        for (var i = 1; i < digitos.Length; i++)
        {
            if (digitos[i] != digitos[0])
            {
                return false;
            }
        }

        return true;
    }

    private static int CalcularDigitoVerificador(Span<int> digitos, int quantidade)
    {
        var soma = 0;

        for (var i = 0; i < quantidade; i++)
        {
            soma += digitos[i] * (quantidade + 1 - i);
        }

        var resto = soma % 11;

        return resto < 2 ? 0 : 11 - resto;
    }
}