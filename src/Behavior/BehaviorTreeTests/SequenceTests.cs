using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BehaviorTreeLibrary;
using NUnit.Framework;

namespace BehaviorTreeTests
{
    public class SequenceTests
    {
        [Test]
        public void Tick_OnePassThrough_ReturnsResult()
        {
            Status[] status = {Status.BhSuccess, Status.BhFailure,};
            for (int i = 0; i < 2; i++)
            {
                MockSequence sequence = new MockSequence(1);

                sequence.Tick();
                Assert.AreEqual(sequence.Status, Status.BhRunning);
                Assert.AreEqual(0, sequence[0]._iTerminateCalled);

                sequence[0]._eReturnStatus = status[i];
                sequence.Tick();
                Assert.AreEqual(sequence.Status, status[i]);
                Assert.AreEqual(1, sequence[0]._iTerminateCalled);
            }
        }

        [Test]
        public void Tick_TwoChildrenFails_ReturnsFailure()
        {
            MockSequence sequence = new MockSequence(2);

            sequence.Tick();
            Assert.AreEqual(sequence.Status, Status.BhRunning);
            Assert.AreEqual(0, sequence[0]._iTerminateCalled);

            sequence[0]._eReturnStatus = Status.BhFailure;
            sequence.Tick();
            Assert.AreEqual(sequence.Status, Status.BhFailure);
            Assert.AreEqual(1, sequence[0]._iTerminateCalled);
            Assert.AreEqual(0, sequence[1]._iInitializeCalled);
        }

        [Test]
        public void Tick_TwoChildrenSucceed_ReturnsSuccess()
        {
            MockSequence sequence = new MockSequence(2);

            sequence.Tick();
            Assert.AreEqual(sequence.Status, Status.BhRunning);
            Assert.AreEqual(0, sequence[0]._iTerminateCalled);

            sequence[0]._eReturnStatus = Status.BhSuccess;
            sequence.Tick();
            Assert.AreEqual(sequence.Status, Status.BhRunning);
            Assert.AreEqual(1, sequence[0]._iTerminateCalled);
            Assert.AreEqual(1, sequence[1]._iInitializeCalled);

            sequence[1]._eReturnStatus = Status.BhSuccess;
            sequence.Tick();
            Assert.AreEqual(sequence.Status, Status.BhSuccess);
            Assert.AreEqual(1, sequence[1]._iTerminateCalled);
        }
    }
}
