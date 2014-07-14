using RTS.Core.Structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTS.Entities.Requests
{
    public class MoveRequest : EntityRequestDepriciated
    {
        public Vector3 Position { get; set; }
        public Vector3 Velocity { get; set; }
        public bool Dirty { get; set; }
    }
}
