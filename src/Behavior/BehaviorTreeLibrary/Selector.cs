﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BehaviorTreeLibrary
{
    public class Selector : Composite
    {
        protected int _selector;
        public Selector()
        {
            Update = () =>
                         {
                             for (;;)
                             {
                                 Status s = GetChild(_selector).Tick(this.DeltaTime);
                                 if (s != Status.BhFailure)
                                 {
                                     if (s == Status.BhSuccess)
                                     {
                                         _selector = 0;
                                     }
                                     return s;
                                 }
                                 if (++_selector == ChildCount)
                                 {
                                     _selector = 0;
                                     return Status.BhFailure;
                                 }
                             }
                         };
            Initialize = () =>
                             {
                                 _selector = 0;
                             };
        }

        public override void Reset()
        {
            Status = Status.BhInvalid;
        }
    }
}
