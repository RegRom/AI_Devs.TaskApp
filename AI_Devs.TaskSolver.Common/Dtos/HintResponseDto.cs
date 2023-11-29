using System.Text.Json.Serialization;

namespace AI_Devs.TaskApp.Common.Dtos;

public class HintResponseDto : BaseTaskContentDto
{
    [JsonPropertyName("hint")]
    public string Hint { get; set; }
}
