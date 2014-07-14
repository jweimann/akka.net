using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTS.Entities.Requests
{
    public class DebugEntityRequest
    {
        public bool Moved { get; set; }
        public bool Turned { get; set; }
        public bool LongTick { get; set; }
        public string EntityId { get; set; }
        public TimeSpan LastUpdateDuration { get; set; }
    }
}
