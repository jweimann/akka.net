using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BehaviorTreeLibrary;
using NUnit.Framework;

namespace BehaviorTreeTests
{
    public class DecoratorTests
    {
        [Test]
        public void Tick_ConditionTrue_RunsChild()
        {
            int health = 10;
            Decorator decorator = new Decorator();
            MockBehavior behavior = decorator.Add<MockBehavior>();

            decorator.CanRun = () =>
                                 {
                                     if (health < 50)
                                     {
                                         return true;
                                     }
                                     return false;
                                 };

            decorator.Tick();
            Assert.AreEqual(Status.BhRunning, behavior.Status);
        }

        [Test]
        public void Tick_ConditionFalse_ReturnsValue()
        {
            int health = 60;
            Decorator decorator = new Decorator();
            MockBehavior behavior = decorator.Add<MockBehavior>();
            decorator.ReturnStatus = Status.BhSuccess;
            decorator.CanRun = () =>
            {
                if (health < 50)
                {
                    return true;
                }
                return false;
            };

            decorator.Tick();
            Assert.AreEqual(Status.BhInvalid, behavior.Status);
            Assert.AreEqual(Status.BhSuccess, decorator.Status);
        }

    }
}
