using RTS.Commands.Base;
using RTS.Core.Structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RTS.Commands
{
    [Serializable]
    public class AOIDetailsCommand : EntityControllerCommand
    {
        public Vector3 Center { get; set; }
        public Vector3 Extents { get; set; }
        public override void Execute(Entities.Interfaces.Control.IEntityController target)
        {
            target.SetBounds(new Bounds(Center, Extents));
        }

        public override bool CanExecute(Entities.Interfaces.Control.IEntityController target)
        {
            return (target != null);
        }

        public override Core.Enums.CommandId CommandId
        {
            get { return Core.Enums.CommandId.AOIDetailsCommand; }
        }

        public override string ToString()
        {
            return "AOIDetailsCommand " + Center.ToString();
        }
    }
}
