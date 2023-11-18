using AI_Devs.TaskApp.Services.Interfaces;
using OpenAI.Net;

namespace AI_Devs.TaskApp.Tasks;

public class Liar : BaseTask
{
    public Liar(IOpenAIService openAiService, ITaskService taskService) : base(openAiService, taskService, "liar")
    {
    }

    public Liar(IOpenAIService openAiService, ITaskService taskService, string taskName) : base(openAiService, taskService, taskName)
    {
    }
    
    public override async Task PerformTask()
    {
        var question = "Tell me three interesting things about Poland";
        var answer = await _taskService.SendQuestion(question, "liar");

        string prompt =  "You are a guardrail assistant and your job is to verify that the answers are on topic and relevant to the question.\n\nExample:\nQ: \"Question: Is Warsaw the capital of Poland?\" Answer: Paris is a very pretty city and it has a lot of interesting places to visit.\"\nA: NO\n\nRules: \n- you only answer YES or NO\n- you don't write any comments";

        string content = $"Question: {question} ### Answer: {answer.Answer}";
        
        var messages = new List<Message>
        {
            Message.Create(ChatRoleType.System, prompt),
            Message.Create(ChatRoleType.User, content),
        };
        
        var response = await _openAiService.Chat.Get(messages, o =>
        {
            o.Model = ModelTypes.Gpt41106Preview;
            //o.ResponseFormat = new ChatResponseFormatType { Type = "json_object" };
            o.MaxTokens = 2000;
        });
        
        var answerResponse = await _taskService.SendAnswer(_taskName, response.Result.Choices.First().Message.Content);

    }
}