using MongoDB.Bson.Serialization.Attributes;

namespace QuestionTask.Entities;

public class Question
{
    [BsonId]
    public Guid Id { get; set; }
    public required string Title { get; set; }
    public List<Choise> Choises { get; set; } = new List<Choise>();
    public string Photo { get; set; }
}

public class Choise
{
    public required string Text { get; set; }
    public bool IsAnswer { get; set; }
}

