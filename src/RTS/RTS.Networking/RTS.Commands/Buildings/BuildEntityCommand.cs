using RTS.Commands.Interfaces;
using RTS.Core.Enums;
using RTS.Core.Structs;
using RTS.Entities.Interfaces.Control;
using RTS.Entities.Interfaces.UnitTypes;
using RTS.DataStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RTS.Commands.Buildings
{
    [Serializable]
    public class BuildEntityCommand : MmoCommand<IBuilding>, IBuildingCommand
    {
        public long BuildingEntityId { get; set; }
        public string Name { get; set; }
        public Vector3? BuildPosition { get; set; }
        public UnitType UnitTypeId { get; set; }

        public override Core.Enums.CommandId CommandId
        {
            get { return Core.Enums.CommandId.BuildEntity; }
        }
        public override Destination CommandDestination { get { return Destination.Server; } }
      
        public override void Execute(IBuilding target)
        {
            target.BuildEntity(this.UnitTypeId, BuildPosition);
        }

        public override bool CanExecute(IBuilding target)
        {
            return target.CanBuild(new UnitDefinition() { UnitType = this.UnitTypeId });
        }

        CommandId IMmoCommand<IBuilding>.CommandId
        {
            get { throw new NotImplementedException(); }
        }

        Destination IMmoCommand<IBuilding>.CommandDestination
        {
            get { throw new NotImplementedException(); }
        }

        void IMmoCommand<IBuilding>.Execute(IBuilding target)
        {
            throw new NotImplementedException();
        }

        bool IMmoCommand<IBuilding>.CanExecute(IBuilding target)
        {
            throw new NotImplementedException();
        }
    }
}
