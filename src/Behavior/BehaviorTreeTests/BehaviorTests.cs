using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BehaviorTreeLibrary;
using NUnit.Framework;

namespace BehaviorTreeTests
{
    public class BehaviorTests
    {
        [Test]
        public void Tick_DoesInitialize_Successful()
        {
            MockBehavior t = new MockBehavior();

            Assert.AreEqual(0, t._iInitializeCalled);

            t.Tick();

            Assert.AreEqual(1, t._iInitializeCalled);
        }

        [Test]
        public void Tick_UpdateCalled_ReturnsSuccess()
        {
            MockBehavior t = new MockBehavior();

            t.Tick();
            Assert.AreEqual(1, t._iUpdateCalled);

            t._eReturnStatus = Status.BhSuccess;

            t.Tick();
            Assert.AreEqual(2, t._iUpdateCalled);
        }

        [Test]
        public void Tick_TerminteCalled_ReturnsSuccess()
        {
            MockBehavior t = new MockBehavior();

            t.Tick();
            Assert.AreEqual(0, t._iTerminateCalled);

            t._eReturnStatus = Status.BhSuccess;
            t.Tick();
            Assert.AreEqual(1, t._iTerminateCalled);
        }
    }
}
