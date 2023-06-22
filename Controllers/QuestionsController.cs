using Microsoft.AspNetCore.Mvc;
using QuestionTask.Managers;
using QuestionTask.Models;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace QuestionTask.Controllers;

[Route("api/[controller]")]
[ApiController]
public class QuestionsController : ControllerBase
{
    private readonly QuestionManager questionManager;

    public QuestionsController(QuestionManager questionManager)
    {
        this.questionManager = questionManager;
    }

    [HttpGet]
    public async Task<IActionResult> GetQuestions()
    {
        var questions = await questionManager.GetQuestions();
        return Ok(questions);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] QuestionModel model)
    {
        var question = await questionManager.CreateQuestion(model); 
        return Ok(question);
    }

    [HttpPost("AddphotoToQuestion")]
    public async Task<IActionResult> AddPhoto([FromForm] Media photo)
    {
        var path = await questionManager.AddPhotoToQuestion(photo.QuestionId, photo.Photo);
        return Ok(path);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var question = await questionManager.GetQuestionById(id);
        return Ok(question);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateQuestion(Guid id, [FromBody] QuestionModel model)
    {
        var question = await questionManager.UpdateQuestion(id, model);
        return Ok(question);
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteQuestion(Guid id)
    {
        await questionManager.DeleteQuestion(id);
        return Ok("Deleted");
    }

    [HttpPost("/bot")]
    public async Task GetUpdate([FromBody] Update update)
    {
        var bot = new TelegramBotClient("6034108179:AAE0JCP4SdwliEATcogsAFFszPa8yblcEcg");

        if(update.Type == Telegram.Bot.Types.Enums.UpdateType.Message)
        {
            var javob = update.Message!.Text;

            await bot.SendTextMessageAsync(update.Message!.From!.Id, "For You Question in hour");

            await questionManager.GetUser(update.Message!.From!.Id);

            if(javob == "/result")
            {
                var result = await questionManager.GetResult(update.Message!.From!.Id);

                await bot.SendTextMessageAsync(update.Message!.From!.Id, $"Corrects: {result.CorrectCount}, InCorrects: {result.InCorrectCount}");
            }
        }

        if (update.Type == Telegram.Bot.Types.Enums.UpdateType.CallbackQuery)
        {
            var javob = update.CallbackQuery!.Data;
            
            string[] data = javob!.Split(',').ToArray();

            await questionManager.CheckAnswer(update.CallbackQuery.From.Id, data);
        }
    }
}
