
using AI_Devs.TaskApp.Common.Consts;
using AI_Devs.TaskApp.Common.Dtos;
using AI_Devs.TaskApp.Services.Interfaces;
using Microsoft.SemanticKernel.Connectors.Memory.Qdrant;
using OpenAI.Net;
using System.Linq;
using System.Net.Http.Json;
using System.Text.Json;

namespace AI_Devs.TaskApp.Tasks;

public class People : BaseTask
{
    private readonly IHttpClientFactory _httpClientFactory;

    public People(IOpenAIService openAiService, ITaskService taskService, IHttpClientFactory httpClientFactory) : base(openAiService, taskService, "people")
    {
        _httpClientFactory = httpClientFactory;
    }

    public override async Task PerformTask()
    {
        var collectionName = "people";
        var qdrantHttpClient = _httpClientFactory.CreateClient();
        qdrantHttpClient.BaseAddress = new Uri(Urls.LocalQdrant);
        var qdrantClient = new QdrantVectorDbClient(qdrantHttpClient, OpenAi.VectorSize);

        var taskContent = await _taskService.GetCustomTypedTaskContent<TaskContentPeopleResponse>(_taskName);

        //await PrepareTask(null, taskContent.Data);

        var embedding = await _openAiService.Embeddings.Create(taskContent.Question, "text-embedding-ada-002", "test");
        var floatEmbedding = Array.ConvertAll(embedding.Result.Data.First().Embedding, item => (float)item);

        var nearestPoints = await qdrantClient.FindNearestInCollectionAsync(collectionName, floatEmbedding, 0.7).ToListAsync();

        var nearestPoint = nearestPoints.FirstOrDefault();

        string prompt = $"Answer the question based on the context given below and only based on that: " +
            $"\r\n {JsonSerializer.Serialize(nearestPoint.Item1.Payload["Person"])}";


        var messages = new List<Message>
        {
            Message.Create(ChatRoleType.System, prompt),
            Message.Create(ChatRoleType.User, taskContent.Question),
        };

        var response = await _openAiService.Chat.Get(messages, o =>
        {
            o.Model = ModelTypes.Gpt41106Preview;
            o.MaxTokens = 200;
        });

        var answerResponse = await _taskService.SendAnswer(_taskName, response.Result.Choices.First().Message.Content);
        Console.WriteLine(answerResponse.Msg);
    }

    public async Task PrepareTask(int? entryCount, string dataSource)
    {
        var collectionName = "people";
        var qdrantHttpClient = _httpClientFactory.CreateClient();
        qdrantHttpClient.BaseAddress = new Uri(Urls.LocalQdrant);
        var qdrantClient = new QdrantVectorDbClient(qdrantHttpClient, OpenAi.VectorSize);

        if (!await qdrantClient.DoesCollectionExistAsync(collectionName))
        {
            await qdrantClient.CreateCollectionAsync(collectionName);
        }

        var dataClient = _httpClientFactory.CreateClient();
        var dataList = new List<PersonDto>();
        var points = new List<QdrantVectorRecord>();
        try
        {
            dataList = await dataClient.GetFromJsonAsync<List<PersonDto>>(dataSource);
        }
        catch (Exception)
        {
            throw;
        }

        int counter = 0;
        List<PersonDto> collection;

        if (entryCount.HasValue)
        {
            collection = dataList.Take(entryCount.Value).ToList();
        }
        else
        {
            collection = dataList;
        }

        foreach (var entry in collection)
        {
            var response = await _openAiService.Embeddings.Create($"{entry.Name} {entry.Surname}", "text-embedding-ada-002", "test");
            var floatEmbedding = Array.ConvertAll(response.Result.Data.First().Embedding, item => (float)item);
            var metadata = new Dictionary<string, object>
            {
                { "Person", entry },
            };
            var qdrantVector = new QdrantVectorRecord(
                Guid.NewGuid().ToString(),
                floatEmbedding,
                metadata
            );
            points.Add(qdrantVector);
        }
        if (points.Any())
        {
            await qdrantClient.UpsertVectorsAsync(collectionName, points);
        }
    }
}
