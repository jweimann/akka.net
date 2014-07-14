using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RTS.Core.Structs
{
    [Serializable]
    public struct Bounds
    {
        private Vector3 _minimum;
        private Vector3 _maximum;
        private Vector3 _center;
        private Vector3 _extents;
        public Vector3 Center { get { return _center; } set { _center = value; } }
        public Vector3 Extents { get { return _extents; } set { _extents = value; } }
        
        public Bounds(Vector3 c, Vector3 e)
        {
            _center = c;
            _extents = e;

            _minimum = _center - _extents;
            _maximum = _center + _extents;
        }
        public Bounds(Vector3 c, float size)
        {
            _center = c;
            _extents = new Vector3(size, size, size);

            _minimum = _center - _extents;
            _maximum = _center + _extents;
        }

        public bool Contains(Vector3 _location)
        {
            if (_location > _minimum && 
                _location < _maximum)
            {
                return true;
            }

            return false;
        }

        public override string ToString()
        {
            return String.Format("Center: {0} Extents: {1}", Center.ToString(), Extents.ToString());
        }
    }

    public static class ExtensionMethods
    {
        public static int RoundOff(this int i, int nearest)
        {
            return ((int)Math.Round(i / (float)nearest)) * nearest;
        }
    }
}
