using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TEN.UTILS.MATRIX
{
    public static class MatrixUtils
    {
        public static Matrix4x4 Quaternion2Matrix(Quaternion q)
        {
            float x = q.x, y = q.y, z = q.z, w = q.w;

            float xx = x * x;
            float yy = y * y;
            float zz = z * z;
            float xy = x * y;
            float xz = x * z;
            float yz = y * z;
            float wx = w * x;
            float wy = w * y;
            float wz = w * z;

            Matrix4x4 m = new Matrix4x4();
            m.m00 = 1f - 2f * (yy + zz);
            m.m01 = 2f * (xy - wz);
            m.m02 = 2f * (xz + wy);
            m.m03 = 0f;

            m.m10 = 2f * (xy + wz);
            m.m11 = 1f - 2f * (xx + zz);
            m.m12 = 2f * (yz - wx);
            m.m13 = 0f;

            m.m20 = 2f * (xz - wy);
            m.m21 = 2f * (yz + wx);
            m.m22 = 1f - 2f * (xx + yy);
            m.m23 = 0f;

            m.m30 = 0f;
            m.m31 = 0f;
            m.m32 = 0f;
            m.m33 = 1f;

            return m;
        }
    }


    /// <summary>
    ///项目 : TEN
    ///日期：2025/4/17 23:51:08 
    ///创建者：Michael Corleone
    ///类用途：
    /// </summary>
    public static class Orthodonan
    {
        public static Matrix4x4 GetProjectionMatrix(Camera camera)
        {
            if (!camera.orthographic)
            {
                return Matrix4x4.identity;
            }
            Matrix4x4 mt = Matrix4x4.identity;
            mt.SetColumn(3, new Vector4(camera.transform.position.x, camera.transform.position.y, camera.transform.position.z, 1));
            Matrix4x4 mr = MatrixUtils.Quaternion2Matrix(camera.transform.rotation);
            Matrix4x4 ms = Matrix4x4.identity;
            ms.SetColumn(0, new Vector4(1 / (camera.aspect * camera.orthographicSize), 0, 0, 0));
            ms.SetColumn(1, new Vector4(0, 1 / (camera.orthographicSize), 0, 0));
            ms.SetColumn(2, new Vector4(0, 0, 2 / (camera.nearClipPlane - camera.farClipPlane), 0));
            ms.SetColumn(3, new Vector4(0, 0, -(camera.farClipPlane + camera.nearClipPlane) / (-camera.farClipPlane + camera.nearClipPlane), 1));
            return ms * mr * mt;
        }
        public static Matrix4x4 GetViewMatrix(Camera camera)
        {
            Matrix4x4 mt = Matrix4x4.identity;
            mt.SetColumn(3, new Vector4(camera.transform.position.x, camera.transform.position.y, camera.transform.position.z, 1));
            Matrix4x4 mr = MatrixUtils.Quaternion2Matrix(camera.transform.rotation);
            return (mr * mt).inverse;
        }
	}
}
