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
    public class FinishBuildEntityCommand : MmoCommand, IMmoCommand<ITeam>
    {
        public string Name { get; set; }
        public Vector3 Position { get; set; }
        public void Execute(ITeam target)
        {
            target.FinishBuildEntity(this.Name, this.Position);
        }

        public bool CanExecute(ITeam target)
        {
            return true;
        }

        public Core.Enums.CommandId CommandId
        {
            get { return Core.Enums.CommandId.FinishBuildEntity; }
        }

        public Destination CommandDestination { get { return Core.Enums.Destination.Server; } }
         
    }
}
