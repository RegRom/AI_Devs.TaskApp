using AI_Devs.TaskApp.Common.Dtos;

namespace AI_Devs.TaskApp.Services.Interfaces;

public interface ITaskService
{
    Task<TaskContentResponseDto> GetTaskContent(string taskName);

    Task<TaskContentGenericResponseDto<T>> GetTaskContent<T>(string taskName);

    Task<string> GetRawTaskContent(string taskName);

    Task<AnswerResponseDto?> SendAnswer(string taskName, string answer);

    Task<AnswerResponseDto?> SendJsonAnswer(string taskName, string answer);

    Task<QuestionResponseDto?> SendQuestion(string question, string taskName);

    Task<T> GetCustomTypedTaskContent<T>(string taskName) where T : BaseTaskContentDto, new();
}
