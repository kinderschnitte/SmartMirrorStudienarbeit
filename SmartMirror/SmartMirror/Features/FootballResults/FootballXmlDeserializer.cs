using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SmartMirror.Features.FootballResults
{
    public static class FootballXmlDeserializer
    {
        public static async Task<FootballMatchdayResults> GetFootballMatchdayResults()
        {
            FootballMatchdayResults footballMatchdayResults = new FootballMatchdayResults();

            string matchdayXmlString = await getXml("https://www.openligadb.de/api/getmatchdata/bl1");

            XDocument xDocument = XDocument.Parse(matchdayXmlString);
            

            foreach (XElement matchElement in xDocument.Descendants("Match"))
            {
                Match match = new Match();

                foreach (XElement goalElement in matchElement.Elements("Goal"))
                {
                    Goal goal = new Goal
                    {
                        GoalGetterId = int.Parse(goalElement.Element("GoalGetterID").Value),
                        GoalGetterName = goalElement.Element("GoalGetterName").Value,
                        IsOvertimeGoal = bool.Parse(goalElement.Element("IsOvertimeGoal").Value),
                        IsOwnGoal = bool.Parse(goalElement.Element("IsOwnGoal").Value),
                        IsPenaltyGoal = bool.Parse(goalElement.Element("IsPenaltyGoal").Value),
                        MatchMinute = int.Parse(goalElement.Element("MatchMinute").Value),
                        ScoreTeam1 = int.Parse(goalElement.Element("ScoreTeam1").Value),
                        ScoreTeam2 = int.Parse(goalElement.Element("ScoreTeam2").Value)
                    };

                    match.Goals.Add(goal);
                }

                Group group = new Group
                {
                    GroupId = int.Parse(matchElement.Element("Group").Element("GoalGetterID").Value),
                    GroupName = matchElement.Element("Group").Element("GoalGetterName").Value,
                    GroupOrderId = int.Parse(matchElement.Element("Group").Element("GoalGetterID").Value)
                };

                match.Group = group;

                match.LeagueId = int.Parse(matchElement.Element("LeagueId").Value);

                match.LeagueName = matchElement.Element("LeagueName").Value;

                Location location = new Location
                {
                    LocationCity = matchElement.Element("Location").Element("LocationCity").Value,
                    LocationId = int.Parse(matchElement.Element("Location").Element("LocationID").Value),
                    LocationStadion = matchElement.Element("Location").Element("LocationStadium").Value
                };

                match.Location = location;

                match.MatchDateTime = DateTime.Parse(matchElement.Element("MatchDateTime").Value);

                match.MatchId = int.Parse(matchElement.Element("MatchID").Value);

                foreach (XElement matchResultElement in matchElement.Element("MatchResults").Elements("MatchResult"))
                {
                    MatchResult matchResult = new MatchResult
                    {
                        GoalsTeamOne = int.Parse(matchResultElement.Element("PointsTeam1").Value),
                        GoalsTeamTwo = int.Parse(matchResultElement.Element("PointsTeam2").Value),
                        ResultDescription = matchResultElement.Element("ResultDescription").Value,
                        ResultId = int.Parse(matchResultElement.Element("ResultID").Value),
                        ResultName = matchResultElement.Element("ResultName").Value,
                        ResultOrderId = int.Parse(matchResultElement.Element("ResultOrderID").Value)
                    };

                    match.MatchResults.Add(matchResult);
                }


                match.NumberOfViewers = int.Parse(matchElement.Element("NumberOfViewers").Value);

                Team teamOne = new Team
                {
                    TeamIconUrl = matchElement.Element("Team1").Element("TeamIconUrl").Value,
                    TeamId = int.Parse(matchElement.Element("Team1").Element("TeamId").Value),
                    TeamName = matchElement.Element("Team1").Element("TeamName").Value
                };

                match.TeamOne = teamOne;

                Team teamTwo = new Team
                {
                    TeamIconUrl = matchElement.Element("Team2").Element("TeamIconUrl").Value,
                    TeamId = int.Parse(matchElement.Element("Team2").Element("TeamId").Value),
                    TeamName = matchElement.Element("Team2").Element("TeamName").Value
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
                HttpResponseMessage response =  await client.GetAsync(uri);

                using (HttpContent content = response.Content)
                    return await content.ReadAsStringAsync();
            }
        }
    }
}