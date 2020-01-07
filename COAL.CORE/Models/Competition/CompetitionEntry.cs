namespace COAL.CORE.Models.Competition
{
    public class CompetitionEntry : BaseModel
    {
        public string CompetitionEntryId { get; set; }
        public string CompetitionId { get; set; }
        public string TeamId { get; set; }

        public int Order { get; set; }
        public int Group { get; set; }
    }
}
