using Mapster;
using MongoDB.Driver;
using QuestionTask.Entities;
using QuestionTask.Models;
using QuestionTask.Services;

namespace QuestionTask.Managers;

public class QuestionManager
{
    private readonly FileService fileService;
    private MongoService _mongoService;

    public QuestionManager(MongoService mongoService, FileService fileService)
    {
        _mongoService = mongoService;
        this.fileService = fileService;
    }

    public async Task<List<Question>> GetQuestions()
    {
        var questions = await (await _mongoService._questions.FindAsync(_ => true)).ToListAsync();
        return questions;
    }

    public async Task<Question> GetQuestionById(Guid questionid)
    {
        var question = await (await _mongoService._questions.FindAsync(p=>p.Id == questionid)).FirstOrDefaultAsync();

        if (question == null)
        {
            throw new ArgumentException(" no such question yet ");
        }

        return question;
    }

    public async Task<Question> CreateQuestion(QuestionModel model)
    {
        var question = new Question()
        {
            Title = model.Title,
            Choises = model.Choises
        };

        await _mongoService._questions.InsertOneAsync(question);

        return question;
    }

    public async Task<string> AddPhotoToQuestion(Guid questionId, IFormFile photo)
    {
        var filter = Builders<Question>.Filter.Eq(p => p.Id, questionId);
        var question = await (await _mongoService._questions.FindAsync(p=>p.Id == questionId)).FirstOrDefaultAsync();

        if (question == null)
        {
            throw new Exception("     Takoy question no exist     ");
        }

        question.Photo = await fileService.SaveQuestionPhoto(photo);
        await _mongoService._questions.ReplaceOneAsync(filter, question);

        return question.Photo;
    }

    public async Task<Question> UpdateQuestion(Guid questionId, QuestionModel model)
    {
        var filter = Builders<Question>.Filter.Eq(p => p.Id, questionId);
        var question = await (await _mongoService._questions.FindAsync(p => p.Id == questionId)).FirstOrDefaultAsync();

        if (question == null)
        {
            throw new Exception("     Takoy question no exist     ");
        }

        question.Title = model.Title;
        question.Choises = model.Choises;

        await _mongoService._questions.ReplaceOneAsync(filter, question);

        return question;
    }

    public async Task DeleteQuestion(Guid questionId)
    {
        var filter = Builders<Question>.Filter.Eq(p => p.Id, questionId);
        var question = await (await _mongoService._questions.FindAsync(p => p.Id == questionId)).FirstOrDefaultAsync();

        if (question == null)
        {
            throw new Exception("     Takoy question no exist     ");
        }

        await _mongoService._questions.DeleteOneAsync(filter);
    }

    public async Task<Result> GetResult(long userid)
    {
        var result = await(await _mongoService._results.FindAsync(p=>p.UserId == userid)).FirstOrDefaultAsync();

        return result;
    }

    public async Task PlusResult(long userid)
    {
        var result = await (await _mongoService._results.FindAsync(p => p.UserId == userid)).FirstOrDefaultAsync();
        result.CorrectCount++;
        await _mongoService._results.ReplaceOneAsync(p=>p.UserId == userid, result);    
    }

    public async Task MinusResult(long userid)
    {
        var result = await (await _mongoService._results.FindAsync(p => p.UserId == userid)).FirstOrDefaultAsync();
        result.InCorrectCount++;
        await _mongoService._results.ReplaceOneAsync(p => p.UserId == userid, result);
    }

    public async Task CheckAnswer(long userid, string[] answer)
    {
        var result = await (await _mongoService._results.FindAsync(p => p.UserId == userid)).FirstOrDefaultAsync();

        var question = await(await _mongoService._questions.FindAsync(p=>p.Id == Guid.Parse(answer[0]))).FirstOrDefaultAsync();

        if (question.Choises[int.Parse(answer[1])].IsAnswer)
        {
            await PlusResult(userid);
        }
        else
        {
            await MinusResult(userid);
        }
    }

    public async Task GetUser(long userid)
    {
        var result = await (await _mongoService._results.FindAsync(p => true)).ToListAsync();

        if(!result.Any(p => p.UserId == userid))
        {
            var b = new Result()
            {
                UserId = userid
            };

            await _mongoService._results.InsertOneAsync(b);
        }
    }
}
