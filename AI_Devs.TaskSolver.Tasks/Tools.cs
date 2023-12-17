
using AI_Devs.TaskApp.Common.Dtos;
using AI_Devs.TaskApp.Services.Interfaces;
using OpenAI.Net;

namespace AI_Devs.TaskApp.Tasks;

public class Tools : BaseTask
{
    public Tools(IOpenAIService openAiService, ITaskService taskService) : base(openAiService, taskService, "tools")
    {
    }

    public override async Task PerformTask()
    {
        var taskContent = await _taskService.GetCustomTypedTaskContent<TaskContentQuestionResponse>(_taskName);

        string tools =
            $"- ToDo\r\n" +
            $"- Calendar\r\n\r\n";

        string prompt = $"You are a prompt classifier. You accept an input and classify as one of predefined tools and return a json object appropriate for this category. \r\n\r\n" +
            $"### tools:\r\n" +
            tools +
            $"### Rules:\r\n" +
            $"- if user prompt mentions a date, then the tool is Calendar\r\n" +
            $"- if there is no mention of date, choose ToDo\r\n" +
            $"- be ultra concise and return only JSON\r\n" +
            $"- no comments or any text that is not json\r\n" +
            $"- if the date is relative, refer to date specified below\r\n" +
            $"- use YYYY-MM-DD format for dates\r\n\r\n" +
            $"### Date\r\n{DateTime.Today}\r\n\r\n" +
            "Examples:\r\n" +
            "Q: \"Przypomnij mi, że mam kupić mleko\r\nA: {\r\n  \"answer\": {\r\n    \"tool\": \"ToDo\",\r\n    \"desc\": \"Kup mleko\"\r\n  }\r\n}\r\n\r\n\r\n" +
            "Q: \"Jutro mam spotkanie z Marianem \r\nA: {\r\n  \"answer\": {\r\n    \"tool\": \"Calendar\",\r\n    \"desc\": \"Spotkanie z Marianem\",\r\n    \"date\": \"2023-12-16\"\r\n  }\r\n}";

        var messages = new List<Message>
        {
            Message.Create(ChatRoleType.System, prompt),
            Message.Create(ChatRoleType.User, taskContent.Question),
        };

        var response = await _openAiService.Chat.Get(messages, o =>
        {
            o.Model = ModelTypes.Gpt41106Preview;
            o.ResponseFormat = new ChatResponseFormatType { Type = "json_object" };
            o.MaxTokens = 2000;
        });

        var answerResponse = await _taskService.SendJsonAnswer(_taskName, response.Result.Choices.First().Message.Content);

        Console.WriteLine(answerResponse);
    }
}
