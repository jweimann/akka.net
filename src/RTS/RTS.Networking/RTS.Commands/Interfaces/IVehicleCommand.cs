﻿using RTS.Entities.Interfaces.UnitTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RTS.Commands.Interfaces
{
    public interface IVehicleCommand : IEntityComponentCommand, IMmoCommand<IVehicle>
    {
        List<long> EntityIds { get; set; }
    }
}
