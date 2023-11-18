using System.Text.Json;
using AI_Devs.TaskApp.Services.Interfaces;
using OpenAI.Net;

namespace AI_Devs.TaskApp.Tasks;

public class Embedding : BaseTask
{
    public Embedding(IOpenAIService openAiService, ITaskService taskService) : base(openAiService, taskService, "embedding")
    {
    }

    public Embedding(IOpenAIService openAiService, ITaskService taskService, string taskName) : base(openAiService, taskService, taskName)
    {
    }
    
    public override async Task PerformTask()
    {
        var content = await _taskService.GetTaskContent<string[]>(_taskName);
        string extracted;
        
        int semicolonIndex = content.Msg.IndexOf(':');

        if (semicolonIndex != -1)
        {
            extracted = content.Msg.Substring(semicolonIndex + 1).Trim();
            Console.WriteLine(extracted);
        }
        else
        {
            throw new ArgumentException("There is no text to create embedding from");
        }
        
        var response = await _openAiService.Embeddings.Create(extracted, "text-embedding-ada-002", "test");
        var jsonAnswer = new { answer = response.Result.Data.First().Embedding };
        string jsonString = JsonSerializer.Serialize(jsonAnswer);
        
        var answerResponse = await _taskService.SendJsonAnswer(_taskName, jsonString);
    }
}