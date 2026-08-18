using Desafio.Domain.Compartilhado.Validadores;

namespace Desafio.Api.Tests;

public sealed class ValidadorCpfTests
{
    [Theory]
    [InlineData("52998224725")]
    [InlineData("71428793860")]
    [InlineData("39053344705")]
    public void Cpf_valido_deve_ser_aceito(string cpf)
    {
        Assert.True(ValidadorCpf.EhValido(cpf));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("123")]
    [InlineData("5299822472")]
    [InlineData("529982247256")]
    [InlineData("529.982.247-25")]
    [InlineData("5299822472A")]
    [InlineData("52998224726")]
    [InlineData("00000000000")]
    [InlineData("11111111111")]
    public void Cpf_invalido_deve_ser_recusado(string? cpf)
    {
        Assert.False(ValidadorCpf.EhValido(cpf));
    }
}
