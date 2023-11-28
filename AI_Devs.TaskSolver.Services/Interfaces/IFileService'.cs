namespace AI_Devs.TaskApp.Services.Interfaces;

public interface IFileService
{
    Task DownloadFileAsync(string fileUrl, string localPath);

    Task<string> DownloadFileAsync(string url, Dictionary<string, string>? customHeaders = null);
}