using RTS.Commands.Base;
using RTS.Core.Structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RTS.Commands
{
    public class SendMovementToAOICommand : EntityControllerCommand
    {
        public Int64 EntityId { get; set; }
        public Vector3 Position { get; set; }
        public Vector3 Velocity { get; set; }

        public override void Execute(Entities.Interfaces.Control.IEntityController target)
        {
            target.MoveEntity(this);
        }

        public override bool CanExecute(Entities.Interfaces.Control.IEntityController target)
        {
            return (target != null);
        }

        public override Core.Enums.CommandId CommandId
        {
            get { return Core.Enums.CommandId.SendMovementToAOI; }
        }
    }
}
