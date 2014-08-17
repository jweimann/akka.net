using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BehaviorTreeLibrary
{
    public class Behavior : IBehavior
    {
        public Action Initialize { protected get; set; }
        public Func<Status> Update { protected get; set; }
        public Action<Status> Terminate { protected get; set; }
        public Composite Parent { get; set; }
        public Status Status { get; set; }

        protected double DeltaTime { private set; get; }

        public Status Tick(double deltaTime = 0.0)
        {
            this.DeltaTime = deltaTime;
            if (Status == Status.BhInvalid && Initialize != null)
            {
                Initialize();
            }

            Status = Update();

            if (Status != Status.BhRunning && Terminate != null)
            {
                Terminate(Status);
            }

            return Status;
        }

        public virtual void Reset() {}

        public Composite End()
        {
            return Parent;
        }

        public IBehavior SetUpdate(Func<Status> update)
        {
            Update = update;
            return this;
        }

        public IBehavior SetInitialize(Action init)
        {
            Initialize = init;
            return this;
        }

        public IBehavior SetTerminate(Action<Status> term)
        {
            Terminate = term;
            return this;
        }
    }
}
