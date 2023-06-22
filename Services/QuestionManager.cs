using Microsoft.AspNetCore.Components.Forms;
using MongoDB.Driver;
using QuestionTask.Entities;
using System;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace QuestionTask.Services;

public class QuestionManagerr
{
    private readonly MongoService _mongoService;

    public QuestionManagerr(MongoService mongoService)
    {
        _mongoService = mongoService;
    }

    public async Task Get(ITelegramBotClient bot, long chatId)
    {
        var questionColl = _mongoService._questions;

        await bot.SendTextMessageAsync(chatId, "Salom");

        var random = new Random();

        var questions = (questionColl.Find(_ => true)).ToList();

        string messageText = "Questions no exist";

        if (questions != null)
        {
            var index = random.Next(0, questions.Count);
            var question = questions[index];
            messageText = $"   Savol : {question.Title} ?  ";

            if (question.Photo != null)
            {
                var fileBytes = System.IO.File.ReadAllBytes($"wwwroot{question.Photo}");
                var ms = new MemoryStream(fileBytes);
                Message message = await bot.SendPhotoAsync(
                photo: Telegram.Bot.Types.InputFile.FromStream(ms),
                caption: messageText,
                chatId: chatId,
                replyMarkup: CreateQuestionChoiceButtons(index));
            }
            else
            {
                await bot.SendTextMessageAsync(chatId, messageText, replyMarkup: CreateQuestionChoiceButtons(index));
            }

            InlineKeyboardMarkup CreateQuestionChoiceButtons(int index)
            {
                var choisesButtons = new List<List<InlineKeyboardButton>>();

                if (question.Choises != null)
                {
                    for (int i = 0; i < question.Choises!.Count; i++)
                    {
                        var choiseButtons = new List<InlineKeyboardButton>()
                                {
                                    InlineKeyboardButton.WithCallbackData($"{question.Choises[i].Text}",
                                    $"{question.Id},{question.Choises[i].Id}")
                                };
                        choisesButtons.Add(choiseButtons);
                    }
                }
                else
                {
                    var choiseButtons = new List<InlineKeyboardButton>()
                    {
                        InlineKeyboardButton.WithCallbackData("Choises no", "?")
                    };
                    choisesButtons.Add(choiseButtons);
                }

                return new InlineKeyboardMarkup(choisesButtons);

            }
        }
    }
}
