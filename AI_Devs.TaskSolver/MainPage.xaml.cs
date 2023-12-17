using AI_Devs.TaskApp.Services.Interfaces;
using AI_Devs.TaskApp.Tasks;
using Microsoft.Extensions.Logging;
using OpenAI.Net;

namespace AI_Devs.TaskApp
{
    public partial class MainPage : ContentPage
    {
        private readonly ITaskService taskService;
        private readonly IOpenAIService openAIService;
        private readonly IFileService fileService;

        public MainPage()
        {
            InitializeComponent();
        }

        public MainPage(ITaskService taskService, IOpenAIService openAIService, IFileService fileService)
        {
            InitializeComponent();
            this.taskService = taskService;
            this.openAIService = openAIService;
            this.fileService = fileService;
        }

        private async void OnGetTaskContentClicked(object sender, EventArgs e)
        {
            SemanticScreenReader.Announce(GetTaskContentBtn.Text);

            var taskContent = await taskService.GetRawTaskContent(TaskName.Text);

            TaskInput.Text = taskContent;
        }

        private async void OnSendAnswerClicked(object sender, EventArgs e)
        {
            SemanticScreenReader.Announce(AnswerBtn.Text);

            var answerResponse = await taskService.SendAnswer(TaskName.Text, AnswerInput.Text);

            AnswerResponse.Text = answerResponse.Note;
        }

        private async void OnBloggerBtnClicked(object sender, EventArgs e)
        {
            SemanticScreenReader.Announce(BloggerBtn.Text);

            var blogger = new Blogger(openAIService, taskService);

            await blogger.PerformTask();
        }
        
        private async void OnLiarBtnClicked(object sender, EventArgs e)
        {
            SemanticScreenReader.Announce(LiarBtn.Text);

            var liar = new Liar(openAIService, taskService);

            await liar.PerformTask();
        }

        private async void OnInpromptBtnClicked(object sender, EventArgs e)
        {
            SemanticScreenReader.Announce(InpromptBtn.Text);

            var liar = new Inprompt(openAIService, taskService);

            await liar.PerformTask();
        }

        private async void OnEmbeddingBtnClicked(object sender, EventArgs e)
        {
            SemanticScreenReader.Announce(EmbeddingBtn.Text);

            var embedding = new Embedding(openAIService, taskService);

            await embedding.PerformTask();
        }

        private async void OnWhisperBtnClicked(object sender, EventArgs e)
        {
            SemanticScreenReader.Announce(EmbeddingBtn.Text);

            var whisper = new Whisper(openAIService, taskService, fileService);

            await whisper.PerformTask();
        }

        private async void OnFunctionsBtnClicked(object sender, EventArgs e)
        {
            SemanticScreenReader.Announce(FunctionsBtn.Text);

            var whisper = new Functions(openAIService, taskService);

            await whisper.PerformTask();
        }

        private async void OnRodoBtnClicked(object sender, EventArgs e)
        {
            SemanticScreenReader.Announce(RodoBtn.Text);

            var whisper = new Rodo(openAIService, taskService);

            await whisper.PerformTask();
        }
        private async void OnScraperBtnClicked(object sender, EventArgs e)
        {
            SemanticScreenReader.Announce(ScraperBtn.Text);

            var logger = this.Handler.MauiContext.Services.GetService<ILogger<Scraper>>();

            var scraper = new Scraper(openAIService, taskService, fileService, logger);

            await scraper.PerformTask();
        }

        private async void OnWhoamiBtnClicked(object sender, EventArgs e)
        {
            SemanticScreenReader.Announce(WhoamiBtn.Text);

            var whoami = new Whoami(openAIService, taskService);

            await whoami.PerformTask();
        }

        private async void OnSearchBtnClicked(object sender, EventArgs e)
        {
            SemanticScreenReader.Announce(WhoamiBtn.Text);

            var factory = this.Handler.MauiContext.Services.GetService<IHttpClientFactory>();

            var search = new Search(openAIService, taskService, factory);

            await search.PrepareTask(300);

            await search.PerformTask();
        }

        private async void OnPeopleBtnClicked(object sender, EventArgs e)
        {
            SemanticScreenReader.Announce(WhoamiBtn.Text);

            var factory = this.Handler.MauiContext.Services.GetService<IHttpClientFactory>();

            var search = new People(openAIService, taskService, factory);

            await search.PerformTask();
        }

        private async void OnToolsBtnClicked(object sender, EventArgs e)
        {
            SemanticScreenReader.Announce(WhoamiBtn.Text);

            var search = new Tools(openAIService, taskService);

            await search.PerformTask();
        }
    }
}