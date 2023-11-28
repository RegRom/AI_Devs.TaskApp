using AI_Devs.TaskApp.Services.Interfaces;
using Microsoft.Extensions.Logging;
using OpenAI.Net;

namespace AI_Devs.TaskApp.Tasks;

public class Scraper : BaseTask
{
    private readonly IFileService _fileService;
    private readonly ILogger<Scraper> _logger;

    public Scraper(IOpenAIService openAiService, ITaskService taskService, IFileService fileService, ILogger<Scraper> logger) : base(openAiService, taskService, "scraper")
    {
        _fileService = fileService;
        _logger = logger;
    }

    public override async Task PerformTask()
    {
        var taskContent = await _taskService.GetTaskContent<string>(_taskName);
        string fileContent;

        try
        {
            if (Uri.IsWellFormedUriString(taskContent.Input, UriKind.Absolute))
            {
                var customHeaders = new Dictionary<string, string>
                {
                    { "User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/110.0.0.0 Safari/537.36" }
                };

                fileContent = await _fileService.DownloadFileAsync(taskContent.Input, customHeaders);

                var prompt = "You will answer question asked by the user using only the context specified below:" +
                    $"\r\n### Context \r\n{fileContent} \r\n###";

                var messages = new List<Message>
                {
                    Message.Create(ChatRoleType.System, prompt),
                    Message.Create(ChatRoleType.User, taskContent.Question),
                };

                var response = await _openAiService.Chat.Get(messages, o =>
                {
                    o.Model = ModelTypes.Gpt41106Preview;
                    o.MaxTokens = 2000;
                });

                var answerResponse = await _taskService.SendAnswer(_taskName, response.Result.Choices.First().Message.Content);
            }

        }
        catch (Exception e)
        {
            _logger.LogError($"Task failed with {e.GetType()}: {e.Message}");
            throw;
        }

        //var response = await _openAiService.Files.Upload(@"Images\BabyCat.png");
    }
}
