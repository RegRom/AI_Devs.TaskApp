using System.Text.Json.Serialization;

namespace AI_Devs.TaskApp.Common.Dtos;

public class UnknowNewsEntryDto
{
    [JsonPropertyName("title")]
    public string Title { get; set; }
    [JsonPropertyName("url")]
    public string Url { get; set; }
    [JsonPropertyName("info")]
    public string Info { get; set; }
    [JsonPropertyName("date")]
    public string Date { get; set; }
}
