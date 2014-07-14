using RTS.Core.Enums;
using RTS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RTS.Commands.Interfaces
{
    //public interface IMmoCommand //TODO: Remove this and use generics for the methods...
    //{
    //    CommandId CommandId { get; }
    //    Destination CommandDestination { get; }

    //}
    public interface IMmoCommand<T>// : IMmoCommand 
    {
        CommandId CommandId { get; }
        Destination CommandDestination { get; }
        void Execute(T target);
        bool CanExecute(T target);

    }
}
