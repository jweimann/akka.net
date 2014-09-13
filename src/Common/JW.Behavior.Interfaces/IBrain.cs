using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JW.Behavior.Interfaces
{
    public interface IBrain
    {
        void AddBehavior(JW.Behavior.Interfaces.IBehavior behavior);
        void Tick(double deltaTime);
    }
}
