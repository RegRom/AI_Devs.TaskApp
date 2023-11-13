using System.Text.Json.Serialization;

namespace AI_Devs.TaskApp.Common.Dtos;

public class TaskContentResponseDto
{
    [JsonPropertyName("code")]
    public int Code { get; set; }

    [JsonPropertyName("msg")]
    public string Msg { get; set; }

    [JsonPropertyName("input")]
    public object Input { get; set; }
}
