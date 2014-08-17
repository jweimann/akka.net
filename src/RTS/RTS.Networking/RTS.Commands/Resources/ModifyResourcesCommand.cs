using RTS.Commands.Interfaces;
using RTS.Entities.Interfaces.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RTS.Commands.Resources
{
    public class ModifyResourcesCommand : MmoCommand<IPlayer>, IMmoCommand<IPlayer>
    {
        public int Amount { get; set; }
        public override Core.Enums.CommandId CommandId
        {
            get { return Core.Enums.CommandId.ModifyResources; }
        }

        public override Core.Enums.Destination CommandDestination
        {
            get { return Core.Enums.Destination.Server; }
        }

        public override void Execute(IPlayer target)
        {
            target.SetMoney(target.GetMoney() + Amount);
        }

        public override bool CanExecute(IPlayer target)
        {
            return true;
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
