
using AI_Devs.TaskApp.Common.Dtos;
using AI_Devs.TaskApp.Services.Interfaces;
using OpenAI.Net;

namespace AI_Devs.TaskApp.Tasks;

public class Whoami : BaseTask
{
    public Whoami(IOpenAIService openAiService, ITaskService taskService) : base(openAiService, taskService, "whoami")
    {
    }

    public override async Task PerformTask()
    {
        var prompt = "Agent, your primary objective is to accurately guess the identity of a person. You will receive a series of undisclosed hints. " +
            "Analyze each hint thoroughly. Make no assumptions beyond the provided information. " +
            "Maintain confidentiality and avoid bias. Report your guess only when confident. " +
            "This task requires precision and careful deduction. \r\n\r\nRules:" +
            "\r\n- the desired response is only the Identity of this person and nothing else" +
            "\r\n- You should give me the identity only when you are absolutely sure" +
            "\r\n- if you are not sure or the user message is not on topic, simply answer with three dots" +
            "\r\n\r\nExample:" +
            "\r\nQ1: It is a man" +
            "\r\nA1: ..." +
            "\r\nQ2: He created one of the biggest tech companies on Earth" +
            "\r\nA2: ..." +
            "\r\nQ3: He is no longer the CEO of this company and Steve Ballmer took his place" +
            "\r\nA3: Bill Gates";

        var noAnswer = "...";
        var answer = "...";
        var messages = new List<Message>
        {
            Message.Create(ChatRoleType.System, prompt),
        };


        while (answer == noAnswer) {
            var taskContent = await _taskService.GetCustomTypedTaskContent<HintResponseDto>(_taskName);
            messages.Add(Message.Create(ChatRoleType.User, taskContent.Hint));

            var response = await _openAiService.Chat.Get(messages, o =>
            {
                o.Model = ModelTypes.Gpt41106Preview;
                o.MaxTokens = 2000;
            });

            answer = response?.Result?.Choices.First().Message.Content ?? noAnswer;
        }

        var answerResponse = await _taskService.SendAnswer(_taskName, answer);
    }
}
