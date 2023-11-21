namespace AI_Devs.TaskApp.Services.Interfaces;

public interface IFileService
{
    Task DownloadFileAsync(string fileUrl, string localPath);
}