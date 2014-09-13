using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JW.Behavior.Interfaces
{
    public interface IBehavior
    {
        Action Completed { get; set; }
        void Tick(double deltaTime);

        IBehavior Then { get; set; }
    }
}
