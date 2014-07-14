using RTS.Commands.Interfaces;
using RTS.Entities.Interfaces.Teams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RTS.Commands.Server
{
    public class SetPlayerCommand : IMmoCommand<ITeam>
    {
        public object PlayerActor { get; set; }


        public void Execute(ITeam target)
        {
            target.SetPlayer(this.PlayerActor);
        }

        public bool CanExecute(ITeam target)
        {
            return true;
        }

        public Core.Enums.CommandId CommandId
        {
            get { return Core.Enums.CommandId.SetPlayer; }
        }

        public Core.Enums.Destination CommandDestination
        {
            get { return Core.Enums.Destination.Server; }
        }
    }
}
