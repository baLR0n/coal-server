namespace COAL.PES.Models
{
    public class PESCompetition
    {
        // Position in .BIN file
        public int Position { get; set; }

        // Metadata
        public uint Id { get; set; }
        public string Name { get; set; }
        public uint RegionId { get; set; }
    }
}
