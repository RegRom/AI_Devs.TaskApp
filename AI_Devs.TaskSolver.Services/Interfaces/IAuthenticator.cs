using AI_Devs.TaskApp.Common.Dtos;
using AI_Devs.TaskApp.Common.Models;

namespace AI_Devs.TaskApp.Services.Interfaces;

public interface IAuthenticator
{
    Task<Token?> GetToken(string taskName, HttpClient restClient);
}
