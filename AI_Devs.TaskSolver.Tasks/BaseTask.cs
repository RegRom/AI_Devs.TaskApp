using AI_Devs.TaskApp.Services.Interfaces;
using AI_Devs.TaskApp.Tasks.Interfaces;
using OpenAI.Net;

namespace AI_Devs.TaskApp.Tasks;

public abstract class BaseTask : ITaskPerformer
{
    protected readonly IOpenAIService _openAiService;
    protected readonly ITaskService _taskService;
    protected string _taskName;

    public BaseTask(IOpenAIService openAiService, ITaskService taskService)
    {
        _openAiService = openAiService;
        _taskService = taskService;
    }
    
    public BaseTask(IOpenAIService openAiService, ITaskService taskService, string taskName)
    {
        _openAiService = openAiService;
        _taskService = taskService;
        _taskName = taskName;
    }

    public abstract Task PerformTask();
}