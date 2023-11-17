﻿namespace trivia.State;

public class Game
{
    public bool GameStarted { get; set; }
    public bool GameFinished { get; set; }
    public int SecondsSinceGameStart { get; set; }
    public int Stage { get; set; }
    public List<Player> Players { get; set; } = new List<Player>();
    public EventHandler StateChange { get; set; } = default!;
    public void StateChanged() => StateChange?.Invoke(this, EventArgs.Empty);

    public int NumberOfCategoriesPerUser { get; set; } = 3;
    public int SecondsPerStage { get; set; } = 10;

    public List<QuestionCSVModel> Questions { get; set; }

    public Game()
    {
        var engine = new FileHelpers.FileHelperEngine<QuestionCSVModel>();

        this.Questions = engine.ReadFile($"State/questions.csv").ToList();
    }

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

    public void RemovePlayer(string code)
    {
        var player = this.Players.FirstOrDefault(x=> x.Code == code);

        if (player is not null)
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
        if (this.Stage != stage)
            return;


    }

    private async Task Timer()
    {
        while (this.GameStarted)
        {
            await Task.Delay(1000);

            this.SecondsSinceGameStart++;

            if (SecondsPerStage * (this.Stage + 1) == this.SecondsSinceGameStart)
                this.Stage++;

            if ((this.Stage + 1) > this.Questions.Count)
            {
                this.GameStarted = false;
                this.GameFinished = true;
            }

            this.StateChanged();
        }
    }

    public async Task StartGame()
    {
        this.GameStarted = true;
        this.Stage = 0;
        this.SecondsSinceGameStart = 0;

        this.StateChanged();

        await this.Timer();
    }
}
