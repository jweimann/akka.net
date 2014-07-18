using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RTS.Commands.Interfaces
{
    public interface IEntityComponentCommand
    {
        long EntityId { get; set; }
    }
}
