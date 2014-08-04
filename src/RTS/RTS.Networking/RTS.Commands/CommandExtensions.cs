using NetSerializer;
using RTS.Commands.Interfaces;
using RTS.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace RTS.Commands
{
    public static class CommandExtensions
    {
        public static byte[] ToBytes<T>(this IMmoCommand<T> command)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                Serializer.Serialize(stream, command);
                return stream.ToArray();
            }
        }

        public static byte[] ToBytes(this MmoCommand command)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                Serializer.Serialize(stream, command);
                return stream.ToArray();
            }
        }

        public static byte[] ToBytes(this IMmoCommand command)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                Serializer.Serialize(stream, command);
                return stream.ToArray();
            }
        }
        public static bool IsHandledBy<T>(this IMmoCommand cmd) where T : class
        {
            return typeof(IMmoCommand<T>).IsAssignableFrom(cmd.GetType());
        }
    }
}
