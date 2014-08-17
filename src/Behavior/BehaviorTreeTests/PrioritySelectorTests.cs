using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BehaviorTreeLibrary;
using NUnit.Framework;

namespace BehaviorTreeTests
{
    public class PrioritySelectorTests
    {
        [Test]
        public void Tick_OnSecondTick_RerunsChildren()
        {
            PrioritySelector selector = new PrioritySelector();
            MockBehavior behavior = selector.Add<MockBehavior>();
            MockBehavior behavior1 = selector.Add<MockBehavior>();
            MockBehavior behavior2 = selector.Add<MockBehavior>();
            behavior._eReturnStatus = Status.BhFailure;
            behavior1._eReturnStatus = Status.BhRunning;
            behavior2._eReturnStatus = Status.BhRunning;

            selector.Tick();
            Assert.AreEqual(Status.BhRunning, selector.Status);

            behavior._eReturnStatus = Status.BhRunning;
            behavior1._eReturnStatus = Status.BhFailure;
            behavior2._eReturnStatus = Status.BhFailure;

            selector.Tick();
            Assert.AreEqual(Status.BhRunning, selector.Status);

        }

        [Test]
        public void Tick_OnSecondTick_ResetsSequences()
        {
            PrioritySelector selector = new PrioritySelector();
            MockSequence sequence = new MockSequence(2);
            MockSequence sequence1 = new MockSequence(2);

            selector.Add(sequence);
            selector.Add(sequence1);

            selector.Tick();
            Assert.AreEqual(Status.BhRunning, selector.Status);
            sequence[0]._eReturnStatus = Status.BhSuccess;
            sequence[1]._eReturnStatus = Status.BhFailure;

            selector.Tick();
            Assert.AreEqual(Status.BhRunning, selector.Status);
            Assert.AreEqual(Status.BhRunning, sequence1.Status);

            sequence1[0]._eReturnStatus = Status.BhSuccess;
            sequence1[1]._eReturnStatus = Status.BhFailure;

            selector.Tick();
            Assert.AreEqual(Status.BhFailure, selector.Status);
            Assert.AreEqual(Status.BhFailure, sequence1.Status);

            sequence1[0]._eReturnStatus = Status.BhSuccess;
            sequence1[1]._eReturnStatus = Status.BhRunning;

            selector.Tick();
            Assert.AreEqual(Status.BhRunning, selector.Status);
            Assert.AreEqual(Status.BhRunning, sequence1.Status);

            sequence[0]._eReturnStatus = Status.BhRunning;
            selector.Tick();
            Assert.AreEqual(Status.BhRunning, selector.Status);
            Assert.AreEqual(Status.BhInvalid, sequence1.Status);
        }

    }
}
