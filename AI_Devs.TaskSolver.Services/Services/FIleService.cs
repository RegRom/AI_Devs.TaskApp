using AI_Devs.TaskApp.Services.Interfaces;

namespace AI_Devs.TaskApp.Services.Services;

public class FileService : IFileService
{
    private readonly IHttpClientFactory _httpClientFactory;
    
    public FileService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
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
}