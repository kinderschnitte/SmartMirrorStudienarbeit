namespace SmartMirrorServer.Features.FootballResults
{
    internal class TableTeam
    {
        public int Draw { get; set; }

        public int Goals { get; set; }

        public int Lost { get; set; }

        public int Matches { get; set; }

        public int OpponentGoals { get; set; }

        public int Points { get; set; }

        public string ShortName { get; set; }

        public string TeamIconUrl { get; set; }

        public int TeamInfoId { get; set; }

        public string TeamName { get; set; }

        public int Won { get; set; }
    }
}