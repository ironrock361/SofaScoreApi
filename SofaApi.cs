using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SofaScoreApi
{
    public static class SofaApi
    {
        //public static Dictionary<int, SofaTeam> TeamsDict = new Dictionary<int, SofaTeam>();
        public static Dictionary<int, SofaPlayer> PlayersDict = new Dictionary<int, SofaPlayer>();

        public static ObservableCollection<SofaTeam> Teams { get; set; } = new ObservableCollection<SofaTeam>(); 

        public static string Serialize()
        {
            string teams = JsonConvert.SerializeObject(Teams);
            //var t = JsonConvert.DeserializeObject<Dictionary<int, SofaTeam>>(teams);

            return teams;
        }

        public static async Task<List<SofaTeam>> ReadTournament(int id)
        {
            var result = new List<SofaTeam>();

            _ = await WebApi.GetAsync($"https://api.sofascore.com/api/v1/unique-tournament/{id}/season/42632/standings/total")
                .ContinueWith(async (response) =>
              {
                  if (!response.IsFaulted)
                  {
                      dynamic js = JObject.Parse(response.Result);

                      for (int i = 0; i < js.standings[0].rows.Count; i++)
                      {
                          var r = js.standings[0].rows[i];
                          Console.WriteLine(r.team.name);
                          var tid = Convert.ToInt32(r.team.id);
                          SofaTeam t = ReadTeam2(tid);
                          //t.Wait();
                          result.Add(t);
                      }
                  }
              });

            return result;
            
        }
        public static bool ReadTournament2(int id)
        {
            var result = new List<SofaTeam>();
            var seasonInfo = WebApi.Get($"https://api.sofascore.com/api/v1/unique-tournament/{id}/seasons");
            dynamic js1 = JObject.Parse(seasonInfo);
            var seasonId = js1.seasons[0].id;

            //var tourInfo = WebApi.Get($"https://api.sofascore.com/api/v1/unique-tournament/{id}/season/42632/standings/total");
            var tourInfo = WebApi.Get($"https://api.sofascore.com/api/v1/unique-tournament/{id}/season/{seasonId}/standings/total");
            dynamic js = JObject.Parse(tourInfo);

            for (int i = 0; i < js.standings[0].rows.Count; i++)
            {
                var r = js.standings[0].rows[i];
                var tid = Convert.ToInt32(r.team.id);

                SofaTeam t = ReadTeam2(tid);

                //if (Teams.Count(t => t.ID == tid) == 0)
                //{
                //    var t = ReadTeam2(tid);
                //}
            }

            //return result;
            return true;
        }

        public static SofaTeam ReadTeam2(int id)
        {
            var team = new SofaTeam();

            //await Task.Run(() =>
            //{
            // read team info
            var teamInfo = WebApi.Get($"https://api.sofascore.com/api/v1/team/{id}");
            dynamic js = JObject.Parse(teamInfo);
            team.TeamName = js.team.name;
            team.ShortLicensedName = js.team.nameCode;
            team.ID = ConvertToInt32(js.team.id);
            //team.IsNationalTeam = js.team.country.name;
            team.ManagerID = (uint)ConvertToInt32(js.team.manager?.id);
            //team.ManagerName = js.team.manager.name;
            //ManagerCountry = js.team.manager.country.name;
            team.IsNationalTeamSofa = js.team.national;
            team.TeamNationality = js.team.country.name;
            var tournamentId = ConvertToInt32(js.team.primaryUniqueTournament?.id);

            if (tournamentId > 0)
            {
                team.Tournaments.Add(tournamentId);
            }

            Console.WriteLine($"{team.TeamName} bilgileri alındı.");

            // read players
            var playerInfo = WebApi.Get($"https://api.sofascore.com/api/v1/team/{id}/players");

            js = JObject.Parse(playerInfo);

            foreach (var jsp in js.players)
            {
                var p = new SofaPlayer();
                p.ID = (uint)ConvertToInt32(jsp.player.id);
                p.PlayerName = jsp.player.name;
                p.PrintNameClub = jsp.player.shortName;
                p.Nationality = jsp.player.country.name;

                if (team.IsNationalTeamSofa)
                {
                    p.ShirtNoNational = ConvertToInt32(jsp.player.shirtNumber);
                }
                else
                {
                    p.ShirtNoClub = ConvertToInt32(jsp.player.shirtNumber);
                }

                team.Players.Add(p);
                p.TeamName = team.TeamName;
                p.TeamID = id;
            }

            Console.WriteLine($"{team.TeamName} oyuncuları alındı.");
            //});

            Teams.Add(team);
            return team;
        }

        public static int ConvertToInt32(object obj)
        {
            try
            {
                return Convert.ToInt32(obj);
            }
            catch
            {
                return 0;
            }
        }

        public static async Task<SofaTeam> ReadTeam(int id)
        {
            var team = new SofaTeam();

            // read team info
            var taskInfo = WebApi.GetAsync($"https://api.sofascore.com/api/v1/team/{id}");
            var taskInfoCompleted = taskInfo.ContinueWith((response) =>
                {
                    if (!response.IsFaulted)
                    {
                        dynamic js = JObject.Parse(response.Result);
                        team.TeamName = js.team.name;
                        team.ShortLicensedName = js.team.nameCode;
                        team.ID = Convert.ToInt32(js.team.id);
                        //team.IsNationalTeam = js.team.country.name;
                        team.ManagerID = (uint)Convert.ToInt32(js.team.manager.id);
                        //team.ManagerName = js.team.manager.name;
                        //ManagerCountry = js.team.manager.country.name;
                        team.IsNationalTeamSofa = js.team.national;
                        team.TeamNationality = js.team.country.name;

                        Console.WriteLine($"{team.TeamName} bilgileri alındı.");
                    }
                });

            // read players
            var taskPlayers = WebApi.GetAsync($"https://api.sofascore.com/api/v1/team/{id}/players");
            var taskPlayersCompleted = taskPlayers.ContinueWith((response) =>
                {
                    if (!response.IsFaulted)
                    {
                        dynamic js = JObject.Parse(response.Result);

                        foreach (var jsp in js.players)
                        {
                            var p = new SofaPlayer();
                            p.ID = (uint)Convert.ToInt32(jsp.player.id);
                            p.PlayerName = jsp.player.name;
                            p.PrintNameClub = jsp.player.shortName;
                            p.Nationality = jsp.player.country.name;

                            if (team.IsNationalTeamSofa)
                            {
                                p.ShirtNoNational = Convert.ToInt32(jsp.player.shirtNumber);
                            }
                            else
                            {
                                p.ShirtNoClub = Convert.ToInt32(jsp.player.shirtNumber);
                            }

                            team.Players.Add(p);
                            p.TeamName = team.TeamName;
                            p.TeamID = id;
                        }
                        Console.WriteLine($"{team.TeamName} oyuncuları alındı.");

                    }
                });

            await taskInfoCompleted;
            await taskPlayersCompleted;

            Teams.Add(team);
            return team;
        }
    }
}
