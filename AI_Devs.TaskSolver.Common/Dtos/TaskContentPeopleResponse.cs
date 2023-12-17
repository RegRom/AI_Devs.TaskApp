using System.Text.Json.Serialization;

namespace AI_Devs.TaskApp.Common.Dtos;

public class TaskContentPeopleResponse : TaskContentQuestionResponse
{
    [JsonPropertyName("data")]
    public string Data { get; set; }
}
