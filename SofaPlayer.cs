namespace SofaScoreApi
{
    public class SofaPlayer
    {
        public uint ID { get; set; }
        public string PlayerName { get; set; }
        public string PrintNameClub { get; set; }
        public int ShirtNoNational { get; set; }
        public int ShirtNoClub { get; set; }
        //public SofaTeam Team { get; set; }
        public string Nationality { get; set; } = "";
        public string TeamName { get; set; }
        public int TeamID { get; set; }
        public override string ToString()
        {
            return PlayerName;
        }
    }
}
