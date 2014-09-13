using RTS.Commands.Interfaces;
using RTS.Entities.Interfaces.Teams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RTS.Commands.Server
{
    public class PlayerConnectedCommand : MmoCommand<ITeam>
    {
        public object PlayerActor { get; set; }

        public override void Execute(ITeam target)
        {
            target.HandlePlayerJoined(PlayerActor);
        }

        public override bool CanExecute(ITeam target)
        {
            return true;
        }

        public override Core.Enums.CommandId CommandId
        {
            get { return Core.Enums.CommandId.PlayerJoined; }
        }

        public override Core.Enums.Destination CommandDestination
        {
            get { return Core.Enums.Destination.Server; }
        }
        public bool TellClient
        {
            get { return false; }
        }

        public bool TellServer
        {
            get { return true; }
        }
    }
}
