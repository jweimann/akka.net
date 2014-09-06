using RTS.Commands.Interfaces;
using RTS.Core.Enums;
using RTS.Core.Structs;
using RTS.Entities.Interfaces.Teams;
using RTS.Entities.Interfaces.UnitTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RTS.Commands.Buildings
{
    /// <summary>
    /// Updates the client on build progress of a specific entity.
    /// </summary>
    [Serializable]
    public class UpdateBuildProgressCommand : MmoCommand<IBuilding>, IMmoCommand<IBuilding>, IEntityComponentCommand
    {
        public UpdateBuildProgressCommand(long builderEntityId, UnitType unitType, byte percentageComplete)
        {
            this.EntityId = builderEntityId;
            this.UnitType = unitType;
            this.PercentageComplete = percentageComplete;
        }
        public long EntityId { get; set; }
        public UnitType UnitType { get; set; }
        public byte PercentageComplete { get; set; } // use 0-100

        public override Destination CommandDestination { get { return Core.Enums.Destination.Client; } }
        public override CommandId CommandId { get { return CommandId.UpdateBuildProgressCommand; } }
        public override void Execute(IBuilding target)
        {
            target.UpdateBuildProgress(this.UnitType, this.PercentageComplete);
        }

        public override bool CanExecute(IBuilding target)
        {
            return true;
        }
    }
}
