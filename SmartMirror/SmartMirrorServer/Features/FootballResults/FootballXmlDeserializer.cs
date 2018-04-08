using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SmartMirrorServer.Features.FootballResults
{
    internal static class FootballXmlDeserializer
    {

        #region Public Methods

        public static async Task<List<GoalGetter>> GetFootballGoalGetters(string league, int season)
        {
            string matchdayXmlString = await getXml($"https://www.openligadb.de/api/getgoalgetters/{league}/{season}");

            XDocument xDocument = XDocument.Parse(matchdayXmlString);

            XNamespace xNamespace = "http://schemas.datacontract.org/2004/07/OLDB.Spa.Models.Api";

            return xDocument.Descendants(xNamespace + "GoalGetter").Select(goalgetterElement => new GoalGetter { GoalCount = int.Parse(goalgetterElement.Element(xNamespace + "GoalCount")?.Value), GoalGetterId = int.Parse(goalgetterElement.Element(xNamespace + "GoalGetterId")?.Value), GoalGetterName = goalgetterElement.Element(xNamespace + "GoalGetterName")?.Value }).ToList();
        }

        public static async Task<Matchday> GetFootballMatchdayInfo(string league)
        {
            string matchdayXmlString = await getXml($"https://www.openligadb.de/api/getcurrentgroup/{league}");

            XDocument xDocument = XDocument.Parse(matchdayXmlString);

            XNamespace xNamespace = "http://schemas.datacontract.org/2004/07/OLDB.Spa.Models.Api";

            Matchday matchday = new Matchday
            {
                GroupId = int.Parse(xDocument.Element(xNamespace + "Group")?.Element(xNamespace + "GroupID")?.Value),
                GroupName = xDocument.Element(xNamespace + "Group")?.Element(xNamespace + "GroupName")?.Value,
                GroupOrderId = int.Parse(xDocument.Element(xNamespace + "Group")?.Element(xNamespace + "GroupOrderID")?.Value)
            };

            return matchday;
        }

        public static async Task<FootballMatchdayResults> GetFootballMatchdayResults(string league)
        {
            string matchdayXmlString = await getXml($"https://www.openligadb.de/api/getmatchdata/{league}");

            FootballMatchdayResults footballMatchdayResults = getFootballResults(matchdayXmlString);

            return footballMatchdayResults;
        }

        public static async Task<FootballMatchdayResults> GetFootballResultsForMatchday(string league, int matchday, int season) // Season 2017/2018 --> season = 2017
        {
            string matchdayXmlString = await getXml($"https://www.openligadb.de/api/getmatchdata/{league}/{season}/{matchday}");

            FootballMatchdayResults footballMatchdayResults = getFootballResults(matchdayXmlString);

            return footballMatchdayResults;
        }

        public static async Task<FootballMatchdayResults> GetFootballResultsForSeason(string league, int season)
        {
            string matchdayXmlString = await getXml($"https://www.openligadb.de/api/getmatchdata/{league}/{season}");

            FootballMatchdayResults footballMatchdayResults = getFootballResults(matchdayXmlString);

            return footballMatchdayResults;
        }
        public static async Task<List<TableTeam>> GetFootballTable(string league, int season)
        {
            List<TableTeam> table = new List<TableTeam>();

            string matchdayXmlString = await getXml($"https://www.openligadb.de/api/getbltable/{league}/{season}");

            XDocument xDocument = XDocument.Parse(matchdayXmlString);

            XNamespace xNamespace = "http://schemas.datacontract.org/2004/07/OLDB.Spa.Models.Api";

            foreach (XElement teamElement in xDocument.Descendants(xNamespace + "BlTableTeam"))
            {
                TableTeam tableTeam = new TableTeam
                {
                    Draw = int.Parse(teamElement.Element(xNamespace + "Draw")?.Value),
                    Goals = int.Parse(teamElement.Element(xNamespace + "Goals")?.Value),
                    Lost = int.Parse(teamElement.Element(xNamespace + "Lost")?.Value),
                    Matches = int.Parse(teamElement.Element(xNamespace + "Matches")?.Value),
                    OpponentGoals = int.Parse(teamElement.Element(xNamespace + "OpponentGoals")?.Value),
                    Points = int.Parse(teamElement.Element(xNamespace + "Points")?.Value),
                    ShortName = teamElement.Element(xNamespace + "ShortName")?.Value,
                    TeamIconUrl = teamElement.Element(xNamespace + "TeamIconUrl")?.Value,
                    TeamInfoId = int.Parse(teamElement.Element(xNamespace + "TeamInfoId")?.Value),
                    TeamName = teamElement.Element(xNamespace + "TeamName")?.Value,
                    Won = int.Parse(teamElement.Element(xNamespace + "Won")?.Value)
                };

                table.Add(tableTeam);
            }

            return table;
        }

        #endregion Public Methods

        #region Private Methods

        private static FootballMatchdayResults getFootballResults(string xmlString)
        {
            FootballMatchdayResults footballMatchdayResults = new FootballMatchdayResults();

            XDocument xDocument = XDocument.Parse(xmlString);

            XNamespace xNamespace = "http://schemas.datacontract.org/2004/07/OLDB.Spa.Models.Api";

            foreach (XElement matchElement in xDocument.Descendants(xNamespace + "Match"))
            {
                Match match = new Match();

                IEnumerable<XElement> goalElements = matchElement.Element(xNamespace + "Goals")?.Elements(xNamespace + "Goal");
                if (goalElements != null)
                {
                    foreach (XElement goalElement in goalElements)
                    {
                        Goal goal = new Goal { GoalGetterId = int.Parse(goalElement.Element(xNamespace + "GoalGetterID")?.Value), GoalGetterName = goalElement.Element(xNamespace + "GoalGetterName")?.Value, IsOvertime = bool.Parse(goalElement.Element(xNamespace + "IsOvertime")?.Value), IsOwnGoal = bool.Parse(goalElement.Element(xNamespace + "IsOwnGoal")?.Value), IsPenalty = bool.Parse(goalElement.Element(xNamespace + "IsPenalty")?.Value), MatchMinute = goalElement.Element(xNamespace + "MatchMinute")?.Value, ScoreTeam1 = int.Parse(goalElement.Element(xNamespace + "ScoreTeam1")?.Value), ScoreTeam2 = int.Parse(goalElement.Element(xNamespace + "ScoreTeam2")?.Value) };

                        match.Goals.Add(goal);
                    }
                }

                Group group = new Group
                {
                    GroupId = int.Parse(matchElement.Element(xNamespace + "Group")?.Element(xNamespace + "GroupID")?.Value),
                    GroupName = matchElement.Element(xNamespace + "Group")?.Element(xNamespace + "GroupName")?.Value,
                    GroupOrderId = int.Parse(matchElement.Element(xNamespace + "Group")?.Element(xNamespace + "GroupOrderID")?.Value)
                };

                match.Group = group;

                match.LeagueId = int.Parse(matchElement.Element(xNamespace + "LeagueId")?.Value);

                match.LeagueName = matchElement.Element(xNamespace + "LeagueName")?.Value;

                Location location = new Location
                {
                    LocationCity = matchElement.Element(xNamespace + "Location")?.Element(xNamespace + "LocationCity")?.Value,
                    LocationId = matchElement.Element(xNamespace + "Location")?.Element(xNamespace + "LocationID")?.Value,
                    LocationStadion = matchElement.Element(xNamespace + "Location")?.Element(xNamespace + "LocationStadium")?.Value
                };

                match.Location = location;

                match.MatchDateTime = DateTime.Parse(matchElement.Element(xNamespace + "MatchDateTime")?.Value);

                match.MatchId = int.Parse(matchElement.Element(xNamespace + "MatchID")?.Value);

                match.MatchIsFinished = bool.Parse(matchElement.Element(xNamespace + "MatchIsFinished")?.Value);

                IEnumerable<XElement> matchResultElements = matchElement.Element(xNamespace + "MatchResults")?.Elements(xNamespace + "MatchResult");
                if (matchResultElements != null)
                {
                    foreach (XElement matchResultElement in matchResultElements)
                    {
                        MatchResult matchResult = new MatchResult { GoalsTeamOne = int.Parse(matchResultElement.Element(xNamespace + "PointsTeam1")?.Value), GoalsTeamTwo = int.Parse(matchResultElement.Element(xNamespace + "PointsTeam2")?.Value), ResultDescription = matchResultElement.Element(xNamespace + "ResultDescription")?.Value, ResultId = int.Parse(matchResultElement.Element(xNamespace + "ResultID")?.Value), ResultName = matchResultElement.Element(xNamespace + "ResultName")?.Value, ResultOrderId = int.Parse(matchResultElement.Element(xNamespace + "ResultOrderID")?.Value) };

                        match.MatchResults.Add(matchResult);
                    }
                }

                match.NumberOfViewers = matchElement.Element(xNamespace + "NumberOfViewers")?.Value;

                Team teamOne = new Team
                {
                    TeamIconUrl = matchElement.Element(xNamespace + "Team1")?.Element(xNamespace + "TeamIconUrl")?.Value,
                    TeamId = int.Parse(matchElement.Element(xNamespace + "Team1")?.Element(xNamespace + "TeamId")?.Value),
                    TeamName = matchElement.Element(xNamespace + "Team1")?.Element(xNamespace + "TeamName")?.Value
                };

                match.TeamOne = teamOne;

                Team teamTwo = new Team
                {
                    TeamIconUrl = matchElement.Element(xNamespace + "Team2")?.Element(xNamespace + "TeamIconUrl")?.Value,
                    TeamId = int.Parse(matchElement.Element(xNamespace + "Team2")?.Element(xNamespace + "TeamId")?.Value),
                    TeamName = matchElement.Element(xNamespace + "Team2")?.Element(xNamespace + "TeamName")?.Value
                };

                match.TeamTwo = teamTwo;

                footballMatchdayResults.Matches.Add(match);
            }

            return footballMatchdayResults;
        }

        private static async Task<string> getXml(string uri)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));

                HttpResponseMessage response =  await client.GetAsync(uri);

                using (HttpContent content = response.Content)
                    return await content.ReadAsStringAsync();
            }
        }

        #endregion Private Methods

    }
}