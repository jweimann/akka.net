using Akka.Actor;
using RTS.Entities.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTS.Entities.Movement
{
    public class EntityDebugger : TypedActor,
        ILogReceive,
        IHandle<DebugEntityRequest>,
        IHandle<DebuggerStatusRequest>
    {
        private int _updateCount;
        private int _movedCount;
        private int _turnedCount;
        private int _longTickCount;
        private TimeSpan _totalUpdateDuration = new TimeSpan();
        private DateTime _lastUpdateTime;

        public void Handle(DebugEntityRequest message)
        {
            _updateCount++;
            if (message.Moved)
            {
                _movedCount++;
            }
            if (message.Turned)
            {
                _turnedCount++;
            }
            if (message.LongTick)
            {
                _longTickCount++;
            }
            _totalUpdateDuration += message.LastUpdateDuration;
        }

        public void Handle(DebuggerStatusRequest message)
        {
            return;
            bool longUpdate = (DateTime.Now - _lastUpdateTime).TotalMilliseconds > 2000;
            _lastUpdateTime = DateTime.Now;

            if (_longTickCount > 0 || longUpdate)
            {
                Console.ForegroundColor = ConsoleColor.Red;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Gray;
            }
            Console.WriteLine(String.Format("{0} Move Count: {1}  Turn Count: {2}  Long Ticks:{3} TotalUpdateDuration: {4} AvgUpdateDuration: {5:N10}",
                DateTime.Now.ToString(), 
                _movedCount, _turnedCount, 
                _longTickCount, 
                _totalUpdateDuration.ToString(),
                _totalUpdateDuration.TotalSeconds / _updateCount));
            
            _updateCount = 0;
            _movedCount = 0;
            _turnedCount = 0;
            _longTickCount = 0;
            _totalUpdateDuration = new TimeSpan();
        }
    }
}

