using RTS.Core.Structs;
using RTS.Entities.Interfaces.Movement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RTS.Commands
{
    [Serializable]
    public class SetDestinationCommand : MmoCommand<IMover>
    {
        public override Core.Enums.CommandId CommandId { get { return Core.Enums.CommandId.SetDestination; } }
        public Vector3 Position { get; set; }
        public override void Execute(IMover target)
        {
            target.SetDestination(this.Position);
        }

        public override bool CanExecute(IMover target)
        {
            return true;
        }



        public override Core.Enums.Destination CommandDestination
        {
            get { return Core.Enums.Destination.ServerAndClient; }
        }
    }
}
