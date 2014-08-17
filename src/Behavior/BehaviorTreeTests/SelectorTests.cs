using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BehaviorTreeLibrary;
using NUnit.Framework;

namespace BehaviorTreeTests
{
    public class SelectorTests
    {
        [Test]
        public void Tick_PassThroughToSibling_ReturnTerminated()
        {
            Status[] status = {Status.BhSuccess, Status.BhFailure,};
            for (int i = 0; i < 2; i++)
            {
                MockSelector selector = new MockSelector(1);

                selector.Tick();
                Assert.AreEqual(selector.Status, Status.BhRunning);
                Assert.AreEqual(0, selector[0]._iTerminateCalled);

                selector[0]._eReturnStatus = status[i];
                selector.Tick();
                Assert.AreEqual(selector.Status, status[i]);
                Assert.AreEqual(1, selector[0]._iTerminateCalled);
            }
        }

        [Test]
        public void Tick_SelectorFirstFailureSecondRuns_EndsWithSuccess()
        {
            MockSelector selector = new MockSelector(2);

            selector.Tick();
            Assert.AreEqual(selector.Status, Status.BhRunning);
            Assert.AreEqual(0, selector[0]._iTerminateCalled);

            selector[0]._eReturnStatus = Status.BhFailure;
            selector.Tick();
            Assert.AreEqual(selector.Status, Status.BhRunning);
            Assert.AreEqual(1, selector[0]._iTerminateCalled);
            Assert.AreEqual(1, selector[1]._iInitializeCalled);
        }

        [Test]
        public void Tick_SelectorFirstSecondNotInitialized_EndsWithSuccess()
        {
            MockSelector selector = new MockSelector(2);

            selector.Tick();
            Assert.AreEqual(selector.Status, Status.BhRunning);
            Assert.AreEqual(0, selector[0]._iTerminateCalled);

            selector[0]._eReturnStatus = Status.BhSuccess;
            selector.Tick();
            Assert.AreEqual(selector.Status, Status.BhSuccess);
            Assert.AreEqual(1, selector[0]._iTerminateCalled);
            Assert.AreEqual(0, selector[1]._iInitializeCalled);
        }

        [Test]
        public void Tick_OnSecondTick_SelectorStartsWhereItLeftOff()
        {
            Selector selector = new Selector();
            MockBehavior behavior = selector.Add<MockBehavior>();
            MockBehavior behavior1 = selector.Add<MockBehavior>();
            MockBehavior behavior2 = selector.Add<MockBehavior>();
            behavior._eReturnStatus = Status.BhFailure;
            behavior1._eReturnStatus = Status.BhRunning;
            behavior2._eReturnStatus = Status.BhRunning;

            selector.Tick();
            Assert.AreEqual(Status.BhRunning, selector.Status);

            behavior._eReturnStatus = Status.BhRunning;
            // Left off on Behavior1.
            behavior1._eReturnStatus = Status.BhFailure;
            behavior2._eReturnStatus = Status.BhFailure;

            selector.Tick();
            Assert.AreEqual(Status.BhFailure, selector.Status);
        }
    }
}
