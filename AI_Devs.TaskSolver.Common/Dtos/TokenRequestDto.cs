using System.Text.Json.Serialization;

namespace AI_Devs.TaskApp.Common.Dtos;

public class TokenRequestDto
{
    [JsonPropertyName("apikey")]
    public string ApiKey { get; set; }
}
