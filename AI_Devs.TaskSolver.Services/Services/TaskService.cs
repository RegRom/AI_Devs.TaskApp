using AI_Devs.TaskApp.Common.Consts;
using AI_Devs.TaskApp.Common.Dtos;
using AI_Devs.TaskApp.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace AI_Devs.TaskApp.Services.Services;

public class TaskService : ITaskService
{
    private readonly IAuthenticator authenticator;
    private readonly HttpClient httpClient;
    private readonly ILogger<TaskService> _logger;

    public TaskService(IAuthenticator authenticator, HttpClient httpClient, ILogger<TaskService> logger)
    {
        this.authenticator = authenticator;
        this.httpClient = httpClient;
        httpClient.BaseAddress = new Uri(Urls.BaseUrl);
        _logger = logger;
    }

    public async Task<string> GetRawTaskContent(string taskName)
    {
        var token = await authenticator.GetToken(taskName, httpClient);

        if (token is not null)
        {
            try
            {
                var response = await httpClient.GetAsync($"{Urls.TaskUrl}{token.Value}");

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }
                else
                {
                    return "Error fetching data";
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Getting task content failed with {e.GetType()}: {e.Message}");
                throw;
            }

        }

        return "Failed to get the token for this task";
    }

    public async Task<TaskContentResponseDto> GetTaskContent(string taskName)
    {
        var token = await authenticator.GetToken(taskName, httpClient);
        try
        {
            var response = await httpClient.GetFromJsonAsync<TaskContentResponseDto>($"{Urls.TaskUrl}{token.Value}");

            if (response is null)
            {
                return new TaskContentResponseDto
                {
                    Msg = Errors.NoTaskContent
                };
            }

            return response;
        }
        catch (Exception e)
        {
            _logger.LogError($"Getting task content failed with {e.GetType()}: {e.Message}");
            throw;
        }
    }

    public async Task<TaskContentGenericResponseDto<T>> GetTaskContent<T>(string taskName)
    {
        var token = await authenticator.GetToken(taskName, httpClient);

        var response = await httpClient
            .GetFromJsonAsync<TaskContentGenericResponseDto<T>>($"{Urls.TaskUrl}{token.Value}");

        if (response is null)
        {
            return new TaskContentGenericResponseDto<T>
            {
                Msg = Errors.NoTaskContent
            };
        }

        return response;
    }

    public async Task<T> GetCustomTypedTaskContent<T>(string taskName) where T : BaseTaskContentDto, new()
    {
        var token = await authenticator.GetToken(taskName, httpClient);

        var response = await httpClient
            .GetFromJsonAsync<T>($"{Urls.TaskUrl}{token.Value}");

        if (response is null)
        {
            return new T
            {
                Msg = Errors.NoTaskContent
            };
        }

        return response;
    }

    public async Task<AnswerResponseDto?> SendAnswer(string taskName, string answer)
    {
        var token = await authenticator.GetToken(taskName, httpClient);
        var answerBody = new AnswerRequestDto
        {
            Answer = answer,
        };

        var httpResponse = await httpClient.PostAsJsonAsync($"{Urls.AnswerUrl}{token.Value}", answerBody);

        if (!httpResponse.IsSuccessStatusCode)
        {
            _logger.LogError($"Failed to post data. Status code: {httpResponse.StatusCode}");
        }

        var responseContent = await httpResponse.Content.ReadAsStringAsync();
        var answerResponse = JsonSerializer.Deserialize<AnswerResponseDto>(responseContent);

        return answerResponse;
    }
    
    public async Task<AnswerResponseDto?> SendJsonAnswer(string taskName, string answer)
    {
        var token = await authenticator.GetToken(taskName, httpClient);
        var httpContent = new StringContent(answer, Encoding.UTF8, "application/json");
        var httpResponse = await httpClient.PostAsync($"{Urls.AnswerUrl}{token.Value}", httpContent);

        if (!httpResponse.IsSuccessStatusCode)
        {
            _logger.LogError($"Failed to post data. Status code: {httpResponse.StatusCode}");
        }

        var responseString = await httpResponse.Content.ReadAsStringAsync();

        return JsonSerializer.Deserialize<AnswerResponseDto>(responseString);
    }

    public async Task<QuestionResponseDto?> SendQuestion(string question, string taskName)
    {
        var token = await authenticator.GetToken(taskName, httpClient);

        var content = new MultipartFormDataContent
        {
            { new StringContent(question), "question" }
        };

        var response = await httpClient.PostAsync($"{Urls.TaskUrl}{token.Value}", content);

        if (response.IsSuccessStatusCode)
        {
            var responseString = await response.Content.ReadAsStringAsync();
            var answerResponse = JsonSerializer.Deserialize<QuestionResponseDto>(responseString);
            return answerResponse;
        }
        else
        {
            return new QuestionResponseDto { Code = -1, Msg = "Error fetching data" };
        }
    }

}
