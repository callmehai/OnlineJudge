namespace OnlineJudge.Models
{
  public class Submission
  {
    public int Id { get; set; }

    public int ProblemId { get; set; }

    public string Language { get; set; } = "";

    public string Code { get; set; } = "";

    public string Result { get; set; } = "";

    public DateTime CreatedAt { get; set; }
  }
}
