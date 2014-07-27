using RTS.Commands.Interfaces;
using RTS.Core.Enums;
using RTS.Core.Structs;
using RTS.Entities.Interfaces.Teams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RTS.Commands.Buildings
{
    public class FinishBuildEntityCommand : MmoCommand<ITeam>, IMmoCommand<ITeam>
    {
        public UnitType UnitType { get; set; }
        public Vector3 Position { get; set; }
        public override void Execute(ITeam target)
        {
            target.FinishBuildEntity(this.UnitType, this.Position);
        }

        public override bool CanExecute(ITeam target)
        {
            return true;
        }

        public override Core.Enums.CommandId CommandId
        {
            get { return Core.Enums.CommandId.FinishBuildEntity; }
        }

        public override Destination CommandDestination { get { return Core.Enums.Destination.Server; } }

        CommandId IMmoCommand<ITeam>.CommandId
        {
            get { return Core.Enums.CommandId.FinishBuildEntity; }
        }

        Destination IMmoCommand<ITeam>.CommandDestination
        {
            get { return Destination.Server; }
        }

        void IMmoCommand<ITeam>.Execute(ITeam target)
        {
            target.FinishBuildEntity(this.UnitType, this.Position);
        }

        bool IMmoCommand<ITeam>.CanExecute(ITeam target)
        {
            return true;
        }
    }
}
