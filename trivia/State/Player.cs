namespace trivia.State;

public class Player
{
    public string Code { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string? Team { get; set; }
    public int Score { get; set; }
    public List<string> Categories { get; set; } = new List<string>(); // { "SPORT", "GAMING", "DESIGN" };
    public Pages Page { get; set; } = Pages.Rules;
    public Pages? PreviousPage { get; set; }
    public Dictionary<int, string> Answers { get; set; } = new Dictionary<int, string>();
}
