using RTS.Entities.Interfaces.Control;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RTS.Commands.Interfaces
{
    public interface IEntityControllerCommand : IMmoCommand<IEntityController>
    {
        //object Sender { get; set; }
        //int AoiId { get; set; }

    }
}
