using AI_Devs.TaskApp.Common.Consts;
using AI_Devs.TaskApp.Common.Dtos;
using AI_Devs.TaskApp.Services.Interfaces;
using RestSharp;

namespace AI_Devs.TaskApp.Services.Services;

public class TaskService : ITaskService
{
    private readonly IAuthenticator authenticator;
    private readonly RestClient restClient;

    public TaskService(IAuthenticator authenticator)
    {
        this.authenticator = authenticator;
        restClient = new RestClient(Urls.BaseUrl);
    }

    public async Task<string> GetRawTaskContent(string taskName)
    {
        var token = await authenticator.GetToken(taskName, restClient);

        var request = new RestRequest($"{Urls.TaskUrl}{token.Value}");

        var response = await restClient.ExecuteAsync(request);

        if (response.IsSuccessful)
        {
            return response.Content;
        }
        else
        {
            return "Error fetching data";
        }
    }

    public async Task<TaskContentResponseDto> GetTaskContent(string taskName)
    {
        var token = await authenticator.GetToken(taskName, restClient);

        var request = new RestRequest($"{Urls.TaskUrl}{token.Value}");
        var response = await restClient.ExecuteGetAsync<TaskContentResponseDto>(request);

        if (!response.IsSuccessful)
        {
            return new TaskContentResponseDto
            {
                Msg = Errors.NoTaskContent
            };
        }

        return response.Data;
    }

    public async Task<AnswerResponseDto> SendAnswer(string taskName, string answer)
    {
        var token = await authenticator.GetToken(taskName, restClient);

        var answerBody = new AnswerRequestDto
        {
            Answer = answer,
        };

        var request = new RestRequest($"{Urls.AnswerUrl}{token.Value}")
            .AddJsonBody(answerBody);

        var answerResponse = await restClient.ExecutePostAsync<AnswerResponseDto>(request);

        if (answerResponse.IsSuccessful)
        {

        }

        return answerResponse.Data;
    }
    
    public async Task<AnswerResponseDto> SendJsonAnswer(string taskName, string answer)
    {
        var token = await authenticator.GetToken(taskName, restClient);

        var request = new RestRequest($"{Urls.AnswerUrl}{token.Value}", Method.Post);
        request.AddParameter("application/json", answer, ParameterType.RequestBody);
        
        var answerResponse = await restClient.ExecutePostAsync<AnswerResponseDto>(request);

        if (answerResponse.IsSuccessful)
        {

        }

        return answerResponse.Data;
    }
}
