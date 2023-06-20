using MongoDB.Driver;
using QuestionTask.Entities;

namespace QuestionTask.Services;

public class MongoService
{
    private IMongoClient mongo = new MongoClient("mongodb://elbek:elbek@mongo_db:27017");
    private IMongoDatabase database => mongo.GetDatabase("Tasks");
    public IMongoCollection<Question> _questions => database.GetCollection<Question>("questions");

}
