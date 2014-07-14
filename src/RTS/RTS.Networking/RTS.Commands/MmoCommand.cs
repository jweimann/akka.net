using NetSerializer;
using RTS.Commands.Interfaces;
using RTS.Core.Enums;
using RTS.Core.Structs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace RTS.Commands
{
    [Serializable]
    public abstract class MmoCommand
    {
        public static T FromBytes<T>(byte[] bytes)
        {
            using (MemoryStream stream = new MemoryStream(bytes))
            {
                object obj = Serializer.Deserialize(stream);
                if (obj is T)
                {
                    return (T)obj;
                }
                else
                {
                    return default(T);
                }
            }
        }
    }

    [Serializable]
    public abstract class MmoCommand<T> : MmoCommand, IMmoCommand<T>
    {
        public override string ToString()
        {
            return base.ToString();
        }

        public abstract void Execute(T target);

        public abstract bool CanExecute(T target);

        public abstract Core.Enums.CommandId CommandId { get; }

        public abstract Destination CommandDestination { get; }

        public bool TellServer { get { return CommandDestination == Destination.Server || CommandDestination == Destination.ServerAndClient; } }
        public bool TellClient { get { return CommandDestination == Destination.Client || CommandDestination == Destination.ServerAndClient; } }
        
    }
}