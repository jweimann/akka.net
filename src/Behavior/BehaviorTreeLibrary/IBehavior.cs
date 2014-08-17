using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BehaviorTreeLibrary
{
    public interface IBehavior
    {
        Status Status { get; set; }
        Action Initialize { set; }
        Func<Status> Update { set; }
        Action<Status> Terminate { set; }
        Composite Parent { get; set; }

        Status Tick(double deltaTime);
        void Reset();
        Composite End();
        IBehavior SetUpdate(Func<Status> update);
        IBehavior SetInitialize(Action init);
        IBehavior SetTerminate(Action<Status> term);
    }
}
