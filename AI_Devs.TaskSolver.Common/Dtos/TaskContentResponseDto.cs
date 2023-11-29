using System.Text.Json.Serialization;

namespace AI_Devs.TaskApp.Common.Dtos;

public class TaskContentResponseDto : BaseTaskContentDto
{
    [JsonPropertyName("input")]
    public object Input { get; set; }
}
