using Mapster;
using MongoDB.Driver;
using QuestionTask.Entities;
using QuestionTask.Models;
using QuestionTask.Services;

namespace QuestionTask.Managers;

public class QuestionManager
{
    private MongoService _mongoService;

    public QuestionManager(MongoService mongoService)
    {
        _mongoService = mongoService;
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

        question.Photo = FileService.SaveQuestionPhoto(photo);
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
}
