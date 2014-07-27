using RTS.Commands.Interfaces;
using RTS.Entities.Interfaces.Player;
using RTS.Entities.Interfaces.Teams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RTS.Commands.Server
{
    public class PlayerDisconnectedCommand : IMmoCommand<IPlayer>
    {
        public object PlayerActor { get; set; }

        public void Execute(IPlayer target)
        {
            target.HandlePlayerDisconnected(PlayerActor);
        }

        public bool CanExecute(IPlayer target)
        {
            return true;
        }

        public Core.Enums.CommandId CommandId
        {
            get { return Core.Enums.CommandId.PlayerJoined; }
        }

        public Core.Enums.Destination CommandDestination
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
