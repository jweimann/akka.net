using RTS.Commands.Base;
using RTS.Commands.Interfaces;
using RTS.Core.Structs;
using RTS.Entities.Interfaces.Control;
using RTS.Entities.Interfaces.Stats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RTS.Commands
{
    [Serializable]
    public class EntitiesInAreaListCommand : EntityControllerCommand
    {
        public override Core.Enums.CommandId CommandId
        {
            get { return Core.Enums.CommandId.EntitiesInAreaList; }
        }
        public object Sender { get; set; }
        public long[] EntityIds { get; set;}
        public string[] Names { get; set;}
        public Vector3[] Locations { get; set; }

        public EntitiesInAreaListCommand() { }
        public EntitiesInAreaListCommand(int size)
        {
            this.EntityIds = new Int64[size];
            this.Locations = new Vector3[size];
            this.Names = new string[size];
        }

        public override void Execute(IEntityController target)
        {
            for (int i = 0; i < this.EntityIds.Length; i++)
            {
                target.AddEntity(this.EntityIds[i], this.Names[i], this.Locations[i], this.Sender);
            }
        }

        public override bool CanExecute(IEntityController target)
        {
            throw new NotImplementedException();
        }

        
    }
}
