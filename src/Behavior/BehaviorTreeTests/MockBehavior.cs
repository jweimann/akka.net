using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BehaviorTreeLibrary;

namespace BehaviorTreeTests
{
    public class MockBehavior : Behavior
    {
        public int _iInitializeCalled;
        public int _iUpdateCalled;
        public int _iTerminateCalled;

        public Status _eReturnStatus;
        public Status _eTerminateStatus;

        public MockBehavior()
        {
            _iInitializeCalled = 0;
            _iUpdateCalled = 0;
            _iTerminateCalled = 0;

            _eReturnStatus = Status.BhRunning;
            _eTerminateStatus = Status.BhInvalid;

            Initialize = OnInitialize;
            Update = DoUpdate;
            Terminate = OnTerminate;
        }

        private void OnTerminate(Status obj)
        {
            ++_iTerminateCalled;
            _eTerminateStatus = obj;
        }

        public Status DoUpdate()
        {
            ++_iUpdateCalled;
            return _eReturnStatus;
        }

        public void OnInitialize()
        {
            ++_iInitializeCalled;
        }
    }
}
