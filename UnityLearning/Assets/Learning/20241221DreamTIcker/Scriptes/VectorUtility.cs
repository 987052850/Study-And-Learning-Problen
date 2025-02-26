using UnityEngine;

namespace TEN.GLOBAL
{

    /// <summary>
    ///项目 : TEN
    ///日期：2024/12/22 12:10:55 
    ///创建者：Michael Corleone
    ///类用途：
    /// </summary>
    public static class VectorUtility
    {
        public static Vector3Int RoundToInt(this Vector3 v)
        {
            int x = Mathf.RoundToInt(v.x);
            int y = Mathf.RoundToInt(v.y);
            int z = Mathf.RoundToInt(v.z);
            return new Vector3Int(x, y, z);
        }

        public static Vector3Int CeilToInt(this Vector3 v)
        {
            int x = Mathf.CeilToInt(v.x);
            int y = Mathf.CeilToInt(v.y);
            int z = Mathf.CeilToInt(v.z);
            return new Vector3Int(x, y, z);
        }

        public static Vector3Int FloorToInt(this Vector3 v)
        {
            int x = Mathf.FloorToInt(v.x);
            int y = Mathf.FloorToInt(v.y);
            int z = Mathf.FloorToInt(v.z);
            return new Vector3Int(x, y, z);
        }
    }
}
