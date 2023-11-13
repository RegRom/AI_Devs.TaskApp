using AI_Devs.TaskApp.Common.Dtos;

namespace AI_Devs.TaskApp.Services.Interfaces;

public interface ITaskService
{
    Task<TaskContentResponseDto> GetTaskContent(string taskName);

    Task<string> GetRawTaskContent(string taskName);

    Task<AnswerResponseDto> SendAnswer(string taskName, string answer);

    Task<AnswerResponseDto> SendJsonAnswer(string taskName, string answer);
}
