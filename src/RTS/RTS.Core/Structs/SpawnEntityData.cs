using RTS.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RTS.Core.Structs
{
    public class SpawnEntityData // Struct is not getting initialized somewhere and defaulting, causing problems, want a null ref to figure out what's going on.
    {
        public string Name;
        public Vector3 Position;
        public long TeamId;
        public object TeamActor;
        public long EntityId;
        public UnitType UnitType;
        public object Stats;

        public override string ToString()
        {
            return String.Format("Name: {0} ID: {1}", Name, EntityId);
        }
    }
}
