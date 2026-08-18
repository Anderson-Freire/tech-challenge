using System.Text.RegularExpressions;
using Desafio.Api.Kernel.Dominio.Excecoes;

namespace Desafio.Api.Kernel.Dominio.VO;

public sealed partial class CodigoRegistroAns : IEquatable<CodigoRegistroAns>
{
    public string Valor { get; }

    public CodigoRegistroAns(string? valor)
    {
        valor = valor?.Trim() ?? string.Empty;

        var detalhes = new List<DetalheErro>();

        if (valor.Length == 0)
        {
            detalhes.Add(new DetalheErro("codigo_registro_ans", "obrigatorio"));
        }
        else if (!FormatoDoCodigoAns().IsMatch(valor))
        {
            detalhes.Add(new DetalheErro("codigo_registro_ans", "formato_invalido"));
        }

        if (detalhes.Count > 0)
        {
            throw new ValidacaoExcecao("Código de registro ANS inválido", detalhes);
        }

        Valor = valor;
    }

    public bool Equals(CodigoRegistroAns? other) =>
        other is not null && Valor == other.Valor;

    public override bool Equals(object? obj) =>
        Equals(obj as CodigoRegistroAns);

    public override int GetHashCode() => Valor.GetHashCode();

    public override string ToString() => Valor;

    public static implicit operator string(CodigoRegistroAns codigo) => codigo.Valor;

    [GeneratedRegex("^[0-9]{6}$")]
    private static partial Regex FormatoDoCodigoAns();
}