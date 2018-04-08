using System;
using System.Collections.Generic;

namespace SmartMirror.Features.FootballResults
{
    public class Match
    {
        public List<Goal> Goals { get; set; }

        public Group Group { get; set; }

        public int LeagueId { get; set; }

        public string LeagueName { get; set; }

        public Location Location { get; set; }

        public DateTime MatchDateTime { get; set; }

        public int MatchId { get; set; }

        public List<MatchResult> MatchResults { get; set; }

        public int NumberOfViewers { get; set; }

        public Team TeamOne { get; set; }

        public Team TeamTwo { get; set; }

        public Match()
        {
            Goals = new List<Goal>();

            MatchResults = new List<MatchResult>();
        }
    }
}