using RTS.Core.Structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RTS.DataStructures
{
    public class MovementPath
    {
        private List<Vector3> _points;
        private int _currentPoint;
        public List<Vector3> Points { get { return _points; } }
        public bool IsValid { get { return _points != null && _currentPoint < _points.Count; } }
        public MovementPath(List<Vector3> points)
        {
            if (points == null)
            {
                throw new ArgumentOutOfRangeException("MovementPath points can not be null.");
            }
            _points = points;
        }

        public Vector3 NextPosition()
        {
            return _points[_currentPoint];
        }
        public void IncrementPoint()
        {
            _currentPoint++;
        }
    }
}
