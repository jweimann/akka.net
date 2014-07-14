using RTS.Core.Structs;
using RTS.DataStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RTS.Entities.Interfaces.UnitTypes
{
    public interface IBuilding : IEntityComponent
    {
        void BuildEntity(UnitDefinition unitDefinition, Vector3? position);
        List<UnitDefinition> BuildableEntities { get; set; }
        bool CanBuild(UnitDefinition unitDefinition);
    }
}
