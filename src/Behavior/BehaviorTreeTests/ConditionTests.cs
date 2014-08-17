using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BehaviorTreeLibrary;
using NUnit.Framework;

namespace BehaviorTreeTests
{
    public class ConditionTests
    {
        [Test]
        public void Tick_ConditionTrue_ReturnsSuccess()
        {
            int health = 10;
            Condition condition = new Condition();
            condition.CanRun = () =>
                                 {
                                     if (health < 50)
                                     {
                                         return true;
                                     }
                                     return false;
                                 };
            condition.Tick();
            Assert.AreEqual(Status.BhSuccess, condition.Status);
        }

        [Test]
        public void Tick_ConditionFalse_ReturnFailure()
        {
            int health = 60;
            Condition condition = new Condition();
            condition.CanRun = () =>
            {
                if (health < 50)
                {
                    return true;
                }
                return false;
            };
            condition.Tick();
            Assert.AreEqual(Status.BhFailure, condition.Status);
        }

    }
}
