using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SofaScoreApi
{
    public class SofaTeam
    {
        public string TeamName { get; set; } = "";
        public bool IsNationalTeamSofa { get; set; }
        public uint ManagerID { get; set; }
        public int ID { get; set; }
        public string ShortLicensedName { get; set; } = "";
        public string TeamNationality { get; set; } = "";

        public ObservableCollection<SofaPlayer> Players { get; set; } = new ObservableCollection<SofaPlayer>();
        public List<int> Tournaments { get; set; } = new List<int>();

        public override string ToString()
        {
            return TeamName;
        }
    }
}
