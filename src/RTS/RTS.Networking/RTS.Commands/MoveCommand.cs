using RTS.Commands.Interfaces;
using RTS.Core.Structs;
using RTS.Entities.Interfaces.Movement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RTS.Commands
{
    [Serializable]
    public class MoveCommand : MmoCommand<IMover>
    {
        public override Core.Enums.CommandId CommandId
        {
            get { return Core.Enums.CommandId.Move; }
        }
        public Int64 EntityId { get; set; }
        public Vector3 Position { get; set; }
        public Vector3 Velocity { get; set; }
        public override void Execute(IMover target)
        {
            target.SetPosition(this.Position);
            target.SetVelocity(this.Velocity);
        }
        public override bool CanExecute(IMover target)
        {
            return target != null;
        }
        public override string ToString()
        {
            return String.Format("MoveCommand  EntityId: {0}  Position: {1}  Velocity: {2}", EntityId, Position.ToString(), Velocity.ToString());
        }
        public override Core.Enums.Destination CommandDestination { get { return Core.Enums.Destination.ServerAndClient; } }
    }
}
// Add move command to client