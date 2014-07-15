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
        public string Name { get; set; }
        public Vector3 Position { get; set; }
        public override void Execute(ITeam target)
        {
            target.FinishBuildEntity(this.Name, this.Position);
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
         
    }
}
