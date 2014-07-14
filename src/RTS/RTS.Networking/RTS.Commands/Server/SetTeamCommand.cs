﻿using RTS.Commands.Interfaces;
using RTS.Entities.Interfaces.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RTS.Commands.Server
{
    public class SetTeamCommand : MmoCommand<IPlayer>, IPlayerCommand
    {
        public object TeamActor { get; set; }

        public override void Execute(IPlayer target)
        {
            target.SetTeam(this.TeamActor);
        }

        public override bool CanExecute(IPlayer target)
        {
            return true;
        }

        public override Core.Enums.CommandId CommandId
        {
            get { return Core.Enums.CommandId.SetTeam; }
        }

        public override Core.Enums.Destination CommandDestination
        {
            get { return Core.Enums.Destination.Server; }
        }
    }
}
