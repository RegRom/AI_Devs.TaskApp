using System.Text.Json.Serialization;

namespace AI_Devs.TaskApp.Common.Dtos;

public class TokenResponseDto
{
    [JsonPropertyName("code")]
    public int Code { get; set; }

    [JsonPropertyName("msg")]
    public string Msg { get; set; }

    [JsonPropertyName("token")]
    public string Token { get; set; }
}
