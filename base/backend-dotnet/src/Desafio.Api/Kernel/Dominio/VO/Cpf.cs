using Desafio.Api.Kernel.Dominio.Excecoes;
using Desafio.Api.Kernel.Dominio.Validadores;

namespace Desafio.Api.Kernel.Dominio.VO;

public sealed class Cpf : IEquatable<Cpf>
{
    public string Valor { get; }

    public Cpf(string? valor)
    {
        var numeros = LimparFormatacao(valor);

        if (!ValidadorCpf.EhValido(numeros))
        {
            throw new ValidacaoExcecao(
                "CPF inválido",
                [new DetalheErro("cpf", "formato_invalido")]);
        }

        Valor = numeros;
    }

    private static string LimparFormatacao(string? valor) =>
        new([.. (valor ?? string.Empty).Where(char.IsDigit)]);

    public string Formatado() =>
        $"{Valor[..3]}.{Valor[3..6]}.{Valor[6..9]}-{Valor[9..]}";

    public bool Equals(Cpf? other) =>
        other is not null && Valor == other.Valor;

    public override bool Equals(object? obj) =>
        Equals(obj as Cpf);

    public override int GetHashCode() => Valor.GetHashCode();

    public override string ToString() => Valor;

    public static implicit operator string(Cpf cpf) => cpf.Valor;
}