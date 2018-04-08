using System;
using System.Collections.Generic;

namespace SmartMirrorServer.Features.FootballResults
{
    internal class Match
    {
        public List<Goal> Goals { get; }

        public Group Group { get; set; }

        public int LeagueId { get; set; }

        public string LeagueName { get; set; }

        public Location Location { get; set; }

        public DateTime MatchDateTime { get; set; }

        public int MatchId { get; set; }

        public bool MatchIsFinished { get; set; }

        public List<MatchResult> MatchResults { get; }

        public string NumberOfViewers { get; set; }

        public Team TeamOne { get; set; }

        public Team TeamTwo { get; set; }

        public Match()
        {
            Goals = new List<Goal>();

            MatchResults = new List<MatchResult>();
        }
    }
}