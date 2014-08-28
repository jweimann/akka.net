using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BehaviorTreeLibrary
{
    public class Sequence : Composite
    {
        private int _sequence;

        public Sequence()
        {
            Update = () =>
                         {
                             for (;;)
                             {
                                 if (ChildCount == 0)
                                 {
                                     return BehaviorTreeLibrary.Status.BhInvalid;
                                 }
                                 Status s = GetChild(_sequence).Tick(this.DeltaTime);
                                 if (s != Status.BhSuccess)
                                 {
                                     if (s == Status.BhFailure)
                                     {
                                         _sequence = 0;
                                     }
                                     return s;
                                 }
                                 if (++_sequence == ChildCount)
                                 {
                                     _sequence = 0;
                                     return Status.BhSuccess;
                                 }
                             }
                         };

            Initialize = () => { _sequence = 0; };
        }

        public override void Reset()
        {
            Status = Status.BhInvalid;
        }
    }
}
