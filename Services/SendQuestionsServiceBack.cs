using Microsoft.AspNetCore.Components.Forms;
using MongoDB.Bson;
using MongoDB.Driver;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace QuestionTask.Services;

public class SendQuestionsServiceBack : BackgroundService
{
    private readonly MongoService _service;

    public SendQuestionsServiceBack(MongoService service)
    {
        _service = service;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var timer = new Timer(Tick,null, 0, 10000);
        return Task.CompletedTask;
    }

    private async void Tick(object? sender)
    {
        string botToken = "6034108179:AAE0JCP4SdwliEATcogsAFFszPa8yblcEcg";
        var botClient = new TelegramBotClient(botToken);
        string chatId = "2088380689";
        

        var random = new Random();
        var questions = (_service._questions.Find(_ => true)).ToList();
        string messageText = "Questions no exist";

        if (questions != null)
        {
            var index = random.Next(0, questions.Count);
            var question = questions[index];
            string messageChoises = "";
            if(question.Choises != null)
            {
                foreach (var choise in question.Choises)
                {
                    messageChoises = messageChoises + $" - {choise.Text}\n";
                    messageText = $"Savol : {question.Title} ?\n" + messageChoises;
                }
            }
            else
            {
                messageText = $"Savol : {question.Title} ?\n - Variantlar yo'q";
            }
        }    


        //InlineKeyboardMarkup CreateQuestionChoiceButtons(int index, int? choiseIndex = null, bool? answer = false)
        //{
        //    var choisesButtons = new List<List<InlineKeyboardButton>>();
        //    char harf = 'A';
        //    for (int i = 0; i < question.Choises!.Count; i++)
        //    {
        //        var choiseButtons = new List<InlineKeyboardButton>()
        //        {
        //            InlineKeyboardButton.WithCallbackData($"{harf}", $"{index},{i}")
        //        };
        //        choisesButtons.Add(choiseButtons);
        //        harf++;
        //    }
        //    return new InlineKeyboardMarkup(choisesButtons);
        //}


        

        //var fileBytes = System.IO.File.ReadAllBytes($"{question.Photo}");
        //var ms = new MemoryStream(fileBytes);
        //await botClient.SendPhotoAsync(chatId: chatId, photo: new InputOnlainFile(ms), 
        //    caption: messageText, replyMarkup: CreateQuestionChoiceButtons(index));

        //var photoStream = ms;
        //await botClient.SendPhotoAsync(chatId, new InputMedia(ms), messageText);

        string apiUrl = $"https://api.telegram.org/bot{botToken}/sendMessage";

        // Set up the data payload for the POST request
        var data = new FormUrlEncodedContent(new[]
        {
                new KeyValuePair<string, string>("chat_id", chatId),
                new KeyValuePair<string, string>("text", messageText)
            });

        // Send the POST request using HttpClient
        using (var client = new HttpClient())
        {
            var response = await client.PostAsync(apiUrl, data);

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Message sent successfully!");
            }
            else
            {
                Console.WriteLine($"Error sending message: {response.StatusCode} - {response.ReasonPhrase}");
            }
        }
    }
}
