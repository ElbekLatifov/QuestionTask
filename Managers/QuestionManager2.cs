using MongoDB.Driver;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace QuestionTask.Services;

public class QuestionManager2
{
    private readonly MongoService _mongoService;

    public QuestionManager2(MongoService mongoService)
    {
        _mongoService = mongoService;
    }

    public async Task Get(ITelegramBotClient bot)
    {
        var questions = await (await _mongoService._questions.FindAsync(_ => true)).ToListAsync();
        var results = await (await _mongoService._results.FindAsync(p => true)).ToListAsync();

        if(results is not null)
        {
            foreach( var result in results)
            {
                var random = new Random();

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
                        chatId: result.UserId,
                        replyMarkup: CreateQuestionChoiceButtons());
                    }
                    else
                    {
                        await bot.SendTextMessageAsync(result.UserId, messageText, replyMarkup: CreateQuestionChoiceButtons());
                    }

                    InlineKeyboardMarkup CreateQuestionChoiceButtons()
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
    }
}
