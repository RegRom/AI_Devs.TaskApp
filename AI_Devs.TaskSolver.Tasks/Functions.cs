using AI_Devs.TaskApp.Services.Interfaces;
using OpenAI.Net;
using System.Text.Json;

namespace AI_Devs.TaskApp.Tasks;

public class Functions : BaseTask
{
    public Functions(IOpenAIService openAiService, ITaskService taskService) : base(openAiService, taskService, "functions")
    {
    }

    public override async Task PerformTask()
    {
        var content = await _taskService.GetTaskContent(_taskName);

        var function = new 
        {
            name = "addUser",
            description = "Adds user",
            parameters = new {
                type = "object",
                properties = new
                {
                    name = new
                    {
                        type = "string",
                        description = "The name of the user to add"
                    },
                    surname = new
                    {
                        type = "string",
                        description = "The surname of the user to add"
                    },
                    year = new
                    {
                        type = "integer",
                        description = "The year of birth of the user to add"
                    }
                }
            }
        };

        var jsonAnswer = new { answer = function };
        string jsonString = JsonSerializer.Serialize(jsonAnswer);

        var answerResponse = await _taskService.SendJsonAnswer(_taskName, jsonString);
    }
}
