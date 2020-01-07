namespace COAL.PES.Models
{
    public class PESTeam
    {
        // Position in .BIN file
        public int Position { get; set; }

        // Metadata
        public uint Id { get; set; }
        public string Name { get; set; }
        public uint FeederTeamId { get; set; }
        public uint ParentTeamId { get; set; }
        public uint StadiumId { get; set; }
        public uint National { get; set; }
        public uint ManagerId { get; set; }
        public uint LicensedCoach { get; set; }
        public uint LicensedCoach2 { get; set; }
        public uint HasAnthem { get; set; }
        public uint CountryId { get; set; }

    }
}
