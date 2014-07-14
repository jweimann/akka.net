using RTS.Entities.Interfaces.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RTS.Entities.Requests
{
    public class EntityRequestDepriciated : IEntityRequest
    {
        public Int64 EntityId { get; set; }
    }
}
