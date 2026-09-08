namespace Application.Questions;

public class QuestionItem
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public QuestionItemStatus Status { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? DueDate { get; set; }
}