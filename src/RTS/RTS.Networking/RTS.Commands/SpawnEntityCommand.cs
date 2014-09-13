using RTS.Commands.Interfaces;
using RTS.Core.Enums;
using RTS.Core.Structs;
using RTS.DataStructures;
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
        public long TeamId { get; set; }
        public List<Stat> Stats { get; set; }

        public SpawnEntityCommand(Vector3 position, string name, long entityId, long teamId, List<Stat> stats)
        {
            this.Position = position;
            this.Name = name;
            this.EntityId = entityId;
            this.TeamId = teamId;
            this.Stats = stats;
        }

        public override Core.Enums.CommandId CommandId
        {
            get { return Core.Enums.CommandId.SpawnEntity; }
        }

        public override void Execute(IEntityController target)
        {
            target.SpawnEntity(Name, Position, this.EntityId, this.TeamId, this.Stats);
        }

        public override bool CanExecute(IEntityController target)
        {
            return true;
        }
        public override Destination CommandDestination { get { return Destination.Client; } }
    }
}
