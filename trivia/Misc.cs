namespace trivia
{
    public static class Misc
    {
        public static Team GetTeam(string? team)
        {
            Team teamObj = new Team();

            if (string.IsNullOrWhiteSpace(team))
            {
                return teamObj;
            }

            switch(team.ToUpper())
            {
                case "GREEN":
                    teamObj = new Team
                    {
                        Name = team,
                        Color = "#CDFE00",
                        IsDark = false,
                    };
                    break;
                case "RED":
                    teamObj = new Team
                    {
                        Name = team,
                        Color = "#DE1919",
                        IsDark = true,
                    };
                    break;
                case "BLUE":
                    teamObj = new Team
                    {
                        Name = team,
                        Color = "#04B0E6",
                        IsDark = true,
                    };
                    break;
                case "YELLOW":
                    teamObj = new Team
                    {
                        Name = team,
                        Color = "#F8EE06",
                        IsDark = false,
                    };
                    break;
                case "PINK":
                    teamObj = new Team
                    {
                        Name = team,
                        Color = "#FF82AF",
                        IsDark = false,
                    };
                    break;
                case "PURPULE":
                    teamObj = new Team
                    {
                        Name = team,
                        Color = "#9234CB",
                        IsDark = true,
                    };
                    break;
                case "ORANGE":
                    teamObj = new Team
                    {
                        Name = team,
                        Color = "#E65300",
                        IsDark = true,
                    };
                    break;
            }

            return teamObj;
        }
    }


    //"GREEN",
    //"RED",
    //"BLUE",
    //"YELLOW",
    //"PINK",
    //"PURPULE",
    //"ORANGE",

    public class Team
    {
        public string? Name { get; set; }
        public string Color { get; set; } = "#CDFE00";
        public bool IsDark { get; set; }
    }
}
