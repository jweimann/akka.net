using Jitter.LinearMath;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RTS.Core.Structs
{
    [Serializable]
    public struct Vector3
    {
        public float x;
        public float y;
        public float z;

        public Vector3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
        public override string ToString()
        {
            return x + ", " + y + ", " + z;
        }
        public string ToRoundedString(int places = 2)
        {
            return Math.Round(x, places) + ", " + Math.Round(y, places) + ", " + Math.Round(z, places);
        }
        public static Vector3 operator +(Vector3 c1, Vector3 c2)
        {
            return new Vector3(c1.x + c2.x, c1.y + c2.y, c1.z + c2.z);
        }
        public static Vector3 operator -(Vector3 c1, Vector3 c2)
        {
            return new Vector3(c1.x - c2.x, c1.y - c2.y, c1.z - c2.z);
        }
        public static Vector3 operator /(Vector3 c1, int c2)
        {
            return new Vector3(c1.x / c2, c1.y / c2, c1.z / c2);
        }
        public static Vector3 operator *(Vector3 c1, int c2)
        {
            return new Vector3(c1.x * c2, c1.y * c2, c1.z * c2);
        }
        public static Vector3 operator *(Vector3 c1, float c2)
        {
            return new Vector3(c1.x * c2, c1.y * c2, c1.z * c2);
        }
        public static Vector3 operator *(Vector3 c1, double c2)
        {
            return c1 * (float)c2;
        }
        public static Vector3 operator *(double c2, Vector3 c1)
        {
            return c1 * (float)c2;
        }
        public static bool operator >(Vector3 c1, Vector3 c2)
        {
            return c1.x > c2.x && c1.y > c2.y && c1.z > c2.z;
        }
        public static bool operator <(Vector3 c1, Vector3 c2)
        {
            return c1.x < c2.x && c1.y < c2.y && c1.z < c2.z;
        }
        public static bool operator ==(Vector3 c1, Vector3 c2)
        {
            return c1.x == c2.x && c1.y == c2.y && c1.z == c2.z;
        }
        public static bool operator !=(Vector3 c1, Vector3 c2)
        {
            return c1.x != c2.x || c1.y != c2.y || c1.z != c2.z;
        }
        public static Vector3 zero
        {
            get
            {
                return new Vector3(0, 0, 0);
            }
        }
        public static int GetSize()
        {
            return sizeof(float) + sizeof(float) + sizeof(float);
        }

        public static float Distance(Vector3 a, Vector3 b)
        {
            Vector3 vector = new Vector3(a.x - b.x, a.y - b.y, a.z - b.z);
            return (float) Math.Sqrt(vector.x * vector.x + vector.y * vector.y + vector.z * vector.z);
            //return (float)Math.Sqrt(Math.Pow((v1.x - v2.x), 2) + Math.Pow((v1.y - v2.y), 2) + Math.Pow((v1.z - v2.z), 2));
        }

        public Vector3 Normalized()
        {
            float length = (float)Math.Sqrt( x*x + y*y + z*z);
            return new Vector3(x / length, y / length, z / length);
        }

        public static Vector3 LerpByDistance(Vector3 A, Vector3 B, float x)
        {
            Vector3 P = (double)x * (B - A).Normalized() + A;
            return P;
        }

        public Vector3 WithoutHeight()
        {
            return new Vector3(this.x, 0, this.z);
        }

        public bool IsNan()
        {
            return (float.IsNaN(x) || float.IsNaN(y) || float.IsNaN(z));
        }
    }
}
