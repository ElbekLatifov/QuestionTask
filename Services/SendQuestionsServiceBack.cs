﻿using Microsoft.AspNetCore.Identity;
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
            await Task.Delay(TimeSpan.FromSeconds(3));
        }
    }

    public async Task SendMessage()
    {
        string botToken = "6034108179:AAE0JCP4SdwliEATcogsAFFszPa8yblcEcg";
        var botClient = new TelegramBotClient(botToken);

        using (var scope = _serviceProvider.CreateAsyncScope())
        {
            var questionManager = scope.ServiceProvider.GetRequiredService<QuestionManager2>();
            //var resultService = scope.ServiceProvider.GetRequiredService<ResultsService>();

            //var results = await resultService.GEtResultsAsync();
            await questionManager.Get(botClient);

            //if (results is not null)
            //{
            //    foreach (var item in results)
            //    {
            //    }
            //}
        }
        
        
        
    }

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
