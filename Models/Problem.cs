namespace OnlineJudge.Models;

public class Problem
{
  public int Id { get; set; }

  public string Title { get; set; } = "";

  public string Description { get; set; } = "";

  public int TimeLimitMs { get; set; }

  public int MemoryLimitMb { get; set; }
}
