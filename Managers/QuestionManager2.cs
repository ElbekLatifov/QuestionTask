using MongoDB.Driver;
using QuestionTask.Entities;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace QuestionTask.Services;

public class QuestionManager2
{
    private IMongoClient mongo = new MongoClient("mongodb://elbek:elbek@localhost:27017");
    private IMongoDatabase database => mongo.GetDatabase("Tasklar");
    public IMongoCollection<Question> _questions => database.GetCollection<Question>("questions");
    public IMongoCollection<Result> _results => database.GetCollection<Result>("results");

    public async Task Get(ITelegramBotClient bot)
    {
        var questions = await (await _questions.FindAsync(_ => true)).ToListAsync();

        var results = await (await _results.FindAsync(_ => true)).ToListAsync();

        if (results != null)
        {
            foreach (var result in results)
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
                        await bot.SendPhotoAsync(
                        photo: InputFile.FromStream(ms),
                        caption: messageText,
                        chatId: result.UserId,
                        replyMarkup: CreateQuestionChoiceButtons(index));
                    }
                    else
                    {
                        await bot.SendTextMessageAsync(result.UserId, messageText, 
                            replyMarkup: CreateQuestionChoiceButtons(index));
                    }
                }
            }
        }

        InlineKeyboardMarkup CreateQuestionChoiceButtons(int index)
        {
            var choisesButtons = new List<List<InlineKeyboardButton>>();

            for (int i = 0; i < questions[index].Choises!.Count; i++)
            {
                var choiseButtons = new List<InlineKeyboardButton>()
                            {
                                InlineKeyboardButton.WithCallbackData($"{questions[index].Choises[i].Text}",
                                $"{questions[index].Id},{questions[index].Choises[i].Text}")
                            };
                choisesButtons.Add(choiseButtons);
            }

            return new InlineKeyboardMarkup(choisesButtons);
        }
    }
}
