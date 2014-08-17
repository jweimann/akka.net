using RTS.Commands.Interfaces;
using RTS.Entities.Interfaces.Teams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RTS.Commands.Server
{
    public class SetPlayerCommand : MmoCommand<ITeam>, IMmoCommand<ITeam>
    {
        public object PlayerActor { get; set; }


        public override void Execute(ITeam target)
        {
            target.SetPlayer(this.PlayerActor);
        }

        public override bool CanExecute(ITeam target)
        {
            return true;
        }

        public override Core.Enums.CommandId CommandId
        {
            get { return Core.Enums.CommandId.SetPlayer; }
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
