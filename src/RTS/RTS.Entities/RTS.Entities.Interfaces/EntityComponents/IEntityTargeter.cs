using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RTS.Entities.Interfaces.EntityComponents
{
    /// <summary>
    /// Targets other entities.
    /// </summary>
    public interface IEntityTargeter : IEntityComponent
    {
        void SetTarget(long entityId);
        void ClearTarget();

    }
}
