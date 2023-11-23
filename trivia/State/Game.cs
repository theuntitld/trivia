namespace trivia.State;

public class Game
{
    public Game ()
    {
        var engine = new FileHelpers.FileHelperEngine<QuestionCSVModel>();

        var allQuestions = engine.ReadFile($"State/questions.csv").ToList();

        Categories = allQuestions.Select(x => x.Category).Distinct().ToList();
    }

    public Score WisdomOfTheCrowd { get; set; } = new Score();
    public int Stage { get; set; } = -1;
    public bool QuestionIsShowing { get; set; }
    public bool CorrectAnswerIsShowing { get; set; }
    public List<Player> Players { get; set; } = new List<Player>();
    public EventHandler StateChange { get; set; } = default!;
    public void StateChanged() => StateChange?.Invoke(this, EventArgs.Empty);

    public int NumberOfCategoriesPerUser { get; set; } = 3;
    public int SecondsPerStage { get; set; } = 12;

    public int QuestionsPerGame { get; set; } = 25;

    public int RemainingSecondForStage { get; set; }

    public List<QuestionCSVModel> Questions { get; set; } = new List<QuestionCSVModel> { };

    private string? LastAssignedTeam { get; set; }

    public List<string> Teams = new List<string>
    {
        "GREEN",
        "RED",
        "BLUE",
        "YELLOW",
        "PINK",
        "PURPULE",
        "ORANGE",
    };

    public Dictionary<string, Score> TeamScore { get; set; } = new Dictionary<string, Score>
    {
        ["GREEN"] = new Score(),
        ["RED"] = new Score(),
        ["BLUE"] = new Score(),
        ["YELLOW"] = new Score(),
        ["PINK"] = new Score(),
        ["PURPULE"] = new Score(),
        ["ORANGE"] = new Score(),
    };
   
    public List<string> Categories;

    public void AddPlayer(Player player)
    {
        if (!Players.Any(x => x.Code == player.Code))
        {
            this.Players.Add(player);
        }

        StateChanged();
    }

    public void SetOnlineStatus(Player player, bool online)
    {
        player.Online = online;

        StateChanged();
    }

    public void AssignTeam(Player player)
    {
        if (player.Team is not null)
            return;

        var teamIndexToAssign = LastAssignedTeam is null ? 0 : Teams.IndexOf(LastAssignedTeam) + 1;

        if (teamIndexToAssign == Teams.Count())
            teamIndexToAssign = 0;

        var teamToAssign = Teams.ElementAt(teamIndexToAssign);

        player.Team = teamToAssign;

        LastAssignedTeam = teamToAssign;

        this.StateChanged();
    }

    public void RemovePlayer(Player? player)
    {
        if (player is not null && this.Players.Contains(player))
            this.Players.Remove(player);

        this.StateChanged();
    }

    public void ToggleCategory(Player player, string category)
    {
        if (player.Categories.Contains(category))
            player.Categories.Remove(category);
        else
            player.Categories.Add(category);

        if (player.Categories.Count > this.NumberOfCategoriesPerUser)
        {
            player.Categories.Remove(player.Categories.First());
        }

        StateChanged();
    }

    public void AnswerQuestion(Player player, int stage, string answer)
    {
        if (this.Stage != stage || this.RemainingSecondForStage <= 0)
            return;

        player.Answers[stage] = answer;
    }

    private async Task StartStageTimer()
    {
        while (this.RemainingSecondForStage > 0)
        {
            this.StateChanged();

            await Task.Delay(1000);

            if (this.RemainingSecondForStage == 0)
                break;

            this.RemainingSecondForStage--;

            foreach (var team in TeamScore.Keys)
                TeamScore[team] = new Score();

            this.WisdomOfTheCrowd = new Score();

            if (this.RemainingSecondForStage == 0)
            {
                this.CorrectAnswerIsShowing = true;

                foreach (var player in Players)
                {
                    var playerFlatScore = 0;
                    var playerScoreWithPanelty = 0;

                    for (int i = 0; i <= this.Stage; i++)
                    {
                        if (player.Answers.ContainsKey(i))
                        {
                            //Correct
                            if (player.Answers[i] == this.Questions.ElementAt(i).Answer)
                            {
                                playerFlatScore = playerFlatScore + 123;
                                playerScoreWithPanelty = playerScoreWithPanelty + 123;
                            }
                            //Incorrect
                            else
                            {
                                playerScoreWithPanelty = playerScoreWithPanelty - 63;
                            }
                        }
                        //Skipped
                        else
                        {
                            playerScoreWithPanelty = playerScoreWithPanelty - 18;
                        }
                    }

                    player.Score.FlatScore = playerFlatScore;
                    player.Score.ScoreWithPanelty = playerScoreWithPanelty;

                    if (player.Team is not null)
                    {
                        TeamScore[player.Team!].FlatScore += player.Score.FlatScore;
                        TeamScore[player.Team!].ScoreWithPanelty += player.Score.ScoreWithPanelty;
                    }

                    this.WisdomOfTheCrowd.FlatScore += player.Score.FlatScore;
                    this.WisdomOfTheCrowd.ScoreWithPanelty += player.Score.ScoreWithPanelty;
                }
            }
        }

        this.StateChanged();
    }

    public void StopGame()
    {
        this.RemainingSecondForStage = 0;
        this.Stage = -1;
        this.QuestionIsShowing = false;

        this.StateChanged();
    }

    public void RevealQuestion()
    {
        this.QuestionIsShowing = true;

        this.CorrectAnswerIsShowing = false;

        if (this.Stage == -1)
        {
            this.Questions = this.GetGameQuestions();
        }

        this.Stage++;

        this.StateChanged();
    }

    public async Task StartStage()
    {
        this.QuestionIsShowing = false;

        this.CorrectAnswerIsShowing = false;

        this.RemainingSecondForStage = this.SecondsPerStage;

        await this.StartStageTimer();
    }

    public void FinishTheGame()
    {
        if (this.Stage == this.Questions.Count - 1)
            this.Stage++;

        this.StateChanged();
    }

    public List<QuestionCSVModel> GetGameQuestions()
    {
        var engine = new FileHelpers.FileHelperEngine<QuestionCSVModel>();

        var allQuestions = engine.ReadFile($"State/questions.csv").ToList();

        var categoryCounts = this.Players
        .SelectMany(x => x.Categories)
        .GroupBy(x => x)
        .ToDictionary(x => x.Key.ToUpper(), x => x.Count());

        Random rng = new Random();

        var filteredOrderedQuestions = allQuestions
        // .Where(x => categoryCounts.Keys.Contains(x.Category.ToUpper()))
        .OrderByDescending(x => !categoryCounts.ContainsKey(x.Category.ToUpper()) ? -1 : categoryCounts[x.Category.ToUpper()])
        // .Take(3 * 3)
        .Take(this.QuestionsPerGame)
        .ToList()
        .OrderBy(x => rng.Next())
        .ToList();

        return filteredOrderedQuestions;
    }
}
