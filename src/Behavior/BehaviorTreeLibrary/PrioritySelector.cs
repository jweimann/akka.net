using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BehaviorTreeLibrary
{
    public class PrioritySelector : Selector
    {
        private int _lastSelector;

        public PrioritySelector()
        {
            Update = () =>
                         {
                             _selector = 0;
                             for (;;)
                             {
                                 Status s = GetChild(_selector).Tick(this.DeltaTime);
                                 if (s != Status.BhFailure)
                                 {
                                     for (int i = _selector + 1; i <= _lastSelector; i++)
                                     {
                                         GetChild(i).Reset();
                                     }
                                     _lastSelector = _selector;
                                     return s;
                                 }
                                 if (++_selector == ChildCount)
                                 {
                                     return Status.BhFailure;
                                 }
                             }
                         };
        }
    }
}
