using System.Collections.Generic;

namespace SmartMirror.Features.FootballResults
{
    public class FootballMatchdayResults
    {
        public List<Match> Matches { get; set; }

        public FootballMatchdayResults()
        {
            Matches = new List<Match>();
        }
    }
}