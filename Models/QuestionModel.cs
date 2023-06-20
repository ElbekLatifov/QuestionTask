using QuestionTask.Entities;

namespace QuestionTask.Models;

public class QuestionModel
{
    public string Title { get; set; }
    public List<Choise> Choises { get; set; } = new List<Choise>();
}

public class Media
{
    public Guid QuestionId { get; set; }
    public IFormFile Photo { get; set; }
}
