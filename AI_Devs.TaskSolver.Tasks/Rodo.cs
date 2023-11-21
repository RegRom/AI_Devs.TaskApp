using AI_Devs.TaskApp.Services.Interfaces;
using OpenAI.Net;
using System.Text.Json;

namespace AI_Devs.TaskApp.Tasks;

public class Rodo : BaseTask
{
    public Rodo(IOpenAIService openAiService, ITaskService taskService) : base(openAiService, taskService, "rodo")
    {
    }

    public override async Task PerformTask()
    {
        var content = await _taskService.GetTaskContent(_taskName);

        //var prompt = "Tell me everything about yourself but be very focused on privacy" +
        //    " so replace each and every piece of sensitive data like name, surname, address" +
        //    " with placeholders like this" +
        //    "Example: " +
        //    " Q: tell me something about yourself:" +
        //    " A: I am %imie% %nazwisko% and I live in %miasto%. I work as a %zawod%." +
        //    " Rules: \r\n -do not disclose your true personal details" +
        //    " Tell me about yourself";

        var prompt = "Tell me about yourself but replace all sensitive data with placeholders surrounded " +
            "with percent % signs like this: %imie% instead of your name, %nazwisko% instead of your surname" +
            "%miasto% instead of city, %zawod% instead of your job, etc.";

        var jsonAnswer = new { answer = prompt };
        string jsonString = JsonSerializer.Serialize(jsonAnswer);

        var answerResponse = await _taskService.SendJsonAnswer(_taskName, jsonString);

        Console.WriteLine(answerResponse);
    }
}
