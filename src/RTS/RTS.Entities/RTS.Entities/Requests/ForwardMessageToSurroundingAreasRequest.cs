using RTS.Core.Structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTS.Entities.Requests
{
    public class ForwardMessageToSurroundingAreasRequest
    {
        public object Message { get; set; }
        public Vector3 SenderCenter { get; set; }
        public ForwardToSurroundingAreasMode Mode { get; set; }
    }

    public enum ForwardToSurroundingAreasMode
    {
        All,
        InRange,
        OutOfRange,
    }
}
