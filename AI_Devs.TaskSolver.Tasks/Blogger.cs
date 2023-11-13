using AI_Devs.TaskApp.Common.Dtos;
using AI_Devs.TaskApp.Services.Interfaces;
using AI_Devs.TaskApp.Services.Services;
using OpenAI.Net;

namespace AI_Devs.TaskApp.Tasks;

public class Blogger
{
    private readonly TaskContentGenericResponseDto<string[]> taskContentGenericResponseDto;
    private readonly IOpenAIService openAIService;
    private readonly ITaskService taskService;
    private readonly string taskName = "blogger";

    public Blogger()
    {
        
    }

    public Blogger(IOpenAIService openAIService, ITaskService taskService)
    {
        this.openAIService = openAIService;
        this.taskService = taskService;
    }

    public async Task PerformTask()
    {
        var content = await taskService.GetRawTaskContent(taskName);

        string prompt = @"You are a culinary blogger assistant that prepares a json with content for automated article creation. 
            You accept a list of chapter names and you generate a JSON file with a  table of strings that contains content for each chapter.
            Rules:
            \r\n- you always use JSON format
            \r\n- the only field of this json is answer and it is a table of string values
            \r\n- table does not contain chapter names but chapter content
            \r\n- keep chapters short
            \r\n- chapters should be written in the same language as chapter name
            \r\n- response has to be a valid json";


        var messages = new List<Message>
        {
            Message.Create(ChatRoleType.System, prompt),
            Message.Create(ChatRoleType.User, content),
        };

        var response = await openAIService.Chat.Get(messages, o =>
        {
            o.Model = ModelTypes.Gpt41106Preview;
            o.ResponseFormat = new ChatResponseFormatType { Type = "json_object" };
            o.MaxTokens = 2000;
        });

        var answerResponse = await taskService.SendJsonAnswer(taskName, response.Result.Choices.First().Message.Content);

    }
}

internal class TaskContent
{
    public string Command { get; set; }

    public string[] Input { get; set; }
}