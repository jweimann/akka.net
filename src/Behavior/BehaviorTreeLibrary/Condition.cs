using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BehaviorTreeLibrary
{
    public class Condition : Behavior
    {
        public Func<bool> CanRun { protected get; set; }

        public Condition()
        {
            Update = () =>
                         {
                             if (CanRun != null && CanRun())
                             {
                                 return Status.BhSuccess;
                             }
                             return Status.BhFailure;
                         };
        }
    }
}
