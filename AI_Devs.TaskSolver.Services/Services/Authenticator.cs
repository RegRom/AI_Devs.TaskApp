using AI_Devs.TaskApp.Common.Consts;
using AI_Devs.TaskApp.Common.Dtos;
using AI_Devs.TaskApp.Common.Models;
using AI_Devs.TaskApp.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;

namespace AI_Devs.TaskApp.Services.Services;

public class Authenticator : IAuthenticator
{
    private Token? token;
    private readonly IConfiguration configuration;
    private readonly ILogger<Authenticator> _logger;

    public Authenticator(IConfiguration configuration, ILogger<Authenticator> logger)
    {
        this.configuration = configuration;
        _logger = logger;
    }

    private async Task<TokenResponseDto?> GetTokenFromApi(string taskName, HttpClient httpClient)
    {
        string endpointUrl = $"{Urls.TokenUrl}{taskName}";
        var configSection = configuration.GetSection("AI_Devs");
        var tokenBody = new TokenRequestDto
        {
            ApiKey = configSection["ApiKey"],
        };

        var jsonContent = JsonSerializer.Serialize(tokenBody);
        var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        try
        {
            var httpResponse = await httpClient.PostAsync(endpointUrl, httpContent);
            var responseContent = await httpResponse.Content.ReadAsStringAsync();

            if (!httpResponse.IsSuccessStatusCode)
            {
                _logger.LogError($"Failed to post data. Status code: {httpResponse.StatusCode}");
                return null;
            }

            return JsonSerializer.Deserialize<TokenResponseDto>(responseContent);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, taskName, ex.Message );
            return null;
        }
    }


    public async Task<Token?> GetToken(string taskName, HttpClient httpClient)
    {
        if (token?.Value == null || token.IssuedAt <= DateTimeOffset.Now - TimeSpan.FromMinutes(2))
        {
            var tokenDto = await GetTokenFromApi(taskName, httpClient);

            _ = tokenDto is not null
                ? token = new Token
                {
                    IssuedAt = DateTimeOffset.Now,
                    Value = tokenDto.Token
                }
                : token = null;
        }

        return token;
    }
}
