using System.Net;
using Microsoft.EntityFrameworkCore;
namespace Desafio.Api.Tests;

[Collection(ColecaoDaApi.Nome)]
public class BeneficiariosTests(ApiFixture fixture) : IAsyncLifetime
{
    private HttpClient Client => fixture.Client;

    public Task InitializeAsync() => fixture.LimparAsync();

    public Task DisposeAsync() => Task.CompletedTask;

    private static object CorpoDeCriacao(
        string cpf,
        Guid? planoId = null,
        string nome = "Maria Aparecida da Silva",
        string dataNascimento = "1990-05-12") => new
        {
            NomeCompleto = nome,
            Cpf = cpf,
            DataNascimento = dataNascimento,
            PlanoId = planoId ?? Planos.Bronze
        };

    // ------------------------------------------------------------------
    // CRIAÇÃO
    // ------------------------------------------------------------------

    [Fact]
    public async Task Criar_deve_devolver_201_com_header_location()
    {
        var resposta = await Client.PostAsync(
            "/beneficiarios",
            Http.Json(CorpoDeCriacao("52998224725")));

        Assert.Equal(HttpStatusCode.Created, resposta.StatusCode);
        Assert.NotNull(resposta.Headers.Location);

        var corpo = await resposta.CorpoAsync();

        Assert.NotEqual(
            Guid.Empty,
            corpo.GetProperty("id").GetGuid());

        Assert.Equal(
            "52998224725",
            corpo.GetProperty("cpf").GetString());

        Assert.Equal(
            "ATIVO",
            corpo.GetProperty("status").GetString());

        Assert.False(
            corpo.GetProperty("data_cadastro").GetDateTime() == default);
    }

    [Fact]
    public async Task Criar_deve_ignorar_id_status_e_data_cadastro_enviados_pelo_cliente()
    {
        var idEnviado = Guid.NewGuid();

        var resposta = await Client.PostAsync(
            "/beneficiarios",
            Http.Json(new
            {
                Id = idEnviado,
                NomeCompleto = "Maria Aparecida da Silva",
                Cpf = "52998224725",
                DataNascimento = "1990-05-12",
                Status = "INATIVO",
                PlanoId = Planos.Bronze,
                DataCadastro = "2000-01-01T00:00:00Z"
            }));

        Assert.Equal(HttpStatusCode.Created, resposta.StatusCode);

        var corpo = await resposta.CorpoAsync();

        Assert.NotEqual(
            idEnviado,
            corpo.GetProperty("id").GetGuid());

        Assert.Equal(
            "ATIVO",
            corpo.GetProperty("status").GetString());

        Assert.NotEqual(
            DateTime.Parse("2000-01-01T00:00:00Z"),
            corpo.GetProperty("data_cadastro").GetDateTime());
    }

    [Fact]
    public async Task Criar_com_cpf_ja_cadastrado_deve_devolver_409()
    {
        await Client.PostAsync(
            "/beneficiarios",
            Http.Json(CorpoDeCriacao("71428793860")));

        var resposta = await Client.PostAsync(
            "/beneficiarios",
            Http.Json(CorpoDeCriacao("71428793860")));

        Assert.Equal(
            HttpStatusCode.Conflict,
            resposta.StatusCode);
    }

    [Fact]
    public async Task Criar_com_cpf_de_beneficiario_excluido_deve_devolver_409()
    {
        var beneficiario =
            (await fixture.SemearBeneficiariosAsync(1)).Single();

        var exclusao = await Client.DeleteAsync(
            $"/beneficiarios/{beneficiario.Id}");

        Assert.Equal(
            HttpStatusCode.NoContent,
            exclusao.StatusCode);

        var resposta = await Client.PostAsync(
            "/beneficiarios",
            Http.Json(CorpoDeCriacao(beneficiario.Cpf)));

        Assert.Equal(
            HttpStatusCode.Conflict,
            resposta.StatusCode);
    }

    [Theory]
    [InlineData("123")]
    [InlineData("5299822472")]
    [InlineData("529982247256")]
    [InlineData("529.982.247-25")]
    [InlineData("5299822472A")]
    public async Task Criar_com_cpf_fora_do_formato_deve_devolver_400(
        string cpf)
    {
        var resposta = await Client.PostAsync(
            "/beneficiarios",
            Http.Json(CorpoDeCriacao(cpf)));

        Assert.Equal(
            HttpStatusCode.BadRequest,
            resposta.StatusCode);
    }

    [Theory]
    [InlineData("00000000000")]
    [InlineData("11111111111")]
    [InlineData("22222222222")]
    [InlineData("33333333333")]
    [InlineData("44444444444")]
    [InlineData("55555555555")]
    [InlineData("66666666666")]
    [InlineData("77777777777")]
    [InlineData("88888888888")]
    [InlineData("99999999999")]
    public async Task Criar_com_cpf_de_digitos_repetidos_deve_devolver_400(
        string cpf)
    {
        var resposta = await Client.PostAsync(
            "/beneficiarios",
            Http.Json(CorpoDeCriacao(cpf)));

        Assert.Equal(
            HttpStatusCode.BadRequest,
            resposta.StatusCode);
    }

    [Fact]
    public async Task Criar_com_cpf_com_digitos_verificadores_invalidos_deve_devolver_400()
    {
        var resposta = await Client.PostAsync(
            "/beneficiarios",
            Http.Json(CorpoDeCriacao("52998224726")));

        Assert.Equal(
            HttpStatusCode.BadRequest,
            resposta.StatusCode);
    }

    [Fact]
    public async Task Criar_com_data_de_nascimento_no_futuro_deve_devolver_400()
    {
        var resposta = await Client.PostAsync(
            "/beneficiarios",
            Http.Json(
                CorpoDeCriacao(
                    "39053344705",
                    dataNascimento: "2999-01-01")));

        Assert.Equal(
            HttpStatusCode.BadRequest,
            resposta.StatusCode);
    }

    [Fact]
    public async Task Criar_com_plano_inexistente_deve_devolver_422()
    {
        var resposta = await Client.PostAsync(
            "/beneficiarios",
            Http.Json(
                CorpoDeCriacao(
                    "39053344705",
                    Planos.Inexistente)));

        Assert.Equal(
            HttpStatusCode.UnprocessableEntity,
            resposta.StatusCode);
    }

    [Fact]
    public async Task Criar_com_plano_excluido_deve_devolver_422()
    {
        await fixture.UsarBancoAsync(async db =>
        {
            await db.Database.ExecuteSqlInterpolatedAsync($"""
                UPDATE "Planos"
                SET "ExcluidoEm" = {DateTime.UtcNow}
                WHERE "Id" = {Planos.Bronze}
                """);
        });

        var resposta = await Client.PostAsync(
            "/beneficiarios",
            Http.Json(
                CorpoDeCriacao(
                    "39053344705",
                    Planos.Bronze)));

        Assert.Equal(
            HttpStatusCode.UnprocessableEntity,
            resposta.StatusCode);
    }

    // ------------------------------------------------------------------
    // CONSULTA POR ID
    // ------------------------------------------------------------------

    [Fact]
    public async Task Obter_deve_devolver_o_beneficiario()
    {
        var beneficiario =
            (await fixture.SemearBeneficiariosAsync(1)).Single();

        var resposta = await Client.GetAsync(
            $"/beneficiarios/{beneficiario.Id}");

        Assert.Equal(
            HttpStatusCode.OK,
            resposta.StatusCode);

        var corpo = await resposta.CorpoAsync();

        Assert.Equal(
            beneficiario.Id,
            corpo.GetProperty("id").GetGuid());

        Assert.Equal(
            beneficiario.Cpf,
            corpo.GetProperty("cpf").GetString());

        Assert.Equal(
            Planos.Bronze,
            corpo.GetProperty("plano_id").GetGuid());
    }

    [Fact]
    public async Task Obter_inexistente_deve_devolver_404()
    {
        var resposta = await Client.GetAsync(
            $"/beneficiarios/{Guid.NewGuid()}");

        Assert.Equal(
            HttpStatusCode.NotFound,
            resposta.StatusCode);
    }

    [Fact]
    public async Task Obter_beneficiario_excluido_deve_devolver_404()
    {
        var beneficiario =
            (await fixture.SemearBeneficiariosAsync(1)).Single();

        var exclusao = await Client.DeleteAsync(
            $"/beneficiarios/{beneficiario.Id}");

        Assert.Equal(
            HttpStatusCode.NoContent,
            exclusao.StatusCode);

        var resposta = await Client.GetAsync(
            $"/beneficiarios/{beneficiario.Id}");

        Assert.Equal(
            HttpStatusCode.NotFound,
            resposta.StatusCode);
    }

    // ------------------------------------------------------------------
    // ATUALIZAÇÃO
    // ------------------------------------------------------------------

    [Fact]
    public async Task Atualizar_deve_alterar_os_dados_do_beneficiario()
    {
        var beneficiario =
            (await fixture.SemearBeneficiariosAsync(1)).Single();

        var resposta = await Client.PutAsync(
            $"/beneficiarios/{beneficiario.Id}",
            Http.Json(new
            {
                NomeCompleto = "Joana Ribeiro Nunes",
                DataNascimento = "1985-03-20",
                PlanoId = Planos.Ouro,
                Status = "ATIVO"
            }));

        Assert.Equal(
            HttpStatusCode.OK,
            resposta.StatusCode);

        var corpo = await resposta.CorpoAsync();

        Assert.Equal(
            "Joana Ribeiro Nunes",
            corpo.GetProperty("nome_completo").GetString());

        Assert.Equal(
            Planos.Ouro,
            corpo.GetProperty("plano_id").GetGuid());

        Assert.Equal(
            beneficiario.Cpf,
            corpo.GetProperty("cpf").GetString());
    }

    [Fact]
    public async Task Atualizar_nao_deve_permitir_alteracao_do_cpf()
    {
        var beneficiario =
            (await fixture.SemearBeneficiariosAsync(1)).Single();

        var cpfOriginal = beneficiario.Cpf;

        var resposta = await Client.PutAsync(
            $"/beneficiarios/{beneficiario.Id}",
            Http.Json(new
            {
                NomeCompleto = "Joana Ribeiro Nunes",
                Cpf = "71428793860",
                DataNascimento = "1985-03-20",
                PlanoId = Planos.Ouro,
                Status = "ATIVO"
            }));

        Assert.Equal(
            HttpStatusCode.OK,
            resposta.StatusCode);

        var corpo = await resposta.CorpoAsync();

        Assert.Equal(
            cpfOriginal,
            corpo.GetProperty("cpf").GetString());
    }

    [Fact]
    public async Task Atualizar_nao_deve_permitir_alteracao_de_cpf_para_outro_cpf()
    {
        await fixture.SemearBeneficiariosAsync(
            1,
            Planos.Bronze,
            "ATIVO",
            1);

        var beneficiario =
            (await fixture.SemearBeneficiariosAsync(
                1,
                Planos.Bronze,
                "ATIVO",
                100)).Single();

        var outroCpf = GeradorDeCpf.Gerar(1);

        var resposta = await Client.PutAsync(
            $"/beneficiarios/{beneficiario.Id}",
            Http.Json(new
            {
                NomeCompleto = "Joana Ribeiro Nunes",
                Cpf = outroCpf,
                DataNascimento = "1985-03-20",
                PlanoId = Planos.Ouro,
                Status = "ATIVO"
            }));

        Assert.Equal(
            HttpStatusCode.OK,
            resposta.StatusCode);

        var corpo = await resposta.CorpoAsync();

        Assert.Equal(
            beneficiario.Cpf,
            corpo.GetProperty("cpf").GetString());
    }

    [Fact]
    public async Task Atualizar_inexistente_deve_devolver_404()
    {
        var resposta = await Client.PutAsync(
            $"/beneficiarios/{Guid.NewGuid()}",
            Http.Json(new
            {
                NomeCompleto = "Nao Existe",
                DataNascimento = "1985-03-20",
                PlanoId = Planos.Ouro,
                Status = "ATIVO"
            }));

        Assert.Equal(
            HttpStatusCode.NotFound,
            resposta.StatusCode);
    }

    [Fact]
    public async Task Atualizar_apontando_para_plano_inexistente_deve_devolver_422()
    {
        var beneficiario =
            (await fixture.SemearBeneficiariosAsync(1)).Single();

        var resposta = await Client.PutAsync(
            $"/beneficiarios/{beneficiario.Id}",
            Http.Json(new
            {
                NomeCompleto = "Maria Aparecida da Silva",
                DataNascimento = "1990-05-12",
                PlanoId = Planos.Inexistente,
                Status = "ATIVO"
            }));

        Assert.Equal(
            HttpStatusCode.UnprocessableEntity,
            resposta.StatusCode);
    }

    [Fact]
    public async Task Atualizar_apontando_para_plano_excluido_deve_devolver_422()
    {
        var beneficiario =
            (await fixture.SemearBeneficiariosAsync(1)).Single();

        await fixture.UsarBancoAsync(async db =>
        {
            await db.Database.ExecuteSqlInterpolatedAsync($"""
                UPDATE "Planos"
                SET "ExcluidoEm" = {DateTime.UtcNow}
                WHERE "Id" = {Planos.Ouro}
                """);
        });

        var resposta = await Client.PutAsync(
            $"/beneficiarios/{beneficiario.Id}",
            Http.Json(new
            {
                NomeCompleto = "Joana Ribeiro Nunes",
                DataNascimento = "1985-03-20",
                PlanoId = Planos.Ouro,
                Status = "ATIVO"
            }));

        Assert.Equal(
            HttpStatusCode.UnprocessableEntity,
            resposta.StatusCode);
    }

    [Fact]
    public async Task Atualizar_data_de_nascimento_no_futuro_deve_devolver_400()
    {
        var beneficiario =
            (await fixture.SemearBeneficiariosAsync(1)).Single();

        var resposta = await Client.PutAsync(
            $"/beneficiarios/{beneficiario.Id}",
            Http.Json(new
            {
                NomeCompleto = "Joana Ribeiro Nunes",
                DataNascimento = "2999-01-01",
                PlanoId = Planos.Ouro,
                Status = "ATIVO"
            }));

        Assert.Equal(
            HttpStatusCode.BadRequest,
            resposta.StatusCode);
    }

    [Fact]
    public async Task Atualizar_dados_de_beneficiario_inativo_deve_devolver_409()
    {
        var beneficiario =
            (await fixture.SemearBeneficiariosAsync(
                1,
                Planos.Bronze,
                "INATIVO",
                500)).Single();

        var resposta = await Client.PutAsync(
            $"/beneficiarios/{beneficiario.Id}",
            Http.Json(new
            {
                NomeCompleto = "Nome Corrigido do Inativo",
                DataNascimento = "1990-05-12",
                PlanoId = Planos.Bronze,
                Status = "INATIVO"
            }));

        Assert.Equal(
            HttpStatusCode.Conflict,
            resposta.StatusCode);
    }

    [Fact]
    public async Task Reativar_beneficiario_inativo_deve_devolver_200()
    {
        var beneficiario =
            (await fixture.SemearBeneficiariosAsync(
                1,
                Planos.Bronze,
                "INATIVO",
                600)).Single();

        var resposta = await Client.PutAsync(
            $"/beneficiarios/{beneficiario.Id}",
            Http.Json(new
            {
                NomeCompleto = beneficiario.NomeCompleto,
                DataNascimento = beneficiario.DataNascimento,
                PlanoId = beneficiario.PlanoId,
                Status = "ATIVO"
            }));

        Assert.Equal(
            HttpStatusCode.OK,
            resposta.StatusCode);

        var corpo = await resposta.CorpoAsync();

        Assert.Equal(
            "ATIVO",
            corpo.GetProperty("status").GetString());
    }

    // ------------------------------------------------------------------
    // EXCLUSÃO
    // ------------------------------------------------------------------

    [Fact]
    public async Task Excluir_deve_ser_logico_e_tirar_o_beneficiario_das_consultas()
    {
        var beneficiario =
            (await fixture.SemearBeneficiariosAsync(1)).Single();

        var exclusao = await Client.DeleteAsync(
            $"/beneficiarios/{beneficiario.Id}");

        Assert.Equal(
            HttpStatusCode.NoContent,
            exclusao.StatusCode);

        var consulta = await Client.GetAsync(
            $"/beneficiarios/{beneficiario.Id}");

        Assert.Equal(
            HttpStatusCode.NotFound,
            consulta.StatusCode);

        var listagem = await (
            await Client.GetAsync(
                "/beneficiarios?pagina=1&tamanho=50"))
            .CorpoAsync();

        Assert.Equal(
            0,
            listagem.GetProperty("total").GetInt32());

        var novaExclusao = await Client.DeleteAsync(
            $"/beneficiarios/{beneficiario.Id}");

        Assert.Equal(
            HttpStatusCode.NotFound,
            novaExclusao.StatusCode);
    }

    [Fact]
    public async Task Excluir_beneficiario_inexistente_deve_devolver_404()
    {
        var resposta = await Client.DeleteAsync(
            $"/beneficiarios/{Guid.NewGuid()}");

        Assert.Equal(
            HttpStatusCode.NotFound,
            resposta.StatusCode);
    }

    [Fact]
    public async Task Cpf_de_beneficiario_excluido_deve_continuar_ocupado()
    {
        var beneficiario =
            (await fixture.SemearBeneficiariosAsync(1)).Single();

        await Client.DeleteAsync(
            $"/beneficiarios/{beneficiario.Id}");

        var resposta = await Client.PostAsync(
            "/beneficiarios",
            Http.Json(
                CorpoDeCriacao(beneficiario.Cpf)));

        Assert.Equal(
            HttpStatusCode.Conflict,
            resposta.StatusCode);
    }

    [Fact]
    public async Task Atualizar_beneficiario_excluido_deve_devolver_404()
    {
        var beneficiario =
            (await fixture.SemearBeneficiariosAsync(1)).Single();

        await Client.DeleteAsync(
            $"/beneficiarios/{beneficiario.Id}");

        var resposta = await Client.PutAsync(
            $"/beneficiarios/{beneficiario.Id}",
            Http.Json(new
            {
                NomeCompleto = "Joana Ribeiro Nunes",
                DataNascimento = "1985-03-20",
                PlanoId = Planos.Ouro,
                Status = "ATIVO"
            }));

        Assert.Equal(
            HttpStatusCode.NotFound,
            resposta.StatusCode);
    }

    // ------------------------------------------------------------------
    // LISTAGEM
    // ------------------------------------------------------------------

    [Fact]
    public async Task Listar_deve_devolver_envelope_paginado()
    {
        await fixture.SemearBeneficiariosAsync(3);

        var resposta = await Client.GetAsync(
            "/beneficiarios?pagina=1&tamanho=10");

        Assert.Equal(
            HttpStatusCode.OK,
            resposta.StatusCode);

        var corpo = await resposta.CorpoAsync();

        Assert.Equal(
            3,
            corpo.GetProperty("dados").GetArrayLength());

        Assert.Equal(
            1,
            corpo.GetProperty("pagina").GetInt32());

        Assert.Equal(
            10,
            corpo.GetProperty("tamanho").GetInt32());

        Assert.Equal(
            3,
            corpo.GetProperty("total").GetInt32());
    }

    [Fact]
    public async Task Listar_deve_respeitar_pagina_e_tamanho()
    {
        await fixture.SemearBeneficiariosAsync(25);

        var corpo = await (
            await Client.GetAsync(
                "/beneficiarios?pagina=3&tamanho=10"))
            .CorpoAsync();

        Assert.Equal(
            5,
            corpo.GetProperty("dados").GetArrayLength());

        Assert.Equal(
            3,
            corpo.GetProperty("pagina").GetInt32());

        Assert.Equal(
            25,
            corpo.GetProperty("total").GetInt32());
    }

    [Fact]
    public async Task Listar_pagina_alem_do_total_deve_devolver_200_com_lista_vazia()
    {
        await fixture.SemearBeneficiariosAsync(3);

        var resposta = await Client.GetAsync(
            "/beneficiarios?pagina=2&tamanho=10");

        Assert.Equal(
            HttpStatusCode.OK,
            resposta.StatusCode);

        var corpo = await resposta.CorpoAsync();

        Assert.Equal(
            0,
            corpo.GetProperty("dados").GetArrayLength());

        Assert.Equal(
            3,
            corpo.GetProperty("total").GetInt32());
    }

    [Fact]
    public async Task Listar_deve_combinar_os_filtros_de_status_e_plano()
    {
        await fixture.SemearBeneficiariosAsync(
            4,
            Planos.Bronze,
            "ATIVO",
            100);

        await fixture.SemearBeneficiariosAsync(
            6,
            Planos.Bronze,
            "INATIVO",
            200);

        await fixture.SemearBeneficiariosAsync(
            3,
            Planos.Prata,
            "ATIVO",
            300);

        var corpo = await (
            await Client.GetAsync(
                $"/beneficiarios?tamanho=50&status=ATIVO&plano_id={Planos.Bronze}"))
            .CorpoAsync();

        Assert.Equal(
            4,
            corpo.GetProperty("total").GetInt32());

        Assert.All(
            corpo.GetProperty("dados").EnumerateArray(),
            beneficiario =>
            {
                Assert.Equal(
                    "ATIVO",
                    beneficiario.GetProperty("status").GetString());

                Assert.Equal(
                    Planos.Bronze,
                    beneficiario.GetProperty("plano_id").GetGuid());
            });
    }

    [Fact]
    public async Task Listar_por_status_deve_filtrar_corretamente()
    {
        await fixture.SemearBeneficiariosAsync(
            3,
            Planos.Bronze,
            "ATIVO",
            700);

        await fixture.SemearBeneficiariosAsync(
            2,
            Planos.Bronze,
            "INATIVO",
            800);

        var corpo = await (
            await Client.GetAsync(
                "/beneficiarios?status=INATIVO&tamanho=50"))
            .CorpoAsync();

        Assert.Equal(
            2,
            corpo.GetProperty("total").GetInt32());

        Assert.All(
            corpo.GetProperty("dados").EnumerateArray(),
            beneficiario =>
                Assert.Equal(
                    "INATIVO",
                    beneficiario.GetProperty("status").GetString()));
    }

    [Fact]
    public async Task Listar_por_plano_deve_filtrar_corretamente()
    {
        await fixture.SemearBeneficiariosAsync(
            3,
            Planos.Bronze,
            "ATIVO",
            900);

        await fixture.SemearBeneficiariosAsync(
            2,
            Planos.Prata,
            "ATIVO",
            1000);

        var corpo = await (
            await Client.GetAsync(
                $"/beneficiarios?plano_id={Planos.Prata}&tamanho=50"))
            .CorpoAsync();

        Assert.Equal(
            2,
            corpo.GetProperty("total").GetInt32());

        Assert.All(
            corpo.GetProperty("dados").EnumerateArray(),
            beneficiario =>
                Assert.Equal(
                    Planos.Prata,
                    beneficiario.GetProperty("plano_id").GetGuid()));
    }

    [Fact]
    public async Task Listar_nao_deve_contabilizar_beneficiario_excluido()
    {
        var beneficiarios =
            await fixture.SemearBeneficiariosAsync(3);

        await Client.DeleteAsync(
            $"/beneficiarios/{beneficiarios[0].Id}");

        var corpo = await (
            await Client.GetAsync(
                "/beneficiarios?tamanho=50"))
            .CorpoAsync();

        Assert.Equal(
            2,
            corpo.GetProperty("total").GetInt32());

        Assert.Equal(
            2,
            corpo.GetProperty("dados").GetArrayLength());
    }

    [Fact]
    public async Task Listar_sem_informar_tamanho_deve_devolver_20_itens_por_pagina()
    {
        await fixture.SemearBeneficiariosAsync(25);

        var corpo = await (
            await Client.GetAsync("/beneficiarios"))
            .CorpoAsync();

        Assert.Equal(
            20,
            corpo.GetProperty("dados").GetArrayLength());

        Assert.Equal(
            20,
            corpo.GetProperty("tamanho").GetInt32());

        Assert.Equal(
            25,
            corpo.GetProperty("total").GetInt32());
    }

    [Fact]
    public async Task Listar_com_tamanho_maior_que_100_deve_devolver_400()
    {
        var resposta = await Client.GetAsync(
            "/beneficiarios?tamanho=101");

        Assert.Equal(
            HttpStatusCode.BadRequest,
            resposta.StatusCode);
    }

    [Fact]
    public async Task Listar_com_tamanho_zero_deve_devolver_400()
    {
        var resposta = await Client.GetAsync(
            "/beneficiarios?tamanho=0");

        Assert.Equal(
            HttpStatusCode.BadRequest,
            resposta.StatusCode);
    }

    [Fact]
    public async Task Listar_com_pagina_zero_deve_devolver_400()
    {
        var resposta = await Client.GetAsync(
            "/beneficiarios?pagina=0");

        Assert.Equal(
            HttpStatusCode.BadRequest,
            resposta.StatusCode);
    }

    [Fact]
    public async Task Listar_com_pagina_negativa_deve_devolver_400()
    {
        var resposta = await Client.GetAsync(
            "/beneficiarios?pagina=-1");

        Assert.Equal(
            HttpStatusCode.BadRequest,
            resposta.StatusCode);
    }

    [Fact]
    public async Task Listar_com_status_invalido_deve_devolver_400()
    {
        var resposta = await Client.GetAsync(
            "/beneficiarios?status=QUALQUER");

        Assert.Equal(
            HttpStatusCode.BadRequest,
            resposta.StatusCode);
    }

    // ------------------------------------------------------------------
    // SITUAÇÃO
    // ------------------------------------------------------------------

    [Fact]
    public async Task Beneficiario_inativo_deve_continuar_aparecendo_na_listagem()
    {
        await fixture.SemearBeneficiariosAsync(
            1,
            Planos.Bronze,
            "INATIVO",
            1100);

        var corpo = await (
            await Client.GetAsync(
                "/beneficiarios?tamanho=50"))
            .CorpoAsync();

        Assert.Equal(
            1,
            corpo.GetProperty("total").GetInt32());

        Assert.Equal(
            "INATIVO",
            corpo.GetProperty("dados")[0]
                .GetProperty("status")
                .GetString());
    }

    [Fact]
    public async Task Beneficiario_inativo_deve_continuar_acessivel_por_id()
    {
        var beneficiario =
            (await fixture.SemearBeneficiariosAsync(
                1,
                Planos.Bronze,
                "INATIVO",
                1200)).Single();

        var resposta = await Client.GetAsync(
            $"/beneficiarios/{beneficiario.Id}");

        Assert.Equal(
            HttpStatusCode.OK,
            resposta.StatusCode);

        var corpo = await resposta.CorpoAsync();

        Assert.Equal(
            "INATIVO",
            corpo.GetProperty("status").GetString());
    }
}