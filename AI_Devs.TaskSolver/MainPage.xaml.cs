using AI_Devs.TaskApp.Services.Interfaces;
using AI_Devs.TaskApp.Tasks;
using OpenAI.Net;

namespace AI_Devs.TaskApp
{
    public partial class MainPage : ContentPage
    {
        int count = 0;
        private readonly ITaskService taskService;

        public MainPage()
        {
            InitializeComponent();
        }

        public MainPage(ITaskService taskService)
        {
            InitializeComponent();
            this.taskService = taskService;
        }

        private async void OnCounterClicked(object sender, EventArgs e)
        {
            SemanticScreenReader.Announce(CounterBtn.Text);

            var localServices = this.Handler.MauiContext.Services.GetServices<ITaskService>();
            var localTaskService = localServices.First();
            //var taskContent = await localTaskService.GetTaskContent(TaskName.Text);
            var taskContent = await localTaskService.GetRawTaskContent(TaskName.Text);

            //TaskContent.Text = taskContent?.Msg;
            TaskInput.Text = taskContent;
        }

        private async void OnSendAnswerClicked(object sender, EventArgs e)
        {
            SemanticScreenReader.Announce(AnswerBtn.Text);

            var localServices = this.Handler.MauiContext.Services.GetServices<ITaskService>();
            var localTaskService = localServices.First();
            var answerResponse = await localTaskService.SendAnswer(TaskName.Text, AnswerInput.Text);

            AnswerResponse.Text = answerResponse.Note;
        }

        private async void OnBloggerBtnClicked(object sender, EventArgs e)
        {
            SemanticScreenReader.Announce(BloggerBtn.Text);

            var taskService = this.Handler.MauiContext.Services.GetServices<ITaskService>().First();
            var openAIService = this.Handler.MauiContext.Services.GetServices<IOpenAIService>().First();

            var blogger = new Blogger(openAIService, taskService);

            await blogger.PerformTask();
        }
        
        private async void OnLiarBtnClicked(object sender, EventArgs e)
        {
            SemanticScreenReader.Announce(LiarBtn.Text);

            var taskService = this.Handler.MauiContext.Services.GetServices<ITaskService>().First();
            var openAIService = this.Handler.MauiContext.Services.GetServices<IOpenAIService>().First();

            var liar = new Liar(openAIService, taskService);

            await liar.PerformTask();
        }

        private async void OnInpromptBtnClicked(object sender, EventArgs e)
        {
            SemanticScreenReader.Announce(InpromptBtn.Text);

            var taskService = this.Handler.MauiContext.Services.GetServices<ITaskService>().First();
            var openAIService = this.Handler.MauiContext.Services.GetServices<IOpenAIService>().First();

            var liar = new Inprompt(openAIService, taskService);

            await liar.PerformTask();
        }
    }
}