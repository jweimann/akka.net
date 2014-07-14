using RTS.Commands.Base;
using RTS.Commands.Interfaces;
using RTS.Core.Structs;
using RTS.Entities.Interfaces.Control;
using RTS.Entities.Interfaces.Movement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RTS.Commands
{
    [Serializable]
    public class BatchMoveCommand : EntityControllerCommand
    {
        public override Core.Enums.CommandId CommandId
        {
            get { return Core.Enums.CommandId.BatchMove; }
        }
        public Int64[] EntityId { get; set; }
        public Vector3[] Position { get; set; }
        public Vector3[] Velocity { get; set; }
        public override void Execute(IEntityController controller)
        {
            var targets = controller.GetEntities();
            for (int i = 0; i < EntityId.Length; i++)
			{
                if (targets.ContainsKey(EntityId[i]))
                {
                    var target = targets[EntityId[i]];
                    target.SetPosition(this.Position[i]);
                    target.SetVelocity(this.Velocity[i]);
                }
            }
        }
        public override bool CanExecute(IEntityController controller)
        {
            return controller != null && controller.GetEntities() != null;
        }
        public override string ToString()
        {
            return String.Format("BatchMoveCommand  EntityId.Length: {0}  Position.Length: {1}  Velocity.Length: {2}", EntityId.Length, Position.Length, Velocity.Length);
        }

 
    }
}
// Add move command to client