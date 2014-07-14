using System;
namespace RTS.Core.Structs
{
    [Serializable]
    public struct Vector2
    {
        public float x;
        public float y;

        public Vector2(float x, float y)
        {
            this.x = x;
            this.y = y;
        }
        public override string ToString()
        {
            return x + ", " + y;
        }
        public static Vector2 operator +(Vector2 c1, Vector2 c2)
        {
            return new Vector2(c1.x + c2.x, c1.y + c2.y);
        }
        public static Vector2 operator -(Vector2 c1, Vector2 c2)
        {
            return new Vector2(c1.x - c2.x, c1.y - c2.y);
        }
        public static Vector2 operator /(Vector2 c1, int c2)
        {
            return new Vector2(c1.x / c2, c1.y / c2);
        }
        public static Vector2 operator *(Vector2 c1, int c2)
        {
            return new Vector2(c1.x * c2, c1.y * c2);
        }
        public static bool operator >(Vector2 c1, Vector2 c2)
        {
            return c1.x > c2.x && c1.y > c2.y;
        }
        public static bool operator <(Vector2 c1, Vector2 c2)
        {
            return c1.x < c2.x && c1.y < c2.y;
        }
        public static bool operator ==(Vector2 c1, Vector2 c2)
        {
            return c1.x == c2.x && c1.y == c2.y;
        }
        public static bool operator !=(Vector2 c1, Vector2 c2)
        {
            return c1.x != c2.x || c1.y != c2.y;
        }
        public static Vector2 Zero
        {
            get
            {
                return new Vector2(0, 0);
            }
        }
        public static int GetSize()
        {
            return sizeof(float) + sizeof(float);
        }
    }
}
