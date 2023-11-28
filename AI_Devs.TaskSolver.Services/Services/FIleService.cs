using AI_Devs.TaskApp.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace AI_Devs.TaskApp.Services.Services;

public class FileService : IFileService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<FileService> _logger;

    public FileService(IHttpClientFactory httpClientFactory, ILogger<FileService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task DownloadFileAsync(string fileUrl, string localPath)
    {
        var httpClient = _httpClientFactory.CreateClient();
        
        try
        {
            var response = await httpClient.GetAsync(fileUrl, HttpCompletionOption.ResponseHeadersRead);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Error: {response.StatusCode}");
            }

            await using var stream = await response.Content.ReadAsStreamAsync();
            await using var fileStream = new FileStream(localPath, FileMode.Create, FileAccess.Write, FileShare.None);
            await stream.CopyToAsync(fileStream);

            Console.WriteLine("Download completed successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }

    public async Task<string> DownloadFileAsync(string url, Dictionary<string, string>? customHeaders = null)
    {
        var httpClient = _httpClientFactory.CreateClient();
        try
        {
            if (customHeaders != null)
            {
                foreach (var header in customHeaders)
                {
                    httpClient.DefaultRequestHeaders.TryAddWithoutValidation(header.Key, header.Value);
                }
            }
            HttpResponseMessage response = await httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine("Error: Unable to download the file.");
                return "";
            }

            string content = await response.Content.ReadAsStringAsync();
            return content;
        }
        catch (Exception e)
        {
            _logger.LogError($"Getting file content failed with {e.GetType()}: {e.Message}");
            return null;
        }
        
    }
}