namespace SmartMirrorServer.Features.FootballResults
{
    internal class Goal
    {
        public int GoalGetterId { get; set; }

        public string GoalGetterName { get; set; }

        public bool IsOvertime { get; set; }

        public bool IsOwnGoal { get; set; }

        public bool IsPenalty { get; set; }

        public string MatchMinute { get; set; }

        public int ScoreTeam1 { get; set; }

        public int ScoreTeam2 { get; set; }
    }
}