using System.Text.Json.Serialization;

namespace AI_Devs.TaskApp.Common.Dtos;

public class TaskContentQuestionResponse : BaseTaskContentDto
{
    [JsonPropertyName("question")]
    public string Question { get; set; }
}
