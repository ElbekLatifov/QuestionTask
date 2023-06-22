using MongoDB.Driver;
using QuestionTask.Entities;
using QuestionTask.Models;
using QuestionTask.Services;

namespace QuestionTask.Managers;

public class QuestionManager
{
    private readonly FileService fileService;

    public QuestionManager(FileService fileService)
    {
        this.fileService = fileService;
    }
    private IMongoClient mongo = new MongoClient("mongodb://elbek:elbek@localhost:27017");
    private IMongoDatabase database => mongo.GetDatabase("Tasklar");
    public IMongoCollection<Question> _questions => database.GetCollection<Question>("questions");
    public IMongoCollection<Result> _results => database.GetCollection<Result>("results");

    public async Task<List<Question>> GetQuestions()
    {
        var questions = await (await _questions.FindAsync(_ => true)).ToListAsync();
        return questions;
    }

    public async Task<Question> GetQuestionById(Guid questionid)
    {
        var question = await (await _questions.FindAsync(p=>p.Id == questionid)).FirstOrDefaultAsync();

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

        await _questions.InsertOneAsync(question);

        return question;
    }

    public async Task<string> AddPhotoToQuestion(Guid questionId, IFormFile photo)
    {
        var filter = Builders<Question>.Filter.Eq(p => p.Id, questionId);
        var question = await (await _questions.FindAsync(p=>p.Id == questionId)).FirstOrDefaultAsync();

        if (question == null)
        {
            throw new Exception("     Takoy question no exist     ");
        }

        question.Photo = await fileService.SaveQuestionPhoto(photo);
        await _questions.ReplaceOneAsync(filter, question);

        return question.Photo;
    }

    public async Task<Question> UpdateQuestion(Guid questionId, QuestionModel model)
    {
        var filter = Builders<Question>.Filter.Eq(p => p.Id, questionId);
        var question = await (await _questions.FindAsync(p => p.Id == questionId)).FirstOrDefaultAsync();

        if (question == null)
        {
            throw new Exception("     Takoy question no exist     ");
        }

        question.Title = model.Title;
        question.Choises = model.Choises;

        await _questions.ReplaceOneAsync(filter, question);

        return question;
    }

    public async Task DeleteQuestion(Guid questionId)
    {
        var filter = Builders<Question>.Filter.Eq(p => p.Id, questionId);
        var question = await (await _questions.FindAsync(p => p.Id == questionId)).FirstOrDefaultAsync();

        if (question == null)
        {
            throw new Exception("     Takoy question no exist     ");
        }

        await _questions.DeleteOneAsync(filter);
    }

    public async Task<Result> GetResult(long userid)
    {
        var result = await(await _results.FindAsync(p=>p.UserId == userid)).FirstOrDefaultAsync();

        return result;
    }

    public async Task PlusResult(long userid)
    {
        var result = await (await _results.FindAsync(p => p.UserId == userid)).FirstOrDefaultAsync();
        result.CorrectCount++;
        result.TotalQuestionsCount++;
        await _results.ReplaceOneAsync(p=>p.UserId == userid, result);    
    }

    public async Task MinusResult(long userid)
    {
        var result = await (await _results.FindAsync(p => p.UserId == userid)).FirstOrDefaultAsync();
        result.InCorrectCount++;
        result.TotalQuestionsCount++;
        await _results.ReplaceOneAsync(p => p.UserId == userid, result);
    }

    public async Task<string> CheckAnswer(long userid, string[] answer)
    {
        var result = await (await _results.FindAsync(p => p.UserId == userid)).FirstOrDefaultAsync();

        var question = await(await _questions.FindAsync(p=>p.Id == Guid.Parse(answer[0]))).FirstOrDefaultAsync();

        var correct = question.Choises.FirstOrDefault(p => p.IsAnswer)!.Text;
        

        if(correct == answer[1].ToString())
        {
            await PlusResult(userid);
            return "Correct";
        }
        else
        {
            await MinusResult(userid);
            return "Incorrect";
        }
    }

    public async Task GetUser(long userid)
    {
        var result = await (await _results.FindAsync(p => true)).ToListAsync();

        if(!result.Any(p => p.UserId == userid))
        {
            var b = new Result()
            {
                UserId = userid
            };

            await _results.InsertOneAsync(b);
        }
    }
}
