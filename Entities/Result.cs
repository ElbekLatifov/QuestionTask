using MongoDB.Bson.Serialization.Attributes;

namespace QuestionTask.Entities;

public class Result
{
    [BsonId]
    public Guid Id { get; set; }
    public long UserId { get; set; }
    public int TotalQuestionsCount { get; set; }
    public int CorrectCount { get; set; }
    public int InCorrectCount { get; set; }
}
