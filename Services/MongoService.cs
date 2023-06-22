using MongoDB.Driver;
using QuestionTask.Entities;

namespace QuestionTask.Services;

public class MongoService
{
    private IMongoClient mongo = new MongoClient("mongodb://elbek:elbek@localhost:27017");
    private IMongoDatabase database => mongo.GetDatabase("Tasklar");
    public IMongoCollection<Question> _questions => database.GetCollection<Question>("questions");
    public IMongoCollection<Result> _results => database.GetCollection<Result>("results");
}
