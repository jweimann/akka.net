using RTS.Commands.Interfaces;
using RTS.Core.Enums;
using RTS.Core.Structs;
using RTS.Entities.Interfaces.Control;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RTS.Commands
{
    [Serializable]
    public class SpawnEntityCommand : MmoCommand<IEntityController>, IEntityControllerCommand
    {
        public Vector3 Position { get; set; }
        public string Name { get; set; }
        public long EntityId { get; set; }

        public override Core.Enums.CommandId CommandId
        {
            get { return Core.Enums.CommandId.SpawnEntity; }
        }

        public override void Execute(IEntityController target)
        {
            target.SpawnEntity(Name, Position, this.EntityId);
        }

        public override bool CanExecute(IEntityController target)
        {
            return true;
        }
        public override Destination CommandDestination { get { return Destination.Client; } }
    }
}
