
using AI_Devs.TaskApp.Common.Consts;
using AI_Devs.TaskApp.Common.Dtos;
using AI_Devs.TaskApp.Services.Interfaces;
using Microsoft.SemanticKernel.Connectors.Memory.Qdrant;
using Microsoft.SemanticKernel.Memory;
using Microsoft.SemanticKernel.Plugins.Memory;
using OpenAI.Net;
using System.Net.Http.Json;

namespace AI_Devs.TaskApp.Tasks;

public class Search : BaseTask
{
    private readonly IHttpClientFactory _httpClientFactory;

    public Search(IOpenAIService openAiService, ITaskService taskService, IHttpClientFactory httpClientFactory) : base(openAiService, taskService, "search")
    {
        _httpClientFactory = httpClientFactory;
    }

    public async override Task PerformTask()
    {
        var collectionName = "unknowNews";
        var qdrantHttpClient = _httpClientFactory.CreateClient();
        qdrantHttpClient.BaseAddress = new Uri(Urls.LocalQdrant);
        var qdrantClient = new QdrantVectorDbClient(qdrantHttpClient, OpenAi.VectorSize);

        var taskContent = await _taskService.GetCustomTypedTaskContent<TaskContentQuestionResponse>(_taskName);

        var response = await _openAiService.Embeddings.Create(taskContent.Question, "text-embedding-ada-002", "test");
        var floatEmbedding = Array.ConvertAll(response.Result.Data.First().Embedding, item => (float)item);

        var nearestPoints = await qdrantClient.FindNearestInCollectionAsync(collectionName, floatEmbedding, 0.7).ToListAsync();

        var nearestPoint = nearestPoints.FirstOrDefault();

        var answerResponse = await _taskService.SendAnswer(_taskName, nearestPoint.Item1.Payload["Url"].ToString());
    }

    public async Task PrepareTask(int entryCount)
    {
        var collectionName = "unknowNews";
        var qdrantHttpClient = _httpClientFactory.CreateClient();
        qdrantHttpClient.BaseAddress = new Uri(Urls.LocalQdrant);
        var qdrantClient = new QdrantVectorDbClient(qdrantHttpClient, OpenAi.VectorSize);

        if (!await qdrantClient.DoesCollectionExistAsync(collectionName))
        {
            await qdrantClient.CreateCollectionAsync(collectionName);
        }

        var newsLetterClient = _httpClientFactory.CreateClient();
        var newsletterList = new List<UnknowNewsEntryDto>();
        var points = new List<QdrantVectorRecord>();
        try
        {
            newsletterList = await newsLetterClient.GetFromJsonAsync<List<UnknowNewsEntryDto>>("https://unknow.news/archiwum.json");
        }
        catch (Exception)
        {
            throw;
        }

        int counter = 0;
        foreach (var entry in newsletterList.Take(entryCount))
        {
            var response = await _openAiService.Embeddings.Create(entry.Info, "text-embedding-ada-002", "test");
            var floatEmbedding = Array.ConvertAll(response.Result.Data.First().Embedding, item => (float)item);
            var metadata = new Dictionary<string, object>
            {
                { "Url", entry.Url },
                { "Title", entry.Title },
                { "Info", entry.Info }
            };
            var qdrantVector = new QdrantVectorRecord(
                Guid.NewGuid().ToString(),
                floatEmbedding,
                metadata
            );
            points.Add(qdrantVector);
        }
        if ( points.Any() )
        {
            await qdrantClient.UpsertVectorsAsync(collectionName, points);
        }
    }
}
