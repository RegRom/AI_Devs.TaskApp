using AI_Devs.TaskApp.Services.Interfaces;
using OpenAI.Net;

namespace AI_Devs.TaskApp.Tasks;

public class Inprompt : BaseTask
{
    public Inprompt(IOpenAIService openAiService, ITaskService taskService) : base(openAiService, taskService, nameof(Inprompt).ToLower())
    {
    }

    public Inprompt(IOpenAIService openAiService, ITaskService taskService, string taskName) : base(openAiService, taskService, taskName)
    {
    }
    
    public override async Task PerformTask()
    {
        var content = await _taskService.GetTaskContent<string[]>(_taskName);
        
        var filterPrompt = "Given a sentence with a nam, return a name from it:\r\n\r\nExample:\r\n" +
            "Q: Mathias is a good friend\r\nA: Mathias\r\n" +
            "Q: Is Anna a very competitive scrabble player?\r\nA: Anna\r\n\r\nRules:\r\n- always return only the name";

        var filterMessages = new List<Message>
        {
            Message.Create(ChatRoleType.System, filterPrompt),
            Message.Create(ChatRoleType.User, content.Question),
        };

        var filterResponse = await _openAiService.Chat.Get(filterMessages, o =>
        {
            o.Model = ModelTypes.Gpt3_5Turbo0613;
            o.MaxTokens = 200;
        });

        var relevantInput = content.Input.Where(x => x.Contains(filterResponse.Result.Choices.First().Message.Content));
        var prompt = "Given a list of sentences and a question a question, answer the question." +
                     "Rules: \r\n -use only data provided by the user to answer the question" +
                     "\r\n - answer as concisely ans truthfully as possible";
        
        var messages = new List<Message>
        {
            Message.Create(ChatRoleType.System, prompt),
            Message.Create(ChatRoleType.User, $"Sentences: {string.Join(", ", relevantInput)} \r\n {content.Question}"),
        };

        var response = await _openAiService.Chat.Get(messages, o =>
        {
            o.Model = ModelTypes.Gpt41106Preview;
            //o.ResponseFormat = new ChatResponseFormatType { Type = "json_object" };
            o.MaxTokens = 500;
        });

        var answerResponse = await _taskService.SendAnswer(_taskName, response.Result.Choices.First().Message.Content);
    }
}