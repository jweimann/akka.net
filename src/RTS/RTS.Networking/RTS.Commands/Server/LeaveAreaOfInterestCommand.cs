using RTS.Commands.Base;
using RTS.Core.Structs;
using RTS.Entities.Interfaces.Control;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RTS.Commands.Server
{
    [Serializable]
    public class LeaveAreaOfInterestCommand : EntityControllerCommand
    {
        public Int64 EntityId { get; set; }
        public Vector3 Position { get; set; }
        public override void Execute(IEntityController target)
        {
            target.RemoveEntity(EntityId);
        }

        public override bool CanExecute(IEntityController target)
        {
            throw new NotImplementedException();
        }

        public override Core.Enums.CommandId CommandId
        {
            get { throw new NotImplementedException(); }
        }

        public override string ToString()
        {
            return this.GetType().ToString() + " AoiId: " + AoiId + "  EntityId: " + EntityId + "  Position: " + Position.ToString();
        }
    }
}
