using MongoDB.Driver;
using QuestionTask.Entities;

namespace QuestionTask.Services;

public class ResultsService
{
    private IMongoClient mongo = new MongoClient("mongodb://elbek:elbek@localhost:27017");
    private IMongoDatabase database => mongo.GetDatabase("Tasklar");
    public IMongoCollection<Question> _questions => database.GetCollection<Question>("questions");
    public IMongoCollection<Result> _results => database.GetCollection<Result>("results");


    public async Task<List<Result>> GEtResultsAsync()
    {
        var results = await (await _results.FindAsync(_ => true)).ToListAsync();
        return results;
    }


}
