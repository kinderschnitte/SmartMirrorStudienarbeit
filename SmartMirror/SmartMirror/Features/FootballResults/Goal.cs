namespace SmartMirror.Features.FootballResults
{
    public class Goal
    {
        public int GoalGetterId { get; set; }

        public string GoalGetterName { get; set; }

        public bool IsOvertimeGoal { get; set; }

        public bool IsOwnGoal { get; set; }

        public bool IsPenaltyGoal { get; set; }

        public int MatchMinute { get; set; }

        public int ScoreTeam1 { get; set; }

        public int ScoreTeam2 { get; set; }
    }
}