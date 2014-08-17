using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BehaviorTreeLibrary
{
    public abstract class Composite : Behavior
    {
        protected List<IBehavior> Children { get; set; }
 
        protected Composite()
        {
            Children = new List<IBehavior>();
            Initialize = () => { };
            Terminate = status => { };
            Update = () => Status.BhRunning;
        }

        public IBehavior GetChild(int index)
        {
            return Children[index];
        }

        public int ChildCount
        {
            get { return Children.Count; }
        }

        public void Add(Composite composite)
        {
            Children.Add(composite);
        }

        public T Add<T>() where T : class, IBehavior, new()
        {
            var t = new T {Parent = this};
            Children.Add(t);
            return t;
        }
    }

    public static class CompositeExtensions
    {

        public static Composite AddCondition(this Composite composite, Func<bool> behavior) 
        {
            composite.Add<Condition>().CanRun = behavior;
            return composite;
        }

        public static Composite AddBehavior(this Composite composite, Func<Status> behavior)
        {
            composite.Add<Behavior>().Update = behavior;
            return composite;
        }
    }
}
