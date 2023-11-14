using System.Text.Json.Serialization;

namespace AI_Devs.TaskApp.Common.Dtos;

public class TaskContentGenericResponseDto<T>
{
    [JsonPropertyName("code")]
    public int Code { get; set; }

    [JsonPropertyName("msg")]
    public string Msg { get; set; }

    [JsonPropertyName("input")]
    public T Input { get; set; }

    [JsonPropertyName("question")]
    public string Question { get; set; }
}
