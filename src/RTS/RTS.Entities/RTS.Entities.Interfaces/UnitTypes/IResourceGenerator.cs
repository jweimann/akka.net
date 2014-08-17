using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RTS.Entities.Interfaces.UnitTypes
{
    public interface IResourceGenerator : IEntityComponent
    {
        int Amount { get; set;}
        double Interval { get; set; }
    }
}
