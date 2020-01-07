namespace COAL.PES.Models
{
    public class PESCompetitionEntry
    {
        // Position in .BIN file
        public int Position { get; set; }

        // Metadata
        public uint Id { get; set; }
        public uint CompetitionId { get; set; }
        public uint ContinentalCompetitionId { get; set; }
        public uint TeamId { get; set; }
        public byte Order { get; set; }
        public uint Group { get; internal set; }
    }
}
