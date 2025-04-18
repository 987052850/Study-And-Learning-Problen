using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TEN
{
    public interface IMapData
    {
        /// <summary>
        /// 根据颜色获取游戏对象
        /// </summary>
        /// <param name="meshColor">颜色</param>
        /// <param name="go">如果在表中找到了对应的颜色，则将根据该颜色创建的Gameobject赋值给go</param>
        /// <returns>true 存在这个颜色对应的对象 ， false 没有 </returns>
        bool GetMeshByColor(Color32 meshColor , out List<MeshRenderer> go);

        /// <summary>
        /// 根据传入的颜色获取两个mesh的交界的线
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="gos">1、交界为双向交界，即a与b有交界，那么b与a也有交界。但这里只会返回a与b的交界;2、俩mesh可能存在不唯一的交界</param>
        /// <returns>true 这俩颜色有交界 ， false 没有 </returns>
        bool GetJouction(Color32 a, Color32 b ,out List<GameObject> gos);
        /// <summary>
        /// a吞并b，人话：将meshb的颜色设置为mesha的颜色,并将边界线删除
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns>true 存在边界，合并成功 ， false 没有边界 </returns>
        bool Merge(Color32 a , Color32 b);
    }
    /// <summary>
    ///项目 : TEN
    ///日期：2025年4月9日11点37分
    ///创建者：Michael Corleone
    ///类用途：外部接口，用于外部调用
    /// </summary>
    public partial class PaseMaster : IMapData
    {
        public struct PointData
        {
            public Vector3 Position;
            public Color PointColor;

            public PointData(Vector3 pos, Color color)
            {
                Position = pos;
                PointColor = color;
            }
        }
        private static Int64 GenerateCriticalID(Color a , Color b)
        {
            // 将颜色 a 的各个通道转换到 0~255 范围
            byte ar = (byte)Mathf.RoundToInt(a.r * 255f);
            byte ag = (byte)Mathf.RoundToInt(a.g * 255f);
            byte ab = (byte)Mathf.RoundToInt(a.b * 255f);
            byte aa = (byte)Mathf.RoundToInt(a.a * 255f);

            // 将颜色 b 的各个通道转换到 0~255 范围
            byte br = (byte)Mathf.RoundToInt(b.r * 255f);
            byte bg = (byte)Mathf.RoundToInt(b.g * 255f);
            byte bb = (byte)Mathf.RoundToInt(b.b * 255f);
            byte ba = (byte)Mathf.RoundToInt(b.a * 255f);

            // 将颜色 a 拼接为 32 位整数（例如每个通道各 8 位）
            UInt32 idA = ((UInt32)ar << 24) | ((UInt32)ag << 16) | ((UInt32)ab << 8) | ((UInt32)aa);
            // 将颜色 b 拼接为 32 位整数
            UInt32 idB = ((UInt32)br << 24) | ((UInt32)bg << 16) | ((UInt32)bb << 8) | ((UInt32)ba);

            // 组合两个 32 位整数为一个 64 位整数
            Int64 combinedID = ((Int64)idA << 32) | idB;
            return combinedID;
        }

        private static Int64 GenerateCriticalID(Color32 a, Color32 b)
        {
            // 将颜色 a 的各个通道转换到 0~255 范围
            byte ar = (byte)a.r;
            byte ag = (byte)a.g;
            byte ab = (byte)a.b;
            byte aa = (byte)a.a;

            // 将颜色 b 的各个通道转换到 0~255 范围
            byte br = (byte)b.r;
            byte bg = (byte)b.g;
            byte bb = (byte)b.b;
            byte ba = (byte)b.a;

            // 将颜色 a 拼接为 32 位整数（例如每个通道各 8 位）
            UInt32 idA = ((UInt32)ar << 24) | ((UInt32)ag << 16) | ((UInt32)ab << 8) | ((UInt32)aa);
            // 将颜色 b 拼接为 32 位整数
            UInt32 idB = ((UInt32)br << 24) | ((UInt32)bg << 16) | ((UInt32)bb << 8) | ((UInt32)ba);

            // 组合两个 32 位整数为一个 64 位整数
            Int64 combinedID = ((Int64)idA << 32) | idB;
            return combinedID;
        }

        private static (Color , Color) ParseCriticalID(Int64 id)
        {
            // 提取颜色 a 的各个通道
            UInt32 idA = (UInt32)(id >> 32);
            byte ar = (byte)((idA >> 24) & 0xFF);
            byte ag = (byte)((idA >> 16) & 0xFF);
            byte ab = (byte)((idA >> 8) & 0xFF);
            byte aa = (byte)(idA & 0xFF);
            // 提取颜色 b 的各个通道
            UInt32 idB = (UInt32)(id & 0xFFFFFFFF);
            byte br = (byte)((idB >> 24) & 0xFF);
            byte bg = (byte)((idB >> 16) & 0xFF);
            byte bb = (byte)((idB >> 8) & 0xFF);
            byte ba = (byte)(idB & 0xFF);
            // 创建颜色 a 和 b
            Color colorA = new Color(ar / 255f, ag / 255f, ab / 255f, aa / 255f);
            Color colorB = new Color(br / 255f, bg / 255f, bb / 255f, ba / 255f);
            return (colorA, colorB);
        }
        private static (Color32, Color32) ParseCriticalID32(Int64 id)
        {
            // 提取颜色 a 的各个通道
            UInt32 idA = (UInt32)(id >> 32);
            byte ar = (byte)((idA >> 24) & 0xFF);
            byte ag = (byte)((idA >> 16) & 0xFF);
            byte ab = (byte)((idA >> 8) & 0xFF);
            byte aa = (byte)(idA & 0xFF);
            // 提取颜色 b 的各个通道
            UInt32 idB = (UInt32)(id & 0xFFFFFFFF);
            byte br = (byte)((idB >> 24) & 0xFF);
            byte bg = (byte)((idB >> 16) & 0xFF);
            byte bb = (byte)((idB >> 8) & 0xFF);
            byte ba = (byte)(idB & 0xFF);
            // 创建颜色 a 和 b
            Color32 colorA = new Color32(ar, ag , ab, aa );
            Color32 colorB = new Color32(br , bg , bb , ba );
            return (colorA, colorB);
        }

        private static Dictionary<Int64, List<Vector3>> _criticalPoints = new();
        private static Dictionary<Int64, List<GameObject>> _criticalGameobjects = new();
        private static Matrix4x4 _transMatr = Matrix4x4.identity;
        [Header("生成的mesh的高度")]
        public int MeshHeight = 0;
        [Header("生成的line的高度")]
        public int lineHeight = 0;
        private void GeneretorTransMatrix()
        {
            // 生成一个平移矩阵
            Vector3 offset = new Vector3(-1024, MeshHeight, -1024);
            _transMatr = Matrix4x4.TRS(offset, Quaternion.identity, Vector3.one / 2);

            Matrix4x4 coordinateTransMatr = Matrix4x4.identity;
            coordinateTransMatr.SetColumn(0, new Vector4(1, 0, 0, 0));
            coordinateTransMatr.SetColumn(1, new Vector4(0, 0, 1, 0));
            coordinateTransMatr.SetColumn(2, new Vector4(0, 1, 0, 0));
            coordinateTransMatr.SetColumn(3, new Vector4(0, 0, 0, 1));

            _transMatr = _transMatr * coordinateTransMatr;
        }

        public bool GetMeshByColor(Color32 meshColor, out List<MeshRenderer> lm)
        {
            //throw new NotImplementedException();
            if (!c2m.ContainsKey(meshColor))
            {
                lm = null;
                return false;
            }
            lm = new List<MeshRenderer>(c2m[meshColor]);
            return true;
        }

        public bool GetJouction(Color32 a, Color32 b, out List<GameObject> gos)
        {
            //throw new NotImplementedException();
            Int64 key = GenerateCriticalID(a, b);
            if(!_criticalGameobjects.ContainsKey(key))
            {
                gos = null;
                return false;
            }
            gos = new List<GameObject>(_criticalGameobjects[key]);
            return true;
        }

        public bool Merge(Color32 a, Color32 b)
        {
            if (PaseMaster.Instance.GetMeshByColor(b, out List<MeshRenderer> lm))
            {
                foreach (var item in lm)
                {
                    item.material.color = a;
                }
            }
            else
            {
                Debug.Log("2222222222");
            }

            #region 单向边界处理逻辑
            if (PaseMaster.Instance.GetJouction(a, b, out List<GameObject> lg))
            {
                foreach (var item in lg)
                {
                    Debug.Log($"{item.name}");
                    item.SetActive(false);
                }
            }
            else if (PaseMaster.Instance.GetJouction(b, a, out lg))
            {
                foreach (var item in lg)
                {
                    Debug.Log($"{item.name}");
                    item.SetActive(false);
                }
            }
            else
            {
                return false;
            }
            #endregion


            return true;
        }

        private void OnGUI()
        {
            GUI.skin.button.fontSize = 50;
            if (GUI.Button(new Rect(0, 0, 400, 100), "GetRandomMesh"))
            {
                int i = UnityEngine.Random.Range(1, colors.Count);
                i -= 1;
                if (PaseMaster.Instance.GetMeshByColor(colors_inverse[i], out List<MeshRenderer> lm))
                {
                    foreach (var item in lm)
                    {
                        Debug.Log($"{item.name}");
                        item.material.color = Color.white;
                    }
                }
                else
                {
                    Debug.Log("没有介个颜色");
                }

            }

            if (GUI.Button(new Rect(0, 100, 400, 100), "GetJouction"))
            {
                int i = UnityEngine.Random.Range(0, colors.Count);
                Color j = colors_neigbor[colors_inverse[i]][0];

                if (PaseMaster.Instance.GetJouction(colors_inverse[i] , j, out List<GameObject> lg))
                {
                    foreach (var item in lg)
                    {
                        Debug.Log($"{item.name}");
                        item.SetActive(false);
                    }
                }
                else
                {
                    Debug.Log("二者没有边界");
                }

            }

            if (GUI.Button(new Rect(0, 200, 400, 100), "Merge"))
            {
                int i = UnityEngine.Random.Range(0, colors.Count);
                Color j = colors_neigbor[colors_inverse[i]][0];

                if (PaseMaster.Instance.Merge(colors_inverse[i], j))
                {
                    Debug.Log($"{colors_inverse[i]} 已与 {j} 合体");
                }
                else
                {
                    Debug.Log("合体失败");
                }

            }
        }
    }
}
