using RTS.Core.Structs;
using RTS.Entities.Interfaces.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RTS.Entities.Interfaces.Control
{
    public interface IController : ICommandHandler
    {
        void HandleRequest(object request);
    }
}
