using RTS.Commands.Base;
using RTS.Commands.Interfaces;
using RTS.Core.Enums;
using RTS.Core.Structs;
using RTS.Entities.Interfaces.Control;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RTS.Commands.Server
{
    [Serializable]
    public class JoinAreaOfInterestCommand :  EntityControllerCommand
    {
        public Int64 EntityId { get; set; }
        public string Name { get; set; }
        public Vector3 Position { get; set; }
        public override CommandId CommandId { get { return CommandId.JoinAreaOfInterestCommand; } }

        public override void Execute(IEntityController target)
        {
            target.AddEntity(EntityId, Name, Position, Sender);
        }

        public override bool CanExecute(IEntityController target)
        {
            return true;
        }

         
    }
}
