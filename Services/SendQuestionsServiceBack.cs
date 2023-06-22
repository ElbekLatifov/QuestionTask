using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using QuestionTask.Entities;
using QuestionTask.Managers;
using System;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using InputFile = Telegram.Bot.Types.InputFile;

namespace QuestionTask.Services;

public class SendQuestionsServiceBack : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;

    public SendQuestionsServiceBack(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        //var timer = new Timer(Tick, null, 0, 5000);
        while (true)
        {
            await SendMessage();
            await Task.Delay(TimeSpan.FromMinutes(1));
        }
    }

    public async Task SendMessage()
    {
        string botToken = "6034108179:AAE0JCP4SdwliEATcogsAFFszPa8yblcEcg";
        var botClient = new TelegramBotClient(botToken);
        string chatId = "2088380689";

        using (var scope = _serviceProvider.CreateScope())
        {
            var questionManager = scope.ServiceProvider.GetRequiredService<QuestionManagerr>();
            await questionManager.Get(botClient, long.Parse(chatId));
        }
    }

    //private async Task GetQuestionsForHour()
    //{
    //    string botToken = "6034108179:AAE0JCP4SdwliEATcogsAFFszPa8yblcEcg";
    //    var botClient = new TelegramBotClient(botToken);
    //    string chatId = "2088380689";

    //    using (var scope = _serviceProvider.CreateScope())
    //    {
    //        var mongoManager = scope.ServiceProvider.GetRequiredService<MongoService>();
    //        IMongoCollection<Question> questionColl = mongoManager._questions;


    //        var random = new Random();

    //        var questions = (questionColl.Find(_ => true)).ToList();

    //        string messageText = "Questions no exist";

    //        if (questions != null)
    //        {
    //            var index = random.Next(0, questions.Count);
    //            var question = questions[index];
    //            string messageChoises = "";

    //            if (question.Choises != null)
    //            {
    //                foreach (var choise in question.Choises)
    //                {
    //                    messageChoises = messageChoises + $" - {choise.Text}\n";
    //                    messageText = $"Savol : {question.Title} ?\n" + messageChoises;
    //                }
    //            }
    //            else
    //            {
    //                messageText = $"Savol : {question.Title} ?\n - Variantlar yo'q";
    //            }

    //            if (question.Photo != null)
    //            {
    //                var fileBytes = System.IO.File.ReadAllBytes($"{question.Photo}");
    //                var ms = new MemoryStream(fileBytes);
    //                Message message = await botClient.SendPhotoAsync(
    //                photo: InputFile.FromStream(ms),
    //                caption: messageText,
    //                chatId: chatId,
    //                replyMarkup: CreateQuestionChoiceButtons(index));
    //            }
    //            else
    //            {
    //                await botClient.SendTextMessageAsync(chatId, messageText, replyMarkup: CreateQuestionChoiceButtons(index));
    //            }

    //            InlineKeyboardMarkup CreateQuestionChoiceButtons(int index)
    //            {
    //                var choisesButtons = new List<List<InlineKeyboardButton>>();

    //                if(question.Choises != null)
    //                {
    //                    for (int i = 0; i < question.Choises!.Count; i++)
    //                    {
    //                        var choiseButtons = new List<InlineKeyboardButton>()
    //                        {
    //                            InlineKeyboardButton.WithCallbackData($"{question.Choises[i]}", $"{question.Choises[i].IsAnswer}")
    //                        };
    //                        choisesButtons.Add(choiseButtons);
    //                    }
    //                }
    //                else
    //                {
    //                    var choiseButtons = new List<InlineKeyboardButton>()
    //                    {
    //                        InlineKeyboardButton.WithCallbackData("Choises no", $"?")
    //                    };
    //                    choisesButtons.Add(choiseButtons);
    //                }

    //                return new InlineKeyboardMarkup(choisesButtons);

    //            }
    //        }
    //    }
    //}

    //private async void Tick(object? sender)
    //{
    //    string botToken = "6034108179:AAE0JCP4SdwliEATcogsAFFszPa8yblcEcg";
    //    var botClient = new TelegramBotClient(botToken);
    //    string chatId = "2088380689";


    //    string apiUrl = $"https://api.telegram.org/bot{botToken}/sendMessage";

    //    var data = new FormUrlEncodedContent(new[]
    //    {
    //            new KeyValuePair<string, string>("chat_id", chatId),
    //            new KeyValuePair<string, string>("text", "uzur")
    //    });

    //    using (var client = new HttpClient())
    //    {
    //        var response = await client.PostAsync(apiUrl, data);

    //        if (response.IsSuccessStatusCode)
    //        {
    //            Console.WriteLine("Message sent successfully!");
    //        }
    //        else
    //        {
    //            Console.WriteLine($"Error sending message: {response.StatusCode} - {response.ReasonPhrase}");
    //        }
    //    }
    //}
}
