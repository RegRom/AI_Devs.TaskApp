using AI_Devs.TaskApp.Services.Interfaces;
using OpenAI.Net;

namespace AI_Devs.TaskApp.Tasks;

public class Whisper : BaseTask
{
    public Whisper(IOpenAIService openAiService, ITaskService taskService) : base(openAiService, taskService, "whisper")
    {
    }

    public Whisper(IOpenAIService openAiService, ITaskService taskService, string taskName) : base(openAiService, taskService, taskName)
    {
    }

    public override async Task PerformTask()
    {
        var content = await _taskService.GetRawTaskContent(_taskName);
        
        
    }
}