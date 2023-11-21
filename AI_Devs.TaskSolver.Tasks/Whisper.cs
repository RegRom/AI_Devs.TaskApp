using System.Text.Json;
using AI_Devs.TaskApp.Services.Interfaces;
using OpenAI.Net;

namespace AI_Devs.TaskApp.Tasks;

public class Whisper : BaseTask
{
    private readonly IFileService _fileService;
    public Whisper(IOpenAIService openAiService, ITaskService taskService, IFileService fileService) : base(openAiService, taskService, "whisper")
    {
        _fileService = fileService;
    }

    public Whisper(IOpenAIService openAiService, ITaskService taskService, string taskName, IFileService fileService) : base(openAiService, taskService, taskName)
    {
        _fileService = fileService;
    }

    public override async Task PerformTask()
    {
        var content = await _taskService.GetTaskContent(_taskName);
        var localPath = "D:\\audio.mp3";
        string extracted;
        
        int semicolonIndex = content.Msg.IndexOf(':');

        if (semicolonIndex != -1)
        {
            extracted = content.Msg.Substring(semicolonIndex + 1).Trim();
            Console.WriteLine(extracted);
        }
        else
        {
            throw new ArgumentException("There is no text to create embedding from");
        }

        try
        {
            await _fileService.DownloadFileAsync(extracted, localPath);
        }
        catch (Exception)
        {
            throw;
        }

        var response = await _openAiService.Audio.GetTranscription(localPath);
        
        var jsonAnswer = new { answer = response.Result.Text };
        string jsonString = JsonSerializer.Serialize(jsonAnswer);
        
        var answerResponse = await _taskService.SendJsonAnswer(_taskName, jsonString);
    }
}