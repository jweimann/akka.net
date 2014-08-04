using RTS.Commands.Interfaces;
using RTS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RTS.Commands
{
    public static class CommandMatch
    {
        //[Obsolete("This is horribly slow, should be replaced with standard 'is' checks",false)]
        public static Case Match(this object target)
        {
            return new Case(target);
        }
      
    }
        public class Case
        {
            private readonly object _message;
            private bool _handled;

            public Case(object message)
            {
                _message = message;
            }

            public Case WithServer<TMessage>(Action action) where TMessage : class
            {
                IMmoCommand cmd = _message as IMmoCommand;
                var mmoCmd = _message as MmoCommand;

                if (cmd != null && mmoCmd != null && cmd.IsHandledBy<TMessage>() && ((MmoCommand)_message).TellServer)
                {
                    action();
                    _handled = true;
                    return this;
                }

                if (!_handled && _message is TMessage)
                {
                    action();
                    _handled = true;
                }

                return this;
            }

            public Case With<TMessage>(Action<TMessage> action)
            {
                if (!_handled && _message is TMessage)
                {
                    action((TMessage)_message);
                    _handled = true;
                }

                return this;
            }

            public Case WithClient<TMessage>(Action action) where TMessage : class
            {
                IMmoCommand cmd = _message as IMmoCommand;
                var mmoCmd = _message as MmoCommand;
                if (cmd != null && mmoCmd != null && cmd.IsHandledBy<TMessage>() && ((MmoCommand)_message).TellClient)
                {
                    action();
                    _handled = true;
                    return this;
                }

                if (!_handled && _message is TMessage)
                {
                    action();
                    _handled = true;
                }

                return this;
            }

            //public Case WithClient<TMessage>(Action<TMessage> action) where TMessage : class
            //{
            //    if (!_handled && _message is TMessage && ((MmoCommand)_message).TellClient)
            //    {
            //        action((TMessage)_message);
            //        _handled = true;
            //    }

            //    return this;
            //}

            public void Default(Action<object> action)
            {
                if (!_handled)
                {
                    action(_message);
                    _handled = true;
                }
            }

        

        }
    
}
