using AI_Devs.TaskApp.Common.Consts;
using AI_Devs.TaskApp.Common.Dtos;
using AI_Devs.TaskApp.Common.Models;
using AI_Devs.TaskApp.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using RestSharp;

namespace AI_Devs.TaskApp.Services.Services;

public class Authenticator : IAuthenticator
{
    private Token token;
    private readonly IConfiguration configuration;

    public Authenticator(IConfiguration configuration)
    {
        this.configuration = configuration;
    }

    private async Task<TokenResponseDto?> GetTokenFromApi(string taskName, RestClient restClient)
    {
        string endpointUrl = $"{Urls.TokenUrl}{taskName}";
        var configSection = configuration.GetSection("AI_Devs");
        var tokenBody = new TokenRequestDto
        {
            ApiKey = configSection["ApiKey"],
        };
        var request = new RestRequest(endpointUrl)
            .AddJsonBody(tokenBody);

        var tokenResponse = await restClient.ExecutePostAsync<TokenResponseDto>(request);

        if (!tokenResponse.IsSuccessful)
        {
            Console.WriteLine($"Failed to post data. Status code: {tokenResponse.StatusCode}");

            return null;
        }

        return tokenResponse.Data;
    }

    public async Task<Token> GetToken(string taskName, RestClient restClient)
    {
        if (token == null || token.IssuedAt <= DateTimeOffset.Now - TimeSpan.FromMinutes(2))
        {
            var tokenDto = await GetTokenFromApi(taskName, restClient);
            //TODO: API can return null

            token = new Token
            {
                IssuedAt = DateTimeOffset.Now,
                Value = tokenDto.Token
            };
        }

        return token;
    }
}
