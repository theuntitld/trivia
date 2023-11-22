namespace trivia.State;

public class Game
{
    public int Stage { get; set; } = -1;
    public bool QuestionIsShowing { get; set; }
    public bool CorrectAnswerIsShowing { get; set; }
    public List<Player> Players { get; set; } = new List<Player>();
    public EventHandler StateChange { get; set; } = default!;
    public void StateChanged() => StateChange?.Invoke(this, EventArgs.Empty);

    public int NumberOfCategoriesPerUser { get; set; } = 3;
    public int SecondsPerStage { get; set; } = 12;

    public int RemainingSecondForStage { get; set; } 

    public List<QuestionCSVModel> Questions { get; set; }

    public Game()
    {
        var engine = new FileHelpers.FileHelperEngine<QuestionCSVModel>();

        this.Questions = engine.ReadFile($"State/questions.csv").ToList();
    }

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

    public List<string> Categories { get; set; } = new List<string>
    {
        "SPORT",
        "GAMING",
        "DESIGN",
        "TECH",
        "GEOGRAPHY",
        "HISTORY",
        "ART",
        "MOVIES",
        "CARS",
        "MUSIC",
        "MYTH",
        "TRAVEL",
        "LANGUAGES",
        "FOOD",
        "SCIENCE",
    };

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

            if (this.RemainingSecondForStage == 0)
            {
                this.CorrectAnswerIsShowing = true;

                foreach (var player in Players)
                {
                    var playerScore = 0;

                    for (int i = 0; i <= this.Stage; i++)
                    {
                        if (player.Answers.ContainsKey(i) && player.Answers[i] == this.Questions.ElementAt(i).Answer)
                        {
                            playerScore++;
                        }
                    }

                    player.Score = playerScore;
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
}
