namespace COAL.PES.Models
{
    public class PESPlayerAssignment
    {
        // Position in .BIN file
        public int Position { get; set; }

        // Metadata
        public uint EntryId { get; set; }
        public uint PlayerId { get; set; }
        public uint TeamId { get; set; }
        public bool IsCaptain { get; set; }
        public bool IsPenaltyTaker { get; set; }
        public bool IsLongFreekickTaker { get; set; }
        public bool IsShortFreekickTaker { get; set; }
        public bool IsRightCornerTaker { get; set; }
        public bool IsLeftCornerTaker { get; set; }
        public int ShirtNumber { get; set; }
        public uint Order { get; set; }
    }

}