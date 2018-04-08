using System.Collections.Generic;

namespace SmartMirrorServer.Features.FootballResults
{
    internal class FootballMatchdayResults
    {
        public List<Match> Matches { get; }

        public FootballMatchdayResults()
        {
            Matches = new List<Match>();
        }
    }
}