using Desafio.Domain.Compartilhado.Excecoes;
using Desafio.Domain.Compartilhado.Validadores;

namespace Desafio.Domain.Compartilhado.ObjetosDeValor;

public sealed class Cpf(string? valor) : IEquatable<Cpf>
{
    public string Valor { get; } = Validar(valor);

    public string Formatado() =>
        $"{Valor[..3]}.{Valor[3..6]}.{Valor[6..9]}-{Valor[9..]}";

    public bool Equals(Cpf? other) =>
        other is not null && Valor == other.Valor;

    public override bool Equals(object? obj) =>
        Equals(obj as Cpf);

    public override int GetHashCode() =>
        StringComparer.Ordinal.GetHashCode(Valor);

    public override string ToString() => Valor;

    public static bool operator ==(Cpf? esquerda, Cpf? direita) =>
        Equals(esquerda, direita);

    public static bool operator !=(Cpf? esquerda, Cpf? direita) =>
        !Equals(esquerda, direita);

    public static implicit operator string(Cpf cpf) => cpf.Valor;

    private static string Validar(string? valor)
    {
        var numeros = (valor ?? string.Empty).Trim();

        if (!ValidadorCpf.EhValido(numeros))
        {
            throw new ValidacaoExcecao(
                "CPF inválido",
                [new DetalheErro(nameof(Cpf), "formato_invalido")]);
        }

        return numeros;
    }
}
