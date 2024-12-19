using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TEN.UTILS
{
    /// <summary>
    ///项目 : TEN
    ///日期：2024/12/17 21:14:13 
    ///创建者：Michael Corleone
    ///类用途：
    /// </summary>
    public class Vector3EqualityComparer : IEqualityComparer<Vector3>
    {
        private readonly float _epsilon;
        public Vector3EqualityComparer(float vIn_Epsilon)
        {
            _epsilon = vIn_Epsilon;
        }
        public bool Equals(Vector3 x, Vector3 y)
        {
            return (Mathf.Abs(x.x - y.x) < _epsilon) &&
           (Mathf.Abs(x.y - y.y) < _epsilon) &&
           (Mathf.Abs(x.z - y.z) < _epsilon);
        }

        public int GetHashCode(Vector3 obj)
        {
            return (obj.x / _epsilon).GetHashCode() ^ (obj.y / _epsilon).GetHashCode() ^ (obj.z / _epsilon).GetHashCode();
        }
    }
}
