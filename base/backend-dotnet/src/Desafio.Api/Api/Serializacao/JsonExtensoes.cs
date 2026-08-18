using System.Text.Json;
using System.Text.Json.Serialization;

namespace Desafio.Api.Api.Serializacao;

public enum EstrategiaJson
{
    Padrao = 0,
    CamelCase = 1
}

public static class JsonExtensoes
{
    private static readonly JsonSerializerOptions OpcoesCamelCase = CriarOpcoes(
        JsonNamingPolicy.CamelCase);

    private static JsonSerializerOptions CriarOpcoes(
        JsonNamingPolicy namingPolicy)
    {
        var opcoes = new JsonSerializerOptions
        {
            PropertyNamingPolicy = namingPolicy,
            DictionaryKeyPolicy = namingPolicy,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        opcoes.Converters.Add(new JsonStringEnumConverter());

        return opcoes;
    }

    public static string Serializar<T>(
        this T obj,
        EstrategiaJson estrategia = EstrategiaJson.Padrao)
    {
        var opcoes = estrategia == EstrategiaJson.CamelCase
            ? OpcoesCamelCase
            : JsonPadrao.Opcoes;

        return JsonSerializer.Serialize(obj, opcoes);
    }

    public static T? Deserializar<T>(this string json)
    {
        return JsonSerializer.Deserialize<T>(
            json,
            JsonPadrao.Opcoes);
    }
}