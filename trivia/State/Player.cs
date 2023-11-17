namespace trivia.State;

public class Player
{
    public string Code { get; set; } = default!;
    public string Name { get; set; } = default!;
    public int Score { get; set; }
    public List<string> Categories { get; set; } = new();// new List<string>() { "SPORT", "GAMING", "DESIGN" };
    public Pages Page { get; set; } = Pages.NamePicker;
}
