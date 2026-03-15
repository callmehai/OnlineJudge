namespace OnlineJudge.Models
{
  public class TestCase
  {
    public int Id { get; set; }

    public int ProblemId { get; set; }

    public string Input { get; set; } = "";

    public string Output { get; set; } = "";

    public Problem? Problem { get; set; }
  }
}
