using RTS.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RTS.DataStructures
{
    public struct UnitDefinition
    {
        public UnitType UnitType;
        public string Name;
        public bool SpawnLocationRequired;
        public float BuildTime;
        public int Cost;

        public int StartingHP;

        public int ResourceAmount;
        public double ResourceInterval;

        public List<UnitType> CanBuild;
    }
}
