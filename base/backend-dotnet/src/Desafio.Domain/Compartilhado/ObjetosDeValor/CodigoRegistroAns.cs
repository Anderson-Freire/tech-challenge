using System.Text.RegularExpressions;
using Desafio.Domain.Compartilhado.Excecoes;

namespace Desafio.Domain.Compartilhado.ObjetosDeValor;

public sealed partial class CodigoRegistroAns(string? valor) : IEquatable<CodigoRegistroAns>
{
    public string Valor { get; } = Validar(valor);

    public bool Equals(CodigoRegistroAns? other) =>
        other is not null && Valor == other.Valor;

    public override bool Equals(object? obj) =>
        Equals(obj as CodigoRegistroAns);

    public override int GetHashCode() =>
        StringComparer.Ordinal.GetHashCode(Valor);

    public override string ToString() => Valor;

    public static bool operator ==(CodigoRegistroAns? esquerda, CodigoRegistroAns? direita) =>
        Equals(esquerda, direita);

    public static bool operator !=(CodigoRegistroAns? esquerda, CodigoRegistroAns? direita) =>
        !Equals(esquerda, direita);

    public static implicit operator string(CodigoRegistroAns codigo) => codigo.Valor;

    [GeneratedRegex("^[0-9]{6}$")]
    private static partial Regex FormatoDoCodigoAns();

    private static string Validar(string? valor)
    {
        valor = valor?.Trim() ?? string.Empty;

        var detalhes = new List<DetalheErro>();

        if (valor.Length == 0)
        {
            detalhes.Add(new DetalheErro(nameof(CodigoRegistroAns), "obrigatorio"));
        }
        else if (!FormatoDoCodigoAns().IsMatch(valor))
        {
            detalhes.Add(new DetalheErro(nameof(CodigoRegistroAns), "formato_invalido"));
        }

        if (detalhes.Count > 0)
        {
            throw new ValidacaoExcecao("Código de registro ANS inválido", detalhes);
        }

        return valor;
    }
}
