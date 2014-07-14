using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTS.Entities.Requests
{
    public class DebuggerStatusResponse
    {
        public int MovedCount { get; set; }
        public int TurnedCount { get; set; }
    }
}
