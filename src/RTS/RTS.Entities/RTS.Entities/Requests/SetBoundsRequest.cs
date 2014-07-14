using RTS.Commands.Base;
using RTS.Core.Structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTS.Entities.Requests
{
    public class SetBoundsRequest : EntityControllerCommand
    {
        public Bounds Bounds { get; set; }

        public override void Execute(Interfaces.Control.IEntityController target)
        {
            target.SetBounds(Bounds);
        }

        public override bool CanExecute(Interfaces.Control.IEntityController target)
        {
            return (target != null);
        }

        public override Core.Enums.CommandId CommandId
        {
            get { return Core.Enums.CommandId.SetBounds; }
        }

        public override string ToString()
        {
            return "AOI " + this.Bounds.ToString();
        }
    }
}
