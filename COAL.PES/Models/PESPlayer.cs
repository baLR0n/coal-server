using COAL.CORE;
using System;
using System.Collections.Generic;
using System.Text;

namespace COAL.PES.Models
{
    public class PESPlayer
    {
        // Metadata
        public uint Id { get; set; }
        public string Name { get; set; }
        public string ShirtName { get; set; }
        public uint Weight { get; set; }
        public uint Height { get; set; }
        public uint Nationality { get; set; }
        public uint Nationality2 { get; set; }

        // Skills
        public uint AttackProwess { get; set; }
        public uint BallControl { get; set; }
        public uint DefenseProwess { get; set; }
        public uint Dribbling { get; set; }
        public uint Curve { get; set; }
        public uint LowPass { get; set; }
        public uint LoftedPass { get; set; }
        public uint Finishing { get; set; }
        public uint SetPieces { get; set; }
        public uint Header { get; set; }
        public uint BallWinning{ get; set; }
        public uint KickingPower { get; set; }
        public uint Penalty { get; set; }
        public uint Speed { get; set; }
        public uint ExplosivePower { get; set; }
        public uint BodyControl { get; set; }
        public uint PhysicalContact { get; set; }
        public uint Jump { get; set; }
        public uint Goalkeeping { get; set; }
        public uint Catching { get; set; }
        public uint Clearing { get; set; }
        public uint Reflexes { get; set; }
        public uint Coverage { get; set; }
        public uint Stamina { get; set; }
        public uint WeakFootUsage { get; set; }
        public uint WeakFootPrecision { get; set; }
        public uint Condition { get; set; }
        public uint InjuryResistance { get; set; }

        // Positions
        public uint GK { get; set; }
        public uint LeftBack { get; set; }

        //ToDo: Perks, etc.
        public uint Attitude { get; set; }

    }
}
