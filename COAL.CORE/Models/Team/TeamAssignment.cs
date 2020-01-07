using System;
using System.Collections.Generic;
using System.Text;

namespace COAL.CORE.Models.Team
{
    public class TeamAssignment
    {
        public string PlayerId { get; set; }
        public string TeamId { get; set; }
        public int ShirtNumber { get; set; }
        public bool IsCaptain { get; set; }
        public bool IsPenaltyTaker { get; set; }
        public bool IsLongFreekickTaker { get; set; }
        public bool IsLeftShortFreekickTaker { get; set; }
        public bool IsRightShortFreekickTaker { get; set; }
        public bool IsRightCornerTaker { get; set; }
        public bool IsLeftCornerTaker { get; set; }
        public int Order { get; set; }
    }
}
