using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BehaviorTreeLibrary;
using NUnit.Framework;

namespace BehaviorTreeTests
{
    public class BehaviorTreeTests
    {
        [Test]
        public void Tick_SelectorParentSequenceChildren_SelectorSucceeds()
        {
            Selector selector = new Selector();
            MockSequence sequence1 = new MockSequence(2);
            MockSequence sequence2 = new MockSequence(2);
            selector.Add(sequence1);
            selector.Add(sequence2);

            selector.Tick();
            Assert.AreEqual(Status.BhRunning, selector.Status);
            Assert.AreEqual(0, sequence1[0]._iTerminateCalled);
            Assert.AreEqual(1, sequence1[0]._iInitializeCalled);

            sequence1[0]._eReturnStatus = Status.BhSuccess;
            selector.Tick();
            Assert.AreEqual(Status.BhRunning, selector.Status);
            Assert.AreEqual(1, sequence1[0]._iTerminateCalled);
            Assert.AreEqual(1, sequence1[1]._iInitializeCalled);

            sequence1[1]._eReturnStatus = Status.BhSuccess;
            selector.Tick();
            Assert.AreEqual(Status.BhSuccess, selector.Status);
            Assert.AreEqual(1, sequence1[1]._iTerminateCalled);
        }

        [Test]
        public void Tick_SelectorParentSequenceChildren_SelectorSucceedsOnSecondSequence()
        {
            Selector selector = new Selector();
            MockSequence sequence1 = new MockSequence(2);
            MockSequence sequence2 = new MockSequence(2);
            selector.Add(sequence1);
            selector.Add(sequence2);

            selector.Tick();
            Assert.AreEqual(Status.BhRunning, selector.Status);
            Assert.AreEqual(0, sequence1[0]._iTerminateCalled);
            Assert.AreEqual(1, sequence1[0]._iInitializeCalled);

            sequence1[0]._eReturnStatus = Status.BhFailure;
            selector.Tick();
            Assert.AreEqual(Status.BhRunning, selector.Status);
            Assert.AreEqual(1, sequence1[0]._iTerminateCalled);
            Assert.AreEqual(0, sequence1[1]._iInitializeCalled);
            Assert.AreEqual(1, sequence2[0]._iInitializeCalled);

            sequence2[0]._eReturnStatus = Status.BhSuccess;
            selector.Tick();
            Assert.AreEqual(Status.BhRunning, selector.Status);
            Assert.AreEqual(1, sequence2[0]._iTerminateCalled);
            Assert.AreEqual(1, sequence2[1]._iInitializeCalled);

            sequence2[1]._eReturnStatus = Status.BhSuccess;
            selector.Tick();
            Assert.AreEqual(Status.BhSuccess, selector.Status);
            Assert.AreEqual(1, sequence2[1]._iTerminateCalled);
        }

        [Test]
        public void Tick_SelectorParentSequenceChildren_SelectorFailsOnSecondSequence()
        {
            Selector selector = new Selector();
            MockSequence sequence1 = new MockSequence(2);
            MockSequence sequence2 = new MockSequence(2);
            selector.Add(sequence1);
            selector.Add(sequence2);

            selector.Tick();
            Assert.AreEqual(Status.BhRunning, selector.Status);
            Assert.AreEqual(0, sequence1[0]._iTerminateCalled);
            Assert.AreEqual(1, sequence1[0]._iInitializeCalled);

            sequence1[0]._eReturnStatus = Status.BhFailure;
            selector.Tick();
            Assert.AreEqual(Status.BhRunning, selector.Status);
            Assert.AreEqual(1, sequence1[0]._iTerminateCalled);
            Assert.AreEqual(0, sequence1[1]._iInitializeCalled);
            Assert.AreEqual(1, sequence2[0]._iInitializeCalled);

            sequence2[0]._eReturnStatus = Status.BhSuccess;
            selector.Tick();
            Assert.AreEqual(Status.BhRunning, selector.Status);
            Assert.AreEqual(1, sequence2[0]._iTerminateCalled);
            Assert.AreEqual(1, sequence2[1]._iInitializeCalled);

            sequence2[1]._eReturnStatus = Status.BhFailure;
            selector.Tick();
            Assert.AreEqual(Status.BhFailure, selector.Status);
            Assert.AreEqual(1, sequence2[1]._iTerminateCalled);
        }

        [Test]
        public void Tick_SequenceParentSelectorChildren_SequenceSucceeds()
        {
            Sequence sequence = new Sequence();
            MockSelector selector1 = new MockSelector(2);
            MockSelector selector2 = new MockSelector(2);
            sequence.Add(selector1);
            sequence.Add(selector2);

            sequence.Tick();
            Assert.AreEqual(Status.BhRunning, sequence.Status);
            Assert.AreEqual(1, selector1[0]._iInitializeCalled);

            selector1[0]._eReturnStatus = Status.BhSuccess;
            sequence.Tick();
            Assert.AreEqual(Status.BhRunning, sequence.Status);
            Assert.AreEqual(Status.BhInvalid, selector1[1].Status);
            Assert.AreEqual(1, selector1[0]._iTerminateCalled);
            Assert.AreEqual(1, selector2[0]._iInitializeCalled);

            sequence.Tick();
            Assert.AreEqual(Status.BhRunning, sequence.Status);
            Assert.AreEqual(0, selector2[0]._iTerminateCalled);
            Assert.AreEqual(0, selector2[1]._iTerminateCalled);
            Assert.AreEqual(Status.BhRunning, selector2[0].Status);

            selector2[0]._eReturnStatus = Status.BhSuccess;
            sequence.Tick();
            Assert.AreEqual(Status.BhSuccess, sequence.Status);
        }

        [Test]
        public void Tick_SequenceParentSelectorChildrenFirstChildFails_SequenceSucceeds()
        {
            Sequence sequence = new Sequence();
            MockSelector selector1 = new MockSelector(2);
            MockSelector selector2 = new MockSelector(2);
            sequence.Add(selector1);
            sequence.Add(selector2);

            sequence.Tick();
            Assert.AreEqual(Status.BhRunning, sequence.Status);
            Assert.AreEqual(1, selector1[0]._iInitializeCalled);

            selector1[0]._eReturnStatus = Status.BhFailure;
            sequence.Tick();
            Assert.AreEqual(Status.BhRunning, sequence.Status);
            Assert.AreEqual(Status.BhRunning, selector1[1].Status);
            Assert.AreEqual(1, selector1[0]._iTerminateCalled);
            Assert.AreEqual(0, selector2[0]._iInitializeCalled);
            Assert.AreEqual(1, selector1[1]._iInitializeCalled);

            selector1[1]._eReturnStatus = Status.BhSuccess;
            sequence.Tick();
            Assert.AreEqual(Status.BhRunning, sequence.Status);
            Assert.AreEqual(Status.BhRunning, selector2[0].Status);
            Assert.AreEqual(1, selector1[1]._iTerminateCalled);
            Assert.AreEqual(1, selector2[0]._iInitializeCalled);

            selector2[0]._eReturnStatus = Status.BhSuccess;
            sequence.Tick();
            Assert.AreEqual(Status.BhSuccess, sequence.Status);
        }

        [Test]
        public void Tick_SequenceParentSelectorChildrenFirstChildFailsOnBoth_SequenceFails()
        {
            Sequence sequence = new Sequence();
            MockSelector selector1 = new MockSelector(2);
            MockSelector selector2 = new MockSelector(2);
            sequence.Add(selector1);
            sequence.Add(selector2);

            sequence.Tick();
            Assert.AreEqual(Status.BhRunning, sequence.Status);
            Assert.AreEqual(1, selector1[0]._iInitializeCalled);

            selector1[0]._eReturnStatus = Status.BhFailure;
            sequence.Tick();
            Assert.AreEqual(Status.BhRunning, sequence.Status);
            Assert.AreEqual(Status.BhRunning, selector1[1].Status);
            Assert.AreEqual(1, selector1[0]._iTerminateCalled);
            Assert.AreEqual(0, selector2[0]._iInitializeCalled);
            Assert.AreEqual(1, selector1[1]._iInitializeCalled);

            selector1[1]._eReturnStatus = Status.BhSuccess;
            sequence.Tick();
            Assert.AreEqual(Status.BhRunning, sequence.Status);
            Assert.AreEqual(Status.BhRunning, selector2[0].Status);
            Assert.AreEqual(1, selector1[1]._iTerminateCalled);
            Assert.AreEqual(1, selector2[0]._iInitializeCalled);

            selector2[0]._eReturnStatus = Status.BhFailure;
            sequence.Tick();
            Assert.AreEqual(Status.BhRunning, sequence.Status);

            selector2[1]._eReturnStatus = Status.BhFailure;
            sequence.Tick();
            Assert.AreEqual(Status.BhFailure, sequence.Status);

        }

        [Test]
        public void Tick_SequenceParentSelectorChildrenFirstSelectrFailsOnBothChildren_SequenceFails()
        {
            Sequence sequence = new Sequence();
            MockSelector selector1 = new MockSelector(2);
            MockSelector selector2 = new MockSelector(2);
            sequence.Add(selector1);
            sequence.Add(selector2);

            sequence.Tick();
            Assert.AreEqual(Status.BhRunning, sequence.Status);
            Assert.AreEqual(1, selector1[0]._iInitializeCalled);

            selector1[0]._eReturnStatus = Status.BhFailure;
            sequence.Tick();
            Assert.AreEqual(Status.BhRunning, sequence.Status);
            Assert.AreEqual(Status.BhRunning, selector1[1].Status);
            Assert.AreEqual(1, selector1[0]._iTerminateCalled);
            Assert.AreEqual(0, selector2[0]._iInitializeCalled);
            Assert.AreEqual(1, selector1[1]._iInitializeCalled);

            selector1[1]._eReturnStatus = Status.BhFailure;
            sequence.Tick();
            Assert.AreEqual(Status.BhFailure, sequence.Status);
            Assert.AreEqual(Status.BhInvalid, selector2[0].Status);
            Assert.AreEqual(1, selector1[1]._iTerminateCalled);
            Assert.AreEqual(0, selector2[0]._iInitializeCalled);

        }

    }
}
