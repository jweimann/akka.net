using RTS.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RTS.Core.Structs
{
    public struct SpawnEntityData
    {
        public string Name;
        public Vector3 Position;
        public long TeamId;
        public object TeamActor;
        public long EntityId;
        public UnitType UnitType;
    }
}
