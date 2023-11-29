using System.Text.Json.Serialization;

namespace AI_Devs.TaskApp.Common.Dtos;

public class TaskContentGenericResponseDto<T> : BaseTaskContentDto
{
    [JsonPropertyName("input")]
    public T Input { get; set; }

    [JsonPropertyName("question")]
    public string Question { get; set; }
}
