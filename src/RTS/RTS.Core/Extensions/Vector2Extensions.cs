using RTS.Core.Structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RTS.Core.Structs
{
    public static class Vector2Extensions
    {
        public static byte[] ToBytes(this Vector2 value)
        {
            var xBytes = BitConverter.GetBytes(value.x);
            var yBytes = BitConverter.GetBytes(value.y);
            byte[] bytes = new byte[xBytes.Length + yBytes.Length];
            Array.Copy(xBytes, bytes, xBytes.Length);
            Array.Copy(yBytes, 0, bytes, xBytes.Length, yBytes.Length);
            return bytes;
        }

        public static Vector2 FromBytes(this byte[] value)
        {
            Single x = BitConverter.ToSingle(value, 0);
            Single y = BitConverter.ToSingle(value, sizeof(Single));

            Vector2 vector2 = new Vector2(x, y);
            return vector2;
        }
    }
}
