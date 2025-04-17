using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using KdTree;
using KdTree.Math;
using UnityEngine.Pool;


namespace TEN
{
    /// <summary>
    ///项目 : TEN
    ///日期：2025/4/5 14:27:01 
    ///创建者：Michael Corleone
    ///类用途：
    /// </summary>
    public partial class PaseMaster : MonoBehaviour
    {
        public RenderTexture RT;
        public Texture2D ASD;
        public LineRenderer LR;

        private static UnityEngine.Vector2Int _reslution;
        private static int _coefficient = 1000;

        public static int Reserve2Bit(float value)
        {
            return (int)(value * _coefficient);
        }
        public static UnityEngine.Vector3Int Reserve2Bit(Color value)
        {
            return new UnityEngine.Vector3Int((int)(value.r * _coefficient), (int)(value.g * _coefficient), (int)(value.b * _coefficient));
        }
        // 将 Color 转换为 Lab 空间的三个分量 (L, a, b)
        public static (double L, double a, double b) ColorToLab(Color color)
        {
            // 归一化 RGB 值到 [0,1]
            double r = color.r;
            double g = color.g;
            double b = color.b;

            // sRGB -> XYZ 转换（需要进行 Gamma 校正）
            r = PivotRgb(r);
            g = PivotRgb(g);
            b = PivotRgb(b);

            // sRGB 转 XYZ 的矩阵（D65 参考白点）
            double x = r * 0.4124564 + g * 0.3575761 + b * 0.1804375;
            double y = r * 0.2126729 + g * 0.7151522 + b * 0.0721750;
            double z = r * 0.0193339 + g * 0.1191920 + b * 0.9503041;

            // 参考白点 D65
            double refX = 0.95047;
            double refY = 1.00000;
            double refZ = 1.08883;

            x /= refX;
            y /= refY;
            z /= refZ;

            x = PivotXYZ(x);
            y = PivotXYZ(y);
            z = PivotXYZ(z);

            double L = 116 * y - 16;
            double a = 500 * (x - y);
            double bVal = 200 * (y - z);

            return (L, a, bVal);
        }

        // Gamma 校正
        private static double PivotRgb(double n)
        {
            return (n > 0.04045) ? System.Math.Pow((n + 0.055) / 1.055, 2.4) : n / 12.92;
        }

        // XYZ 转 Lab 的中间处理函数
        private static double PivotXYZ(double n)
        {
            return (n > 0.008856) ? System.Math.Pow(n, 1.0 / 3) : (7.787 * n) + (16.0 / 116);
        }

        public static double DeltaE((double L, double a, double b) lab1, (double L, double a, double b) lab2)
        {
            return System.Math.Sqrt(System.Math.Pow(lab1.L - lab2.L, 2) +
                             System.Math.Pow(lab1.a - lab2.a, 2) +
                             System.Math.Pow(lab1.b - lab2.b, 2));
        }

        // 获取指定颜色区域的边界坐标
        public static List<UnityEngine.Vector3Int> GetColorEdges(Texture2D texture, Color targetColor, float colorTolerance = 0.2f)
        {

            #region v2
            int width = texture.width;
            int height = texture.height;

            int count = 0;
            // 创建颜色掩码
            bool[,] colorMask = new bool[width, height];
            List<Vector3Int> edges = new List<Vector3Int>();

            //for (int x = 0; x < width; x++)
            //{
            //    for (int y = 0; y < height; y++)
            //    {
            //        Color pixelColor = texture.GetPixel(x, y);
            //        if (IsColorMatch(pixelColor, targetColor, colorTolerance))
            //        {
            //            colorMask[x, y] = true;
            //            count++;
            //            edges.Add(new UnityEngine.Vector3(x,0, y));
            //        }
            //    }
            //}
            //Debug.Log($"{count}");
            // 假设 targetColor 已经定义好
            var targetLab = ColorToLab(targetColor);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Color pixelColor = texture.GetPixel(x, y);
                    var pixelLab = ColorToLab(pixelColor);
                    if (pixelColor.Equals(targetColor))
                    //if (DeltaE(pixelLab, targetLab) <= colorTolerance)
                    {
                        colorMask[x, y] = true;
                        count++;
                        //edges.Add(new UnityEngine.Vector3(x, 0, y));
                    }
                }
            }
            Debug.Log($"{count}");
            // 使用Sobel算法检测边界
            for (int x = 1; x < width - 1; x++)
            {
                for (int y = 1; y < height - 1; y++)
                {
                    if (colorMask[x, y])
                    {
                        if (IsEdge(colorMask, x, y))
                            edges.Add(new Vector3Int(x, y));
                    }
                }
            }

            //UnityEngine.Vector3 center = UnityEngine.Vector3.zero;
            //foreach (var p in edges)
            //{
            //    center += p;
            //}
            //center /= edges.Count;
            ////center = Vector3.zero;
            ////int diss = 0;
            ////foreach (var item in collection)
            ////{
            ////    if (Vector3.Distance())
            ////    {

            ////    }
            ////}
            //edges = SortByPolarAngle(edges, center);
            return edges;
            #endregion
            #region v1
            //int width = texture.width;
            //int height = texture.height;


            //int count = 0;
            //// 创建颜色掩码
            ////bool[,] colorMask = new bool[width, height];
            //List<int> posx = new List<int>();
            //List<int> posy = new List<int>();
            //for (int x = 0; x < width; x++)
            //{
            //    for (int y = 0; y < height; y++)
            //    {
            //        Color pixelColor = texture.GetPixel(x, y);
            //        if (IsColorMatch(pixelColor, targetColor, colorTolerance))
            //        {
            //            //colorMask[x, y] = true;
            //            posx.Add(x);
            //            posy.Add(y);
            //            count++;
            //        }
            //    }
            //}


            ////Color[,] colorMask = new Color[width, height];

            ////for (int x = 0; x < width; x++)
            ////{
            ////    for (int y = 0; y < height; y++)
            ////    {
            ////        colorMask[x, y] = texture.GetPixel(x, y);
            ////    }
            ////}
            ////Debug.Log(count);
            //// 使用Sobel算法检测边界
            ////List<UnityEngine.Vector3> edges = new List<UnityEngine.Vector3>();
            ////for (int x = 1; x < width - 1; x++)
            ////{
            ////    for (int y = 1; y < height - 1; y++)
            ////    {
            ////        if (colorMask[x, y])
            ////        {
            ////            if (IsEdge(colorMask, x, y))
            ////                edges.Add(new UnityEngine.Vector2(x, y));
            ////        }
            ////    }
            ////}

            //List<UnityEngine.Vector3> edges = new List<UnityEngine.Vector3>();
            //for (int x = 0; x < posx.Count - 1; x++)
            //{
            //    if (IsEdge(texture, posx[x], posy[x]))
            //        edges.Add(new UnityEngine.Vector3(posx[x],0, posy[x]));
            //}
            //UnityEngine.Vector3 center = UnityEngine.Vector3.zero;
            //foreach (var p in edges)
            //{
            //    center += p;
            //}
            //center /= edges.Count;
            //edges = SortByPolarAngle(edges, center);
            //Debug.Log(edges.Count);
            //return edges;
            #endregion
        }
        public static BoundsMessageGroup GetBoundsLine()
        {
            //v4
            BoundsMessageGroup edges = new BoundsMessageGroup();
            int width = _reslution.x;
            int height = _reslution.x;
            // 使用Sobel算法检测边界
            for (int x = 1; x < width - 1; x++)
            {
                for (int y = 1; y < height - 1; y++)
                {
                    int color = _pixleMatrix[x, y];
                    if (color == -1)
                    {
                        continue;
                    }
                    if (color >= 0)
                    {
                        if (!colors_neigbor.ContainsKey(colors_inverse[color]))
                        {
                            //colors_neigbor[colors_inverse[iColor]] = new List<Color>();
                            colors_neigbor.Add(colors_inverse[color], new List<Color>());
                        }
                    }
                    //if (IsEdgeSimple(_pixleMatrix, x, y))
                    if (IsEdge(_pixleMatrix, x, y, color , out List<Color> neighbors))
                    {
                        foreach (var neighborColor in neighbors)
                        {

                            bool writed = false;
                            if (edges.BoundsMessage.ContainsKey(neighborColor))
                            {
                                BoundsNode temp = edges.BoundsMessage[neighborColor].Head;
                                do
                                {
                                    if (temp.NeighborColor == colors_inverse[color])
                                    {
                                        writed = true;
                                        break;
                                    }

                                    temp = temp.Next;
                                } while (temp != edges.BoundsMessage[neighborColor].Head);
                            }

                            if (!writed)
                                edges.AddPoint(colors_inverse[color], neighborColor, new Vector3Int(x, y , 0));
                        }
                        //edges[color].Add(new Vector3Int(x, y));
                    }
                }
            }
            return edges;

        }


        public static List<List<Vector3Int>> GetColorEdges()
        {
            //v3
            List<List<Vector3Int>> edges = Enumerable.Range(0, colors.Count)
                                      .Select(_ => new List<Vector3Int>())
                                      .ToList();
            int width = _reslution.x;
            int height = _reslution.x;
            // 使用Sobel算法检测边界
            for (int x = 1; x < width - 1; x++)
            {
                for (int y = 1; y < height - 1; y++)
                {
                    int color = _pixleMatrix[x, y];
                    if (color == -1)
                    {
                        continue;
                    }
                    int iColor = _pixleMatrix[x, y];
                    if (iColor >= 0)
                    {
                        if (!colors_neigbor.ContainsKey(colors_inverse[iColor]))
                        {
                            //colors_neigbor[colors_inverse[iColor]] = new List<Color>();
                            colors_neigbor.Add(colors_inverse[iColor], new List<Color>());
                        }
                    }
                    //if (IsEdgeSimple(_pixleMatrix, x, y))
                    if (IsEdge(_pixleMatrix, x, y, color))
                    {
                        edges[color].Add(new Vector3Int(x, y));
                    }
                }
            }


            //foreach (var item in colors_neigbor)
            //{
            //    List < Color > fTemp = item.Value;
            //    Debug.Log($"{colors[item.Key]} 有 {fTemp.Count}个邻居");
            //    foreach (var fitem in fTemp)
            //    {
            //        Debug.Log(colors[fitem]);
            //    }
            //}



            //for (int i = 0; i< colors.Count;i++)
            //{
            //    UnityEngine.Vector3 center = UnityEngine.Vector3.zero;
            //    List<Vector3> edge = edges[i];
            //    foreach (var p in edge)
            //    {
            //        center += p;
            //    }
            //    center /= edge.Count;
            //    edges[i] = SortByPolarAngle(edge, center);
            //}

            return edges;

        }
        // lt: returns 1 if a < b, otherwise 0.
        private static int lt(float a, float b)
        {
            return a < b ? 1 : 0;
        }

        // qua: computes the quadrant code for a point p.
        // This implementation follows: 
        // int qua(auto p) { return lt(p.y, 0) << 1 | lt(p.x, 0) ^ lt(p.y, 0); }
        private static int Qua(UnityEngine.Vector2 p)
        {
            // lt(p.Y, 0) << 1 computes a 2-bit value from the Y comparison,
            // then we OR with (lt(p.X, 0) XOR lt(p.Y, 0)).
            return (lt(p.y, 0) << 1) | (lt(p.x, 0) ^ lt(p.y, 0));
        }

        // Cross product for 2D UnityEngine.Vectors: a.X*b.Y - a.Y*b.X.
        private static float Cross(UnityEngine.Vector2 a, UnityEngine.Vector2 b)
        {
            return a.x * b.y - a.y * b.x;
        }
        private float ComputeSignedArea(List<Vector3> Points)
        {
            float area = 0;
            int count = Points.Count;
            for (int i = 0; i < count; i++)
            {
                //Vector2 current = Points[i];
                //Vector2 next = Points[(i + 1) % count];
                Vector2 current = new Vector2(Points[i].x, Points[i].z);
                Vector2 next = new Vector2(Points[(i + 1) % count].x, Points[(i + 1) % count].z);
                area += Cross(current, next);
            }
            return area * 0.5f;
        }

        /// <summary>
        /// 根据相对于 center 的极角，对 points 进行排序
        /// </summary>
        /// <param name="points">要排序的二维点列表</param>
        /// <param name="center">极角计算时的参考中心</param>
        /// <returns>排序后的二维点列表</returns>
        public static List<UnityEngine.Vector3> SortByPolarAngle(List<UnityEngine.Vector3> points, UnityEngine.Vector3 center)
        {
            points.Sort((v1, v2) =>
            {
                UnityEngine.Vector3 d1 = v1 - center;
                UnityEngine.Vector3 d2 = v2 - center;
                int q1 = Qua(d1);
                int q2 = Qua(d2);

                // If quadrant values differ, sort by quadrant.
                //if (q1 != q2)
                //    return q1.CompareTo(q2);

                // Otherwise, compare by cross product sign.
                // If the cross product is negative, then v1 comes before v2.
                float cp = Cross(d1, d2);
                if (cp < 0)
                    return -1;
                if (cp > 0)
                    return 1;

                // If they have the same angle, sort by distance (optional).
                return Vector2.Dot(d1, d1).CompareTo(Vector2.Dot(d2, d2));
            });

            //points.Sort((a, b) =>
            //{
            //    float angleA = Mathf.Atan2(a.y - center.y, a.x - center.x);
            //    float angleB = Mathf.Atan2(b.y - center.y, b.x - center.x);
            //    return angleA.CompareTo(angleB);
            //});
            return points;
        }

        // 判断是否为目标颜色
        static bool IsColorMatch(Color pixel, Color target, float tolerance)
        {
            return UnityEngine.Vector3.Distance(new UnityEngine.Vector3(pixel.r, pixel.g, pixel.b),
                                    new UnityEngine.Vector3(target.r, target.g, target.b)) <= tolerance;
        }
        static UnityEngine.Vector3 greenColor = new UnityEngine.Vector3(0.2125f, 0.7154f, 0.0721f);
        static float luminance(Color color)
        {
            return UnityEngine.Vector3.Dot(greenColor, new UnityEngine.Vector3(color.r, color.g, color.b));
        }
        static int ClampX(int value)
        {
            if (value < 0) return 0;
            if (value >= _reslution.x) return _reslution.x - 1;
            return value;
        }
        static int ClampY(int value)
        {
            if (value < 0) return 0;
            if (value >= _reslution.y) return _reslution.y - 1;
            return value;
        }

        static bool IsEdge(Texture2D mask, int x, int y)
        {
            float sum = 0;
            UnityEngine.Vector2Int pos = new UnityEngine.Vector2Int(x, y);
            UnityEngine.Vector2Int[] uvs = new UnityEngine.Vector2Int[9];
            uvs[0] = pos + new UnityEngine.Vector2Int(-1, -1);
            uvs[1] = pos + new UnityEngine.Vector2Int(0, -1);
            uvs[2] = pos + new UnityEngine.Vector2Int(1, -1);
            uvs[3] = pos + new UnityEngine.Vector2Int(-1, 0);
            uvs[4] = pos + new UnityEngine.Vector2Int(0, 0);
            uvs[5] = pos + new UnityEngine.Vector2Int(1, 0);
            uvs[6] = pos + new UnityEngine.Vector2Int(-1, 1);
            uvs[7] = pos + new UnityEngine.Vector2Int(0, 1);
            uvs[8] = pos + new UnityEngine.Vector2Int(1, 1);
            for (int i = 0; i < 9; i++)
            {
                //TODO,不确定x是否对应width
                uvs[i].x = Mathf.Clamp(uvs[i].x, 0, mask.width);
                uvs[i].y = Mathf.Clamp(uvs[i].y, 0, mask.height);
            }

            // Sobel算子
            int[] GGy = new int[9] { -1, 0, 1, -2, 0, 2, -1, 0, 1 };
            int[] GGx = new int[9] { -1, -2, -1, 0, 0, 0, 1, 2, 1 };
            float edgeX = 0;
            float edgeY = 0;
            float texColor = 0;
            for (int j = 0; j < 9; j++)
            {
                texColor = luminance(mask.GetPixel(uvs[j].x, uvs[j].y));
                edgeX += texColor * GGx[j];
                edgeY += texColor * GGy[j];
            }
            sum = 1 - Mathf.Abs(edgeX) - Mathf.Abs(edgeY);
            return sum < 0.001f;
        }

        static bool IsEdgeSimple(int[,] mask, int x, int y)
        {
            int c = mask[x, y];
            // 8邻域偏移（可根据需求调整成4邻域）
            Vector2Int[] neighbors = new Vector2Int[]
            {
        new Vector2Int(0,1), new Vector2Int(1,1), new Vector2Int(1,0), new Vector2Int(1,-1),
        new Vector2Int(0,-1), new Vector2Int(-1,-1), new Vector2Int(-1,0), new Vector2Int(-1,1)
            };

            int width = mask.GetLength(0);
            int height = mask.GetLength(1);

            foreach (var off in neighbors)
            {
                int nx = x + off.x;
                int ny = y + off.y;
                // 边界检查
                if (nx < 0 || nx >= width || ny < 0 || ny >= height)
                    continue;
                if (mask[nx, ny] != c)
                {
                    // 邻居颜色与自己不同 => 这是边界
                    return true;
                }
            }
            return false;
        }
        public static Color BadColor = new Color(0, 0, 0, 0);
        static bool IsEdge(int[,] mask, int x, int y, int color , out List<Color> neighbor)
        {
            int sum = 0;
            neighbor = new List<Color>();
            // Sobel算子
            int[,] gx = new int[3, 3] { { -1, 0, 1 }, { -2, 0, 2 }, { -1, 0, 1 } };
            int[,] gy = new int[3, 3] { { -1, -2, -1 }, { 0, 0, 0 }, { 1, 2, 1 } };

            int width = mask.GetLength(0);
            int height = mask.GetLength(1);
            int mColor_source = mask[x, y];

            int gradX = 0, gradY = 0;
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    int nx = Mathf.Clamp(x + i, 0, width - 1);
                    int ny = Mathf.Clamp(y + j, 0, height - 1);
                    int neighborColor = mask[nx, ny];
                    int val = neighborColor == color ? 1 : 0;
                    if (neighborColor >= 0 && val == 0)
                    {
                        if (!colors_neigbor[colors_inverse[mColor_source]].Contains(colors_inverse[neighborColor]))
                        {
                            colors_neigbor[colors_inverse[mColor_source]].Add(colors_inverse[neighborColor]);
                        }
                    }
                    if (val == 0)
                    {
                            if (!neighbor.Contains(colors_inverse[neighborColor]))
                            {
                                neighbor.Add(colors_inverse[neighborColor]);
                            }
                        //else
                        //{
                        //    if (!neighbor.Contains(BadColor))
                        //    {
                        //        neighbor.Add(BadColor);
                        //    }
                        //}
                    }
                    //else
                    //{
                    //    if (!neighbor.Contains(BadColor))
                    //    {
                    //        neighbor.Add(BadColor);
                    //    }
                    //}

                    gradX += gx[i + 1, j + 1] * val;
                    gradY += gy[i + 1, j + 1] * val;
                }
            }

            sum = Mathf.Abs(gradX) + Mathf.Abs(gradY);
            return sum > 0;
        }

        static bool IsEdge(int[,] mask, int x, int y, int color)
        {
            int sum = 0;

            // Sobel算子
            int[,] gx = new int[3, 3] { { -1, 0, 1 }, { -2, 0, 2 }, { -1, 0, 1 } };
            int[,] gy = new int[3, 3] { { -1, -2, -1 }, { 0, 0, 0 }, { 1, 2, 1 } };

            int width = mask.GetLength(0);
            int height = mask.GetLength(1);
            int mColor_source = mask[x, y];

            int gradX = 0, gradY = 0;
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    int nx = Mathf.Clamp(x + i, 0, width - 1);
                    int ny = Mathf.Clamp(y + j, 0, height - 1);
                    int iColor = mask[nx, ny];

                    int val = iColor == color ? 1 : 0;
                    if (iColor >= 0 && val == 0)
                    {
                        if (!colors_neigbor[colors_inverse[mColor_source]].Contains(colors_inverse[iColor]))
                        {
                            colors_neigbor[colors_inverse[mColor_source]].Add(colors_inverse[iColor]);
                        }
                    }
                    gradX += gx[i + 1, j + 1] * val;
                    gradY += gy[i + 1, j + 1] * val;
                }
            }

            sum = Mathf.Abs(gradX) + Mathf.Abs(gradY);
            return sum > 0;
        }
        // Sobel边界检测
        static bool IsEdge(bool[,] mask, int x, int y)
        {

            /*
            half Sobel(v2f i)
            {
                const half Gx[9] = {-1,-2,-1,0,0,0,1,2,1};
                const half Gy[9] = {-1,0,1,-2,0,2,-1,0,1};
                half edgeX = 0;
                half edgeY = 0;
                half texColor = 0;
                for(int j = 0 ; j < 9 ; j++)
                {
                    texColor = luminance(tex2D(_MainTex, i.uv[j]));
                    edgeX += texColor * Gx[j];
                    edgeY += texColor * Gy[j];
                }
 
                return 1 - abs(edgeX) - abs(edgeY);
            }
             
             */




            int sum = 0;

            // Sobel算子
            int[,] gx = new int[3, 3] { { -1, 0, 1 }, { -2, 0, 2 }, { -1, 0, 1 } };
            int[,] gy = new int[3, 3] { { -1, -2, -1 }, { 0, 0, 0 }, { 1, 2, 1 } };

            int width = mask.GetLength(0);
            int height = mask.GetLength(1);

            int gradX = 0, gradY = 0;
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    int nx = Mathf.Clamp(x + i, 0, width - 1);
                    int ny = Mathf.Clamp(y + j, 0, height - 1);
                    int val = mask[nx, ny] ? 1 : 0;
                    gradX += gx[i + 1, j + 1] * val;
                    gradY += gy[i + 1, j + 1] * val;
                }
            }

            sum = Mathf.Abs(gradX) + Mathf.Abs(gradY);
            return sum > 0;
        }

        private Texture2D DuplicateTexture(Texture2D source)
        {
            RenderTexture renderTex = RenderTexture.GetTemporary(
                        source.width,
                        source.height,
                        0,
                        RenderTextureFormat.ARGB32,
                        RenderTextureReadWrite.Linear);

            Graphics.Blit(source, renderTex);
            RenderTexture previous = RenderTexture.active;
            RenderTexture.active = renderTex;
            Texture2D readableText = new Texture2D(source.width, source.height);
            readableText.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
            readableText.Apply();
            RenderTexture.active = previous;
            RenderTexture.ReleaseTemporary(renderTex);
            return readableText;
        }


        public static List<Vector3> GetCatmullRomSplineOpen(List<Vector3> points, int subdivisions)
        {
            if (points == null || points.Count < 2)
            {
                // 至少需要两个点来构成一条线段
                return null;
            }

            // 扩展原始点列表：在开头和结尾各复制一个端点
            List<Vector3> extendedPoints = new List<Vector3>();
            extendedPoints.Add(points[0]);           // 在开头复制第一个点
            extendedPoints.AddRange(points);
            extendedPoints.Add(points[points.Count - 1]); // 在结尾复制最后一个点

            List<Vector3> splinePoints = new List<Vector3>();

            // 对扩展列表中每连续4个点，计算一个曲线段
            // 这里 i 从 0 到 extendedPoints.Count - 4，保证有 p0, p1, p2, p3 四个点
            for (int i = 0; i < extendedPoints.Count - 3; i++)
            {
                Vector3 p0 = extendedPoints[i];
                Vector3 p1 = extendedPoints[i + 1];
                Vector3 p2 = extendedPoints[i + 2];
                Vector3 p3 = extendedPoints[i + 3];

                // 对当前段细分 subdivisions 次
                for (int j = 0; j < subdivisions; j++)
                {
                    float t = j / (float)subdivisions;
                    float t2 = t * t;
                    float t3 = t2 * t;

                    // Catmull-Rom 插值公式：
                    // P(t) = 0.5 * [2P1 + (P2 - P0)t + (2P0 - 5P1 + 4P2 - P3)t² + (-P0 + 3P1 - 3P2 + P3)t³]
                    float x = 0.5f * (2 * p1.x +
                                      (-p0.x + p2.x) * t +
                                      (2 * p0.x - 5 * p1.x + 4 * p2.x - p3.x) * t2 +
                                      (-p0.x + 3 * p1.x - 3 * p2.x + p3.x) * t3);
                    float y = 0.5f * (2 * p1.y +
                                      (-p0.y + p2.y) * t +
                                      (2 * p0.y - 5 * p1.y + 4 * p2.y - p3.y) * t2 +
                                      (-p0.y + 3 * p1.y - 3 * p2.y + p3.y) * t3);
                    float z = 0.5f * (2 * p1.z +
                                      (-p0.z + p2.z) * t +
                                      (2 * p0.z - 5 * p1.z + 4 * p2.z - p3.z) * t2 +
                                      (-p0.z + 3 * p1.z - 3 * p2.z + p3.z) * t3);

                    splinePoints.Add(new Vector3(x, y, z));
                }
            }
            // 最后把最后一个点加入（确保曲线终点完整）
            splinePoints.Add(points[points.Count - 1]);

            return splinePoints;
        }

        /// <summary>
        /// 样条曲线插值算法
        /// </summary>
        /// <param name="points"></param>
        /// <param name="subdivisions"></param>
        /// <returns></returns>
        public static List<Vector3> GetCatmullRomSpline(List<Vector3> points, int subdivisions)
        {
            if (points == null || points.Count < 4)
                //throw new ArgumentException("控制点至少需要4个。");
                return null;

            List<Vector3> splinePoints = new List<Vector3>();

            // 对每一段（由四个控制点定义）进行插值计算
            for (int i = 0; i < points.Count; i++)
            {
                int iindex = i;
                Vector3 p0 = points[iindex > (points.Count - 1) ? (iindex - points.Count + 1) : (iindex)];
                iindex++;
                Vector3 p1 = points[iindex > (points.Count - 1) ? (iindex - points.Count + 1) : (iindex)];
                iindex++;
                Vector3 p2 = points[iindex > (points.Count - 1) ? (iindex - points.Count + 1) : (iindex)];
                iindex++;
                Vector3 p3 = points[iindex > (points.Count - 1) ? (iindex - points.Count + 1) : (iindex)];

                // 对当前段进行细分
                for (int j = 0; j < subdivisions; j++)
                {
                    float t = j / (float)subdivisions;
                    float t2 = t * t;
                    float t3 = t2 * t;

                    // 使用 Catmull-Rom 样条公式计算插值点：
                    // P(t) = 0.5 * [2P1 + (P2 - P0)t + (2P0 - 5P1 + 4P2 - P3)t² + (-P0 + 3P1 - 3P2 + P3)t³]
                    float x = 0.5f * ((2 * p1.x) +
                                      (-p0.x + p2.x) * t +
                                      (2 * p0.x - 5 * p1.x + 4 * p2.x - p3.x) * t2 +
                                      (-p0.x + 3 * p1.x - 3 * p2.x + p3.x) * t3);
                    float y = 0.5f * ((2 * p1.y) +
                                      (-p0.y + p2.y) * t +
                                      (2 * p0.y - 5 * p1.y + 4 * p2.y - p3.y) * t2 +
                                      (-p0.y + 3 * p1.y - 3 * p2.y + p3.y) * t3);

                    splinePoints.Add(new Vector3(x, y));
                }
            }
            // 为了保证曲线的完整性，添加最后一个点
            //splinePoints.Add(points[points.Count - 2]);

            return splinePoints;
        }

        public static List<Vector3> GetClosedCatmullRomSpline(List<Vector3> controlPoints, int subdivisions)
        {
            if (controlPoints == null || controlPoints.Count < 3)
                //throw new ArgumentException("控制点至少需要4个。");
                return null;

            // 复制数据，防止修改原始集合
            List<Vector3> points = new List<Vector3>(controlPoints);

            // 如果首尾重复，则移除最后一个重复的点
            if (points[0] == points[points.Count - 1])
                points.RemoveAt(points.Count - 1);

            int numPoints = points.Count;
            List<Vector3> splinePoints = new List<Vector3>();

            // 对于封闭曲线，每个点都是一段曲线的起点，总共处理 numPoints 段
            for (int i = 0; i < numPoints; i++)
            {
                // 使用模运算处理索引，实现首尾相连的效果
                Vector3 p0 = points[(i - 1 + numPoints) % numPoints];
                Vector3 p1 = points[i];
                Vector3 p2 = points[(i + 1) % numPoints];
                Vector3 p3 = points[(i + 2) % numPoints];

                // 对当前段进行细分插值
                for (int j = 0; j < subdivisions; j++)
                {
                    float t = j / (float)subdivisions;
                    Vector3 interpolatedPoint = CalculateCatmullRomPosition(t, p0, p1, p2, p3);
                    splinePoints.Add(interpolatedPoint);
                }
            }
            // 为确保曲线闭合，可以添加第一个控制点作为最后一个点
            splinePoints.Add(points[0]);

            return splinePoints;
        }

        /// <summary>
        /// 根据 Catmull-Rom 样条公式计算插值点
        /// </summary>
        /// <param name="t">参数 t, 范围 [0,1]</param>
        /// <param name="p0">前一个控制点</param>
        /// <param name="p1">当前控制点</param>
        /// <param name="p2">下一个控制点</param>
        /// <param name="p3">下下个控制点</param>
        /// <returns>插值计算得到的点</returns>
        private static Vector3 CalculateCatmullRomPosition(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
        {
            float t2 = t * t;
            float t3 = t2 * t;

            // Catmull-Rom 样条公式
            return 0.5f * ((2f * p1) +
                           (-p0 + p2) * t +
                           (2f * p0 - 5f * p1 + 4f * p2 - p3) * t2 +
                           (-p0 + 3f * p1 - 3f * p2 + p3) * t3);
        }

        #region Visvalingam-Whyatt
        /// <summary>
        /// 对给定的点集执行 Visvalingam-Whyatt 简化。
        /// </summary>
        /// <param name="points">构成折线或多边形的点集合，顺序排列。</param>
        /// <param name="areaThreshold">有效面积阈值，面积小于该值的点将被移除。</param>
        /// <returns>简化后的点集合</returns>
        public static List<Vector3> Simplify(List<Vector3> points, float areaThreshold)
        {
            // 点数不足三个直接返回
            if (points == null)
            {
                return null;
            }
            if (points.Count < 3)
                return new List<Vector3>(points);

            // 构建双向链表的节点，每个节点保存一个点以及计算的有效面积
            List<Node> nodes = new List<Node>();
            for (int i = 0; i < points.Count; i++)
            {
                Node node = new Node
                {
                    Point = points[i]
                };
                nodes.Add(node);
            }

            // 建立前后节点的关联关系
            for (int i = 0; i < nodes.Count; i++)
            {
                if (i > 0)
                    nodes[i].Prev = nodes[i - 1];
                if (i < nodes.Count - 1)
                    nodes[i].Next = nodes[i + 1];
            }

            // 对每个非端点节点计算有效面积
            for (int i = 1; i < nodes.Count - 1; i++)
            {
                nodes[i].Area = CalculateTriangleArea(nodes[i].Prev.Point, nodes[i].Point, nodes[i].Next.Point);
            }
            // 将首尾点的面积设为无穷大，保证它们不会被移除
            nodes[0].Area = float.MaxValue;
            nodes[nodes.Count - 1].Area = float.MaxValue;

            // 迭代移除有效面积小于阈值的节点
            bool removed = true;
            while (removed)
            {
                removed = false;
                Node minNode = null;
                // 找出当前所有节点中有效面积最小且小于阈值的节点（注意端点面积为无穷大，不会被选中）
                foreach (var node in nodes)
                {
                    if (node.Area < areaThreshold)
                    {
                        if (minNode == null || node.Area < minNode.Area)
                            minNode = node;
                    }
                }

                // 若存在需要移除的节点，则进行移除并更新相邻节点的有效面积
                if (minNode != null)
                {
                    removed = true;

                    // 更新双向链表，移除 minNode
                    if (minNode.Prev != null)
                        minNode.Prev.Next = minNode.Next;
                    if (minNode.Next != null)
                        minNode.Next.Prev = minNode.Prev;

                    // 更新相邻节点的有效面积
                    if (minNode.Prev != null && minNode.Prev.Prev != null)
                    {
                        minNode.Prev.Area = CalculateTriangleArea(minNode.Prev.Prev.Point, minNode.Prev.Point, minNode.Next.Point);
                    }
                    if (minNode.Next != null && minNode.Next.Next != null)
                    {
                        minNode.Next.Area = CalculateTriangleArea(minNode.Prev.Point, minNode.Next.Point, minNode.Next.Next.Point);
                    }

                    // 从节点集合中移除该节点
                    nodes.Remove(minNode);
                }
            }

            // 将剩余节点按顺序组合成最终结果
            List<Vector3> result = new List<Vector3>();
            Node current = nodes[0];
            while (current != null)
            {
                result.Add(current.Point);
                current = current.Next;
            }
            result.RemoveAt(result.Count - 1); // 移除最后一个点，避免重复
            return result;
        }
        public static List<Vector3> SimplifyLine(List<Vector3> points, float areaThreshold)
        {
            if (points == null)
                return null;

            if (points.Count < 3)
                return new List<Vector3>(points);

            List<Node> nodes = new List<Node>();
            for (int i = 0; i < points.Count; i++)
            {
                Node node = new Node { Point = points[i] };
                nodes.Add(node);
            }

            for (int i = 0; i < nodes.Count; i++)
            {
                if (i > 0)
                    nodes[i].Prev = nodes[i - 1];
                if (i < nodes.Count - 1)
                    nodes[i].Next = nodes[i + 1];
            }

            // 首尾点有效面积设置为无穷大，确保它们不会被移除
            nodes[0].Area = float.MaxValue;
            nodes[nodes.Count - 1].Area = float.MaxValue;

            for (int i = 1; i < nodes.Count - 1; i++)
            {
                nodes[i].Area = CalculateTriangleArea(nodes[i].Prev.Point, nodes[i].Point, nodes[i].Next.Point);
            }

            bool removed = true;
            while (removed)
            {
                removed = false;
                Node minNode = null;

                foreach (var node in nodes)
                {
                    if (node.Area < areaThreshold)
                    {
                        if (minNode == null || node.Area < minNode.Area)
                            minNode = node;
                    }
                }

                if (minNode != null)
                {
                    removed = true;

                    if (minNode.Prev != null)
                        minNode.Prev.Next = minNode.Next;
                    if (minNode.Next != null)
                        minNode.Next.Prev = minNode.Prev;

                    if (minNode.Prev != null && minNode.Prev.Prev != null && minNode.Next != null)
                        minNode.Prev.Area = CalculateTriangleArea(minNode.Prev.Prev.Point, minNode.Prev.Point, minNode.Next.Point);

                    if (minNode.Next != null && minNode.Next.Next != null && minNode.Prev != null)
                        minNode.Next.Area = CalculateTriangleArea(minNode.Prev.Point, minNode.Next.Point, minNode.Next.Next.Point);

                    nodes.Remove(minNode);
                }
            }

            List<Vector3> result = new List<Vector3>();
            Node current = nodes[0];
            while (current != null)
            {
                result.Add(current.Point);
                current = current.Next;
            }

            // 对于线段概化，不再删除最后一个点
            return result;
        }


        /// <summary>
        /// 根据三个点计算三角形面积（使用向量叉乘）。
        /// </summary>
        private static float CalculateTriangleArea(Vector3 a, Vector3 b, Vector3 c)
        {
            Vector3 ab = b - a;
            Vector3 ac = c - a;
            return 0.5f * Vector3.Cross(ab, ac).magnitude;
        }

        /// <summary>
        /// 内部节点类，构成双向链表，保存点、有效面积以及前后节点引用。
        /// </summary>
        private class Node
        {
            public Vector3 Point;
            public float Area;
            public Node Prev;
            public Node Next;
        }
        #endregion

        public void ResetMapColor()
        {
            colors.Clear();
            int iValue = 0;
            _pixleMatrix = new int[ASD.width, ASD.height];
            for (int i = 0; i < ASD.width; i++)
            {
                for (int j = 0; j < ASD.height; j++)
                {
                    _pixleMatrix[i, j] = -1;
                }
            }
            colors.Add(BadColor, -1);
            colors_inverse.Add(-1, BadColor);

            for (int i = 0; i < ASD.width; i++)
            {
                for (int j = 0; j < ASD.height; j++)
                {
                    Color tempColor = ASD.GetPixel(i, j);
                    if (tempColor.a < (1 - 1e-5f))
                    {
                        continue;
                    }
                    if (!colors.ContainsKey(tempColor))
                    {
                        colors.Add(tempColor, iValue);
                        colors_inverse.Add(iValue, tempColor);
                        iValue++;
                    }
                    _pixleMatrix[i, j] = colors[tempColor];
                }
            }
            Debug.Log($"colors.cont{colors.Count}");
        }

        //Area of the excluded triangle
        [Header("概化算法使用，当三角形的面积小于这个值，则舍弃该点")]
        public float Area = 10f;
        [Header("阙值，自定义的边缘检测算法中使用")]
        public float Thresold = 0.1f;
        [Header("绘制线的宽度")]
        public float LineWidth = 1;
        [Header("当两点之间的距离大于这个值，则判定存在子区域")]
        public float AreaMinDis = 5;
        [Header("一个完整区域包含的最大封闭区域的数量")]
        public float Max = 10;
        [Header("在两点之间插入的点的数量")]
        public int InsertPointCount = 3;
        List<Vector3Int> Vector2Ints = new List<UnityEngine.Vector3Int>();
        List<List<Vector3Int>> allAreas;
        private static Dictionary<Color, int> colors = new Dictionary<Color, int>();
        private static Dictionary<int, Color> colors_inverse = new Dictionary<int, Color>();
        private static Dictionary<Color, List<Color>> colors_neigbor = new Dictionary<Color, List<Color>>();
        private static int[,] _pixleMatrix;
        [Header("概化")]
        public bool VisvalingamWhyatt = false;
        [Header("细分")]
        public bool Catmull = false;
        public static PaseMaster Instance = null;
        private Transform meshDad;
        private Transform lineDad;
        private Dictionary<Color, List<MeshRenderer>> c2m = new Dictionary<Color, List<MeshRenderer>>();

        private void Awake()
        {
            if (Instance)
            {
                Destroy(this);
                return;
            }
            Instance = this;
            GeneretorTransMatrix();
            //ASD = DuplicateTexture(ASD);
            _reslution = new UnityEngine.Vector2Int(ASD.width, ASD.height);
            //GenertorBidirectionalGraph(ASD.width, ASD.height);
            //StartCoroutine(GenertorBidirectionalGraph(ASD.width, ASD.height));
            //Parse(new Color(148f / 255f, 255f / 255f, 111f / 255f, 1) , LR);

            //int i = 0;
            //foreach (var item in Vector2Ints)
            //{
            //    GameObject temp = new GameObject($"{i++}");
            //    temp.transform.position = item;
            //}
        }
        private void Start()
        {
            StartCoroutine(GeneretorLines_v2());
        }

        public class BoundsNode
        {
            public Color SelfColor;
            public Color NeighborColor;
            public List<Vector3Int> Boundary;
            public List<List<Vector3>> Filtered;
            public BoundsNode Next;
            public BoundsNode(Color selfColor, Color neighborColor)
            {
                SelfColor = selfColor;
                NeighborColor = neighborColor;
                Boundary = new List<Vector3Int>();
                Filtered = new ();
            }
            public BoundsNode(BoundsNode data)
            {
                SelfColor = data.SelfColor;
                NeighborColor = data.NeighborColor;
                Boundary = new List<Vector3Int>(data.Boundary);
            }

            // 输入：List<Vector3Int> boundaryPoints，所有边界像素点
            // 输出：List<List<Vector3Int>> sortedSegments，按顺序排列的独立线段

            public List<List<Vector3Int>> SplitAndSortBoundary(List<Vector3Int> boundaryPoints)
            {
                List<List<Vector3Int>> segments = new List<List<Vector3Int>>();
                if (boundaryPoints == null || boundaryPoints.Count == 0)
                    return segments;

                // 构建邻接字典：每个点 -> 与之相邻的所有点（选用 8 邻域作为示例）
                Dictionary<Vector3Int, List<Vector3Int>> neighborsDict = new Dictionary<Vector3Int, List<Vector3Int>>();
                HashSet<Vector3Int> pointSet = new HashSet<Vector3Int>(boundaryPoints);
                foreach (var p in boundaryPoints)
                {
                    List<Vector3Int> nbrs = new List<Vector3Int>();
                    foreach (var n in GetNeighbors(p)) // GetNeighbors 返回 8 个可能的邻域点
                    {
                        if (pointSet.Contains(n))
                            nbrs.Add(n);
                    }
                    neighborsDict[p] = nbrs;
                }

                // 用于标记已经处理过的点
                HashSet<Vector3Int> visited = new HashSet<Vector3Int>();

                // 遍历所有点，提取连通组件
                foreach (var p in boundaryPoints)
                {
                    if (visited.Contains(p))
                        continue;

                    // 使用 BFS 或 DFS 获取一个连通组件（无序集）
                    List<Vector3Int> component = new List<Vector3Int>();
                    Queue<Vector3Int> queue = new Queue<Vector3Int>();
                    queue.Enqueue(p);
                    visited.Add(p);
                    while (queue.Count > 0)
                    {
                        var current = queue.Dequeue();
                        component.Add(current);
                        foreach (var nbr in neighborsDict[current])
                        {
                            if (!visited.Contains(nbr))
                            {
                                visited.Add(nbr);
                                queue.Enqueue(nbr);
                            }
                        }
                    }

                    // 对当前连通组件尝试排序：
                    List<List<Vector3Int>> componentSegments = SortComponent(component, neighborsDict);
                    segments.AddRange(componentSegments);
                    //segments.AddRange(new List<List<Vector3Int>>() { component });
                }

                return segments;
            }

            /// <summary>
            /// 获取 8 邻域中所有候选点
            /// </summary>
            private List<Vector3Int> GetNeighbors(Vector3Int p)
            {
                List<Vector3Int> nbrs = new List<Vector3Int>();
                //for (int dx = -1; dx <= 1; dx++)
                //{
                //    for (int dy = -1; dy <= 1; dy++)
                //    {
                //        if (dx == 0 && dy == 0)
                //            continue;
                //        nbrs.Add(new Vector3Int(p.x + dx, p.y + dy, p.z)); // 假设 z 不变
                //    }
                //}

                nbrs.Add(p + new Vector3Int(0, 1, 0));
                //nbrs.Add(p + new Vector3Int(1, 1, 0));
                nbrs.Add(p + new Vector3Int(1, 0, 0));
                //nbrs.Add(p + new Vector3Int(1, -1, 0));
                nbrs.Add(p + new Vector3Int(0, -1, 0));
                //nbrs.Add(p + new Vector3Int(-1, -1, 0));
                nbrs.Add(p + new Vector3Int(-1, 0, 0));
                //nbrs.Add(p + new Vector3Int(-1, 1, 0));

                return nbrs;
            }

            /// <summary>
            /// 对连通组件中的点进行排序，分解为一个或多个线段
            /// </summary>
            private List<List<Vector3Int>> SortComponent(List<Vector3Int> component, Dictionary<Vector3Int, List<Vector3Int>> neighborsDict)
            {
                List<List<Vector3Int>> segments = new List<List<Vector3Int>>();
                HashSet<Vector3Int> used = new HashSet<Vector3Int>();

                // 找出所有端点（邻居数==1）
                List<Vector3Int> endpoints = component.FindAll(p => neighborsDict[p].Count == 1);

                // 用于跟踪尚未处理的节点
                Queue<Vector3Int> pointsToProcess = new Queue<Vector3Int>(endpoints.Count > 0 ? endpoints : component);

                while (pointsToProcess.Count > 0)
                {
                    Vector3Int start = pointsToProcess.Dequeue();

                    // 跳过已访问节点
                    if (used.Contains(start))
                        continue;

                    List<Vector3Int> segment = new List<Vector3Int>();
                    Vector3Int current = start;
                    Vector3Int prev = default(Vector3Int);

                    while (true)
                    {
                        segment.Add(current);
                        used.Add(current);

                        // 找未访问的邻居，不包括前一个节点
                        var nextCandidates = neighborsDict[current].FindAll(n => !n.Equals(prev) && !used.Contains(n));

                        if (nextCandidates.Count == 0)
                            break;

                        // 选择下一个邻居（可以根据需求优化）
                        Vector3Int next = nextCandidates[0];

                        prev = current;
                        current = next;
                    }

                    segments.Add(segment);
                }

                return segments;
            }

        }
        public class BoundsCircularLink
        {
            public BoundsNode Head;
            public BoundsNode Tail;

            public BoundsCircularLink(BoundsNode boundsNode)
            {
                Head = boundsNode;
                Tail = boundsNode;

                Head.Next = Tail;
                Tail.Next = Head;
            }

            public void Insert(BoundsNode data)
            {
                Tail.Next = data;
                data.Next = Head;
                Tail = data;
            }

            public List<List<Vector3>> GetAllJunctionLine()
            {
                List<List<Vector3>> outValue = new List<List<Vector3>>();
                BoundsNode temp = Head;
                do
                {
                    outValue.AddRange(temp.Filtered);
                    temp = temp.Next;
                } while (temp != Head);
                return outValue;
            }

            public HashSet<Color> GetNeighborsColor()
            {
                HashSet<Color> outValue = new ();
                BoundsNode temp = Head;
                do
                {
                    outValue.Add(temp.NeighborColor);
                    temp = temp.Next;
                } while (temp != Head);
                return outValue;
            }

            public bool ConditionsColor(Color color)
            {
                BoundsNode temp = Head;
                do
                {
                    if (temp.NeighborColor == color)
                    {
                        return true;
                    }
                    temp = temp.Next;
                } while (temp != Head);
                return false;
            }

            public BoundsNode GetNerighbor(Color color)
            {
                BoundsNode temp = Head;
                do
                {
                    if (temp.NeighborColor == color)
                    {
                        return temp;
                    }
                    temp = temp.Next;
                } while (temp != Head);
                return null;
            }
        }

        public class BoundsMessageGroup
        {
            public Dictionary<Color,BoundsCircularLink> BoundsMessage;
            public BoundsMessageGroup()
            {
                BoundsMessage = new Dictionary<Color, BoundsCircularLink>();
            }
            public void AddPoint(Color mColor,Color nColor,Vector3Int vector3)
            {
                if (!BoundsMessage.ContainsKey(mColor))
                {
                    BoundsNode boundsNode = new BoundsNode(mColor, nColor);
                    boundsNode.Boundary.Add(vector3);
                    BoundsMessage.Add(mColor, new BoundsCircularLink(boundsNode));
                }
                else
                {
                    if (!BoundsMessage[mColor].ConditionsColor(nColor))
                    {
                        BoundsMessage[mColor].Insert(new BoundsNode(mColor , nColor));
                    }
                    BoundsMessage[mColor].GetNerighbor(nColor).Boundary.Add(vector3);
                }
            }

            public List<List<Vector3>> GetAllJunctionLine(Color mColor)
            {
                BoundsCircularLink temp = BoundsMessage[mColor];
                List<List<Vector3>> outValue = new List<List<Vector3>>(temp.GetAllJunctionLine());
                List<Color> nColors1 = new List<Color>(colors_neigbor[mColor]);
                HashSet<Color> nColors2 = temp.GetNeighborsColor();
                foreach (var item in nColors1)
                {
                    if (!nColors2.Contains(item))
                    {
                        outValue.AddRange(BoundsMessage[item].GetNerighbor(mColor).Filtered);

                    }
                }
                return outValue;
            }

        }

        private IEnumerator GeneretorLines_v2()
        {
            yield return new WaitForEndOfFrame();
            ResetMapColor();
            BoundsMessageGroup group = GetBoundsLine();
            //Sort(group);
            Transform nidie = new GameObject("LineDad").transform;
            Transform nidie1 = new GameObject("Border").transform;
            foreach (var item in group.BoundsMessage)
            {
                BoundsNode temp = item.Value.Head;
                do
                {
                    if (temp.Boundary != null)
                    {
                        List<List<Vector3Int>> linesPoints = temp.SplitAndSortBoundary(temp.Boundary);
                        foreach (var linePoints in linesPoints)
                        {
                            List<Vector3> filtered = linePoints.Select(v => (Vector3)v).ToList();
                            temp.Filtered.Add(filtered);
                        }
                    }
                    temp = temp.Next;
                } while (temp != item.Value.Head);
                List<List<Vector3>> llTemp = group.GetAllJunctionLine(temp.SelfColor);
                // 提取每个明确闭合的多边形
                List<Vector3> meshPoints = CombineSegmentsIntoClosedPolygon(llTemp);
            }

            foreach (var item in group.BoundsMessage)
            {
                BoundsNode temp = item.Value.Head;
                do
                {
                    if (temp.Boundary != null)
                    {
                        for (int i = 0; i < temp.Filtered.Count; i++)
                        {
                            if (Catmull)
                                temp.Filtered[i] = GetClosedCatmullRomSpline(temp.Filtered[i], InsertPointCount);
                            else
                                temp.Filtered[i] = GetCatmullRomSplineOpen(temp.Filtered[i], InsertPointCount);

                            if (VisvalingamWhyatt)
                                //filtered = Simplify(filtered, Area);
                                temp.Filtered[i] = SimplifyLine(temp.Filtered[i], Area);

                            if (Catmull)
                                temp.Filtered[i] = GetClosedCatmullRomSpline(temp.Filtered[i], InsertPointCount);
                            else
                                temp.Filtered[i] = GetCatmullRomSplineOpen(temp.Filtered[i], InsertPointCount);

                            if (temp.Filtered[i] == null)
                            {
                                continue;
                            }
                            //temp.Filtered.Add(temp.Filtered[i]);
                        }
                    }
                    temp = temp.Next;
                } while (temp != item.Value.Head);
            }

            Transform meshDad = new GameObject("Country").transform;
            //foreach (var item in group.BoundsMessage)
            //{
            //    BoundsNode temp = item.Value.Head;
            //    List<List<Vector3>> llTemp = group.GetAllJunctionLine(temp.SelfColor);

            //    var separatedAreas = SplitIntoSeparateAreas(llTemp);

            //    foreach (var areaSegments in separatedAreas)
            //    {
            //        List<List<Vector3>> allContours = areaSegments.Select(seg => CombineSegmentsIntoClosedPolygon(new List<List<Vector3>> { seg })).ToList();

            //        Mesh mesh = GenerateMeshWithHoles(allContours);

            //        foreach (var lineItem in allContours)
            //        {
            //            // 绘制线
            //            LineRenderer LR1 = Instantiate(LR);
            //            LR1.startWidth = LineWidth;
            //            LR1.endWidth = LineWidth;
            //            LR1.positionCount = lineItem.Count;
            //            LR1.transform.parent = nidie;
            //            LR1.SetPositions(lineItem.ToArray());
            //        }

            //        GameObject gg = new GameObject("Mesh_" + temp.SelfColor.ToString());
            //        MeshFilter mf = gg.AddComponent<MeshFilter>();
            //        MeshRenderer mr = gg.AddComponent<MeshRenderer>();
            //        mf.mesh = mesh;
            //        mr.material = new Material(gridMat) { color = temp.SelfColor };
            //        gg.transform.parent = meshDad;
            //    }
            //}


            //foreach (var item in group.BoundsMessage)
            //{
            //    BoundsNode temp = item.Value.Head;
            //    List<List<Vector3>> llTemp = group.GetAllJunctionLine(temp.SelfColor);

            //    // 区域分离
            //    var separatedAreas = SplitIntoSeparateAreas(llTemp);

            //    foreach (var areaSegments in separatedAreas)
            //    {
            //        // 拼接每个区域
            //        List<Vector3> polygon = CombineSegmentsIntoClosedPolygon(areaSegments);

            //        // 绘制线
            //        LineRenderer LR1 = Instantiate(LR);
            //        LR1.startWidth = LineWidth;
            //        LR1.endWidth = LineWidth;
            //        LR1.positionCount = polygon.Count;
            //        LR1.transform.parent = nidie;
            //        LR1.SetPositions(polygon.ToArray());

            //        // 生成Mesh
            //        Mesh mesh;
            //        if (!GetConnected(polygon))
            //            mesh = GenerateMeshFromPolygons(new List<List<Vector3>> { polygon });
            //        else
            //        {
            //            Debug.Log("闭合多边形");
            //            //List<Vector3> meshPoints = CombineSegmentsIntoClosedPolygon(llTemp);
            //            mesh = GenerateMeshFromPolygon(polygon);
            //        }

            //        // 创建 GameObject 显示该 Mesh
            //        GameObject gg = new GameObject("Mesh_" + temp.SelfColor.ToString());
            //        MeshFilter mf = gg.AddComponent<MeshFilter>();
            //        MeshRenderer mr = gg.AddComponent<MeshRenderer>();
            //        mf.mesh = mesh;
            //        mr.material = new Material(gridMat) { color = temp.SelfColor };
            //        gg.transform.parent = meshDad;
            //    }

            //foreach (var item in group.BoundsMessage)
            //{
            //    BoundsNode temp = item.Value.Head;
            //    List<List<Vector3>> llTemp = group.GetAllJunctionLine(temp.SelfColor);

            //    // 明确提取闭合多边形
            //    List<List<Vector3>> closedPolygons = ExtractClosedPolygons(llTemp);
            //    if (closedPolygons == null)
            //    {
            //        continue;
            //    }

            //    // 生成含孔洞Mesh
            //    Mesh mesh = GenerateMeshWithHoles(closedPolygons);

            //    // 绘制Mesh
            //    GameObject gg = new GameObject("Mesh_" + temp.SelfColor.ToString());
            //    MeshFilter mf = gg.AddComponent<MeshFilter>();
            //    MeshRenderer mr = gg.AddComponent<MeshRenderer>();
            //    mf.mesh = mesh;
            //    mr.material = new Material(gridMat) { color = temp.SelfColor };
            //    gg.transform.parent = meshDad;
            //}
            _criticalGameobjects.Clear();
            foreach (var item in group.BoundsMessage)
            {
                BoundsNode temp = item.Value.Head;
                List<List<Vector3>> llTemp = group.GetAllJunctionLine(temp.SelfColor);
                if (llTemp == null)
                {
                    continue;
                }
                // 提取每个明确闭合的多边形
                List<Vector3> meshPoints = CombineSegmentsIntoClosedPolygon(llTemp);
                meshPoints = meshPoints.ConvertAll(p => _transMatr.MultiplyPoint(p));
                if (!c2m.ContainsKey(temp.SelfColor))
                {
                    c2m.Add(temp.SelfColor, new List<MeshRenderer>());
                }


                if (GetConnected(meshPoints , 5))
                {
                    //LineRenderer LR1 = Instantiate(LR);
                    //LR1.startWidth = LineWidth;
                    //LR1.endWidth = LineWidth;
                    ////LR1.material.color = ParseCriticalID(item.Key).Item1;
                    //LR1.positionCount = meshPoints.Count;
                    //LR1.transform.parent = nidie;
                    //LR1.SetPositions(meshPoints.ToArray());
                    Mesh mesh = GenerateMeshFromPolygon(meshPoints);
                    // 创建新 GameObject 用于显示该 Mesh
                    GameObject gg = new GameObject("Mesh_" + temp.SelfColor.ToString());

                    // 添加 MeshFilter 和 MeshRenderer 组件
                    MeshFilter mf = gg.AddComponent<MeshFilter>();
                    MeshRenderer mr = gg.AddComponent<MeshRenderer>();

                    // 赋值生成的 Mesh
                    mf.mesh = mesh;

                    // 创建材质实例，并设置颜色
                    mr.material = new Material(gridMat) { color = temp.SelfColor };
                    gg.transform.parent = meshDad;
                    c2m[temp.SelfColor].Add(mr);
                }
                else
                {
                    List<List<Vector3>> allAreas = SplitByDistance(meshPoints);

                    foreach (var splitArea in allAreas)
                    {
                        //List<Vector3> tempPoints = splitArea;
                        //if (Vector3.Distance(tempPoints[0] , tempPoints[tempPoints.Count - 1])>0.001f)
                        //{

                        //}
                        //LineRenderer LR1 = Instantiate(LR);
                        //LR1.startWidth = LineWidth;
                        //LR1.endWidth = LineWidth;
                        ////LR1.material.color = ParseCriticalID(item.Key).Item1;
                        //LR1.positionCount = splitArea.Count;
                        //LR1.transform.parent = nidie;
                        //LR1.SetPositions(splitArea.ToArray());

                        Mesh mesh = GenerateMeshFromPolygon(splitArea);
                        // 创建新 GameObject 用于显示该 Mesh
                        GameObject gg = new GameObject("Mesh_" + temp.SelfColor.ToString());

                        // 添加 MeshFilter 和 MeshRenderer 组件
                        MeshFilter mf = gg.AddComponent<MeshFilter>();
                        MeshRenderer mr = gg.AddComponent<MeshRenderer>();

                        // 赋值生成的 Mesh
                        mf.mesh = mesh;

                        // 创建材质实例，并设置颜色
                        mr.material = new Material(gridMat) { color = temp.SelfColor };
                        gg.transform.parent = meshDad;
                        c2m[temp.SelfColor].Add(mr);
                    }
                }


                #region draw line
                do
                {
                    Int64 lColor = GenerateCriticalID(temp.SelfColor ,temp.NeighborColor);
                    if (!_criticalGameobjects.ContainsKey(lColor))
                    {
                        _criticalGameobjects.Add(lColor,new List<GameObject>());
                    }
                    if (temp.Filtered != null)
                    {
                        foreach (var linePoints in temp.Filtered)
                        {

                            if (linePoints == null)
                            {
                                continue;
                            }
                            List<Vector3> pointsConverted = linePoints.ConvertAll(p => _transMatr.MultiplyPoint(p));
                            LineRenderer LR1 = Instantiate(LR);
                            LR1.startWidth = LineWidth;
                            LR1.endWidth = LineWidth;
                            //LR1.material.color = ParseCriticalID(item.Key).Item1;
                            LR1.positionCount = pointsConverted.Count;
                            LR1.transform.parent = nidie1;
                            LR1.SetPositions(pointsConverted.ToArray());
                            _criticalGameobjects[lColor].Add(LR1.gameObject);
                        }
                    }
                    temp = temp.Next;
                } while (temp != item.Value.Head);
                #endregion

            }
            Dictionary<Vector3, List<Vector3>> BuildAdjacency(List<List<Vector3>> segments)
            {
                var adj = new Dictionary<Vector3, List<Vector3>>();

                foreach (var seg in segments)
                {
                    if (!adj.ContainsKey(seg[0]))
                        adj[seg[0]] = new List<Vector3>();
                    if (!adj.ContainsKey(seg[seg.Count - 1]))
                        adj[seg[seg.Count - 1]] = new List<Vector3>();

                    adj[seg[0]].Add(seg[seg.Count - 1]);
                    adj[seg[seg.Count - 1]].Add(seg[0]);
                }
                return adj;
            }


            List<List<List<Vector3>>> ClusterSegmentsByConnectivity(List<List<Vector3>> segments, float threshold = 1f)
            {
                List<List<List<Vector3>>> clusters = new List<List<List<Vector3>>>();
                HashSet<int> visited = new HashSet<int>();

                for (int i = 0; i < segments.Count; i++)
                {
                    if (visited.Contains(i))
                        continue;

                    List<List<Vector3>> cluster = new List<List<Vector3>>();
                    Queue<int> queue = new Queue<int>();
                    queue.Enqueue(i);
                    visited.Add(i);

                    while (queue.Count > 0)
                    {
                        int idx = queue.Dequeue();
                        var seg = segments[idx];
                        if (seg == null)
                            continue;
                        cluster.Add(seg);

                        for (int j = 0; j < segments.Count; j++)
                        {
                            if (visited.Contains(j)) continue;
                            if (segments[j] == null)
                            {
                                continue;
                            }
                            if (SegmentsAreConnected(seg, segments[j], threshold))
                            {
                                visited.Add(j);
                                queue.Enqueue(j);
                            }
                        }
                    }

                    clusters.Add(cluster);
                }

                return clusters;
            }
            List<List<Vector3>> ExtractAllClosedPolygons1(List<List<Vector3>> segments, float connectThreshold = 1f, float joinTolerance = 0.5f)
            {
                var clusters = ClusterSegmentsByConnectivity(segments, connectThreshold);
                var result = new List<List<Vector3>>();

                foreach (var cluster in clusters)
                {
                    var polygons = ExtractClosedPolygons(cluster, joinTolerance);
                    result.AddRange(polygons);
                }

                return result;
            }
            List<List<Vector3>> ExtractClosedPolygons(List<List<Vector3>> segments, float tolerance = 0.5f)
            {
                List<(Vector3 start, Vector3 end)> edges = new List<(Vector3, Vector3)>();

                // 拆分所有线段为起点终点对（忽略中间点）
                foreach (var seg in segments)
                {
                    if (seg == null)
                    {
                        continue;
                    }
                    for (int i = 0; i < seg.Count - 1; i++)
                    {
                        edges.Add((seg[i], seg[i + 1]));
                    }
                }

                List<List<Vector3>> polygons = new List<List<Vector3>>();
                HashSet<int> used = new HashSet<int>();

                for (int i = 0; i < edges.Count; i++)
                {
                    if (used.Contains(i))
                        continue;

                    List<Vector3> polygon = new List<Vector3>();
                    polygon.Add(edges[i].start);
                    polygon.Add(edges[i].end);
                    used.Add(i);

                    Vector3 current = edges[i].end;

                    bool closed = false;
                    while (!closed)
                    {
                        bool found = false;
                        for (int j = 0; j < edges.Count; j++)
                        {
                            if (used.Contains(j)) continue;

                            if (Vector3.Distance(current, edges[j].start) <= tolerance)
                            {
                                polygon.Add(edges[j].end);
                                current = edges[j].end;
                                used.Add(j);
                                found = true;
                                break;
                            }
                            else if (Vector3.Distance(current, edges[j].end) <= tolerance)
                            {
                                // 反转方向
                                polygon.Add(edges[j].start);
                                current = edges[j].start;
                                used.Add(j);
                                found = true;
                                break;
                            }
                        }

                        // 如果已经回到起点，形成闭合
                        if (Vector3.Distance(current, polygon[0]) <= tolerance)
                        {
                            closed = true;
                            polygon.Add(polygon[0]); // 闭合
                            break;
                        }

                        if (!found)
                        {
                            break; // 无法继续匹配
                        }
                    }

                    if (polygon.Count >= 4 && closed)
                        polygons.Add(polygon);
                }

                return polygons;
            }


            bool GetConnected(List<Vector3> pIn_AreaPoints, float maxDis = 5f)
            {
                Vector3 pre = pIn_AreaPoints[0];
                Vector3 cur;
                for (int i = 1; i < pIn_AreaPoints.Count; i++)
                {
                    cur = pIn_AreaPoints[i];
                    if (Vector3.Distance(pre, cur) > maxDis)
                    {
                        return false;
                    }
                    pre = cur;
                }
                return true;
            }

            List<List<List<Vector3>>> SplitIntoSeparateAreas(List<List<Vector3>> segments, float threshold = 0.1f)
            {
                List<List<List<Vector3>>> areas = new List<List<List<Vector3>>>();

                while (segments.Count > 0)
                {
                    List<List<Vector3>> currentArea = new List<List<Vector3>>();
                    Queue<List<Vector3>> queue = new Queue<List<Vector3>>();
                    queue.Enqueue(segments[0]);
                    segments.RemoveAt(0);

                    while (queue.Count > 0)
                    {
                        var seg = queue.Dequeue();
                        currentArea.Add(seg);

                        for (int i = segments.Count - 1; i >= 0; i--)
                        {
                            if (SegmentsAreClose(seg, segments[i], threshold))
                            {
                                queue.Enqueue(segments[i]);
                                segments.RemoveAt(i);
                            }
                        }
                    }
                    areas.Add(currentArea);
                }

                return areas;
            }

            bool SegmentsAreClose(List<Vector3> segA, List<Vector3> segB, float threshold)
            {
                foreach (var pA in segA)
                    foreach (var pB in segB)
                        if (Vector3.Distance(pA, pB) < threshold)
                            return true;

                return false;
            }


            List<Vector3> CombineSegmentsIntoClosedPolygon(List<List<Vector3>> segments)
            {
                if (segments == null || segments.Count == 0)
                    return new List<Vector3>();

                List<Vector3> polygon = new List<Vector3>();

                // 创建副本以避免修改原数据
                List<List<Vector3>> remainingSegments = new List<List<Vector3>>(segments);

                // 以第一条线段的起点为起始点
                List<Vector3> currentSegment = remainingSegments[0];
                polygon.AddRange(currentSegment);
                remainingSegments.RemoveAt(0);

                Vector3 currentPoint = polygon[polygon.Count - 1];

                while (remainingSegments.Count > 0)
                {
                    float minDist = float.MaxValue;
                    int nearestSegmentIndex = -1;
                    bool reverseSegment = false;

                    // 寻找下一个最近的线段（起点或终点）
                    for (int i = 0; i < remainingSegments.Count; i++)
                    {
                        var seg = remainingSegments[i];
                        if (seg == null)
                        {
                            continue;
                        }
                        float distToStart = Vector3.Distance(currentPoint, seg[0]);
                        float distToEnd = Vector3.Distance(currentPoint, seg[seg.Count - 1]);

                        if (distToStart < minDist)
                        {
                            minDist = distToStart;
                            nearestSegmentIndex = i;
                            reverseSegment = false;
                        }

                        if (distToEnd < minDist)
                        {
                            minDist = distToEnd;
                            nearestSegmentIndex = i;
                            reverseSegment = true;
                        }
                    }

                    // 找到最近的线段并拼合
                    if (nearestSegmentIndex != -1)
                    {
                        List<Vector3> nearestSegment = remainingSegments[nearestSegmentIndex];

                        if (reverseSegment)
                            nearestSegment.Reverse();

                        // 避免重复点
                        if (Vector3.Distance(currentPoint, nearestSegment[0]) < 0.001f)
                            nearestSegment.RemoveAt(0);

                        polygon.AddRange(nearestSegment);
                        currentPoint = polygon[polygon.Count - 1];

                        remainingSegments.RemoveAt(nearestSegmentIndex);
                    }
                    else
                    {
                        // 没有找到可拼合的线段，可能数据不连续
                        break;
                    }
                }

                // 最后检查是否需要闭合多边形
                if (polygon.Count > 0 && Vector3.Distance(polygon[0], polygon[polygon.Count - 1]) > 0.001f)
                    polygon.Add(polygon[0]);

                return polygon;
            }
        }

        private void Sort(BoundsMessageGroup group)
        {
            foreach (var item in group.BoundsMessage)
            {
                BoundsNode temp = item.Value.Head;
                do
                {
                    Vector3Int start = temp.Boundary.First();
                    temp.Boundary = TraceContour(ref temp.Boundary, start);
                    temp = temp.Next;
                } while (temp != item.Value.Head);
            }
            //while (points.Count > 0)
            //{
            //    // 随机取出一个点作为当前区域的起点
            //    Vector3Int start = points.First();
            //    List<Vector3Int> contour = TraceContour(ref points, start);
            //    if (contour != null && contour.Count >= 2)
            //    {
            //        sortedContours.Add(contour);
            //    }
            //    else
            //    {
            //        Debug.Log($"{contour != null} , {contour?.Count}");
            //    }
            //    yield return null;
            //}
        }

        private IEnumerator GeneretorLines()
        {
            yield return new WaitForEndOfFrame();
            ResetMapColor();
            allAreas = GetColorEdges();
            //2025年4月8日13点44分 这里OK


            Dictionary<Color, List<List<Vector3>>> meshArray = new Dictionary<Color, List<List<Vector3>>>();

            for (int i = 0; i < allAreas.Count; i++)
            {
                Vector2Ints = allAreas[i];
                Color m_color = ASD.GetPixel(Vector2Ints[0].x , Vector2Ints[0].y);
                if (!meshArray.ContainsKey(m_color))
                {
                    meshArray.Add(m_color , new List<List<Vector3>>());
                }

                //LineRenderer LR1 = Instantiate(LR);
                //LR1.startWidth = LineWidth;
                //LR1.endWidth = LineWidth;
                //LR1.positionCount = Vector2Ints.Count;
                //LR1.SetPositions(Vector2Ints.Select(v => (Vector3)v).ToArray());
                //continue;

                //foreach (var item in Vector2Ints)
                //{
                //    sb.Append($"{item.x},{item.y}    ");
                //}
                //sb.Append($"\n");
                //Parse();

                {
                    //List<List<Vector3Int>> sortedContours = ExtractContours(Vector2Ints);
                    HashSet<Vector3Int> points = new HashSet<Vector3Int>(Vector2Ints);
                    //if (i == 0)
                    //{
                    //    WriteHashSet(points);
                    //}
                    // 复制一份用于处理，避免修改原始数据
                    //HashSet<Vector3Int> unvisited = new HashSet<Vector3Int>(points);
                    List<List<Vector3Int>> sortedContours = new List<List<Vector3Int>>();

                    while (points.Count > 0)
                    {
                        // 随机取出一个点作为当前区域的起点
                        Vector3Int start = points.First();
                        List<Vector3Int> contour = TraceContour(ref points, start);
                        if (contour != null && contour.Count >= 2)
                        {
                            sortedContours.Add(contour);
                        }
                        else
                        {
                            Debug.Log($"{contour != null} , {contour?.Count}");
                        }
                            yield return null;
                    }


                    if (sortedContours.Count >= Max)
                    {
                        continue;
                    }


                    foreach (var region in sortedContours)
                    {
                        List<Vector3> temp = region.Select(v => (Vector3)v).ToList();

                        #region 剔除
                        //排序存在问题 ， 可能会出现两个距离特别远的点连接在一起，原因暂时不详,在这里暂做提出处理
                        // 定义距离阈值
                        float distanceThreshold = 3f;

                        // 用于存储过滤后的结果
                        List<Vector3> filtered = new List<Vector3>();

                        // 如果 temp 非空，首先将第一个点加入结果
                        if (temp.Count > 0)
                        {
                            filtered.Add(temp[0]);
                        }

                        // 从第二个点开始依次判断
                        for (int index = 1; index < temp.Count; index++)
                        {
                            // 获取上一个已保留的点
                            Vector3 lastPoint = filtered[filtered.Count - 1];
                            // 计算距离
                            float dist = Vector3.Distance(lastPoint, temp[index]);

                            // 如果距离小于或等于阈值，则加入结果，否则跳过该点
                            if (dist <= distanceThreshold)
                            {
                                filtered.Add(temp[index]);
                            }
                        }
                        #endregion


                        //Catmull-Rom 无法消除锯齿边界
                        //boundary = GetCatmullRomSpline(boundary, InsertPointCount);
                        if (Catmull)
                            filtered = GetClosedCatmullRomSpline(filtered, InsertPointCount);
                        else
                            filtered = GetCatmullRomSpline(filtered, InsertPointCount);

                        if (VisvalingamWhyatt)
                            filtered = Simplify(filtered, Area);

                        if (Catmull)
                            filtered = GetClosedCatmullRomSpline(filtered, InsertPointCount);
                        else
                            filtered = GetCatmullRomSpline(filtered, InsertPointCount);
                        //temp = GetCatmullRomSpline(temp, InsertPointCount);
                        //if (VisvalingamWhyatt)
                        //    filtered = Simplify(filtered, Area*2);


                        //if (Catmull)
                        //    filtered = GetClosedCatmullRomSpline(filtered, InsertPointCount);
                        //else
                        //    filtered = GetCatmullRomSpline(filtered, InsertPointCount);

                        //if (Catmull)
                        //    filtered = GetClosedCatmullRomSpline(filtered, InsertPointCount);
                        if (filtered == null)
                        {
                            continue;
                        }
                        meshArray[m_color].Add(filtered);
                        //LineRenderer LR1 = Instantiate(LR);
                        //LR1.startWidth = LineWidth;
                        //LR1.endWidth = LineWidth;
                        //LR1.positionCount = filtered.Count;
                        //LR1.SetPositions(filtered.ToArray());
                        // 显示每个区域的点数量
                        //Debug.Log($"Region Size: {region.Count}, Boundary Size: {boundary.Count}");
                    }
                }
                //break;
                yield return null;
            }

            #region 变换

            foreach (var item in meshArray)
            {
                foreach (var item1 in item.Value)
                {
                    for (int i = 0; i < item1.Count; i++)
                    {
                        Vector3 vector3 = _transMatr.MultiplyPoint(item1[i]);
                        vector3.y = lineHeight;
                        item1[i] = vector3;
                    }
                }
            }
            #endregion

            #region 合并操作：修正区域间交界问题
            // meshArray 为 Dictionary<Color, List<List<Vector3>>>
            // colors 为存放所有颜色键的集合
            // colors_neigbor 为每个颜色对应的相邻颜色列表

            // 临时字典，用于存储合并后的mesh数据
            Dictionary<Color, List<List<Vector3>>> tempMeshArray = new Dictionary<Color, List<List<Vector3>>>();

            tempMeshArray.Clear();
            // 定义一个距离阈值（例如2个单位，实际依据你数据的尺度来设定）
            float mergeThreshold = 5f;
            // 遍历每个区域（用颜色做 Key）
            foreach (Color itemColor in meshArray.Keys)
            {
                // 获取当前区域的邻居颜色列表
                List<Color> neighborColors;
                if (!colors_neigbor.TryGetValue(itemColor, out neighborColors))
                {
                    neighborColors = new List<Color>();
                }

                // 复制当前区域点数据，防止修改原始数据
                List<List<Vector3>> regionPoints = new List<List<Vector3>>();
                foreach (List<Vector3> subset in meshArray[itemColor])
                {
                    regionPoints.Add(new List<Vector3>(subset));
                }
                tempMeshArray.Add(itemColor, regionPoints);

                // 针对当前区域的每个点集进行处理
                for (int i = 0; i < regionPoints.Count; i++)
                {
                    List<Vector3> subset = regionPoints[i];

                    // 收集候选点：包含当前区域的点以及所有邻近区域的点
                    List<PointData> candidatePoints = new List<PointData>();
                    // 添加当前区域中的点（标记为 itemColor）
                    foreach (Vector3 pt in subset)
                    {
                        candidatePoints.Add(new PointData(pt, itemColor));
                    }
                    // 添加邻接区域的点
                    foreach (Color neighborColor in neighborColors)
                    {
                        if (meshArray.ContainsKey(neighborColor))
                        {
                            foreach (List<Vector3> neighborSubset in meshArray[neighborColor])
                            {
                                foreach (Vector3 pt in neighborSubset)
                                {
                                    candidatePoints.Add(new PointData(pt, neighborColor));
                                }
                            }
                        }
                    }

                    // 构造 KD-Tree（3 维），使用 FloatMath 作为比较函数
                    var kdTree = new KdTree<float, PointData>(3, new FloatMath());
                    foreach (PointData pd in candidatePoints)
                    {
                        float[] key = new float[] { pd.Position.x, pd.Position.y, pd.Position.z };
                        kdTree.Add(key, pd);
                    }

                    // 对当前点集中的每个点进行处理
                    for (int j = 0; j < subset.Count; j++)
                    {
                        Vector3 currentPoint = subset[j];
                        float[] queryKey = new float[] { currentPoint.x, currentPoint.y, currentPoint.z };

                        // 限定最大返回数量，避免使用 int.MaxValue 导致内存占用过高
                        int maxCount = Mathf.Min(candidatePoints.Count, 100);
                        var neighbors = kdTree.RadialSearch(queryKey, mergeThreshold, maxCount);
                        if (neighbors.Length > 1)
                        {
                            // 使用所有邻近点的平均值进行合并
                            Vector3 sum = Vector3.zero;
                            Color neighborColorForID = itemColor;  // 默认使用当前区域颜色
                            Vector3 minPos = Vector3.one * int.MaxValue;
                            foreach (var node in neighbors)
                            {
                                sum += node.Value.Position;
                                // 如果找到的邻近点所属颜色与当前区域不同，则记录为邻接颜色
                                if (node.Value.PointColor != itemColor)
                                    neighborColorForID = node.Value.PointColor;
                            }
                            Vector3 mergedPoint = sum / neighbors.Length;
                            // 更新当前点为合并后位置
                            subset[j] = mergedPoint;
                            // 生成关键 ID：结合当前区域颜色和邻接区域颜色
                            long criticalID = GenerateCriticalID(itemColor, neighborColorForID);
                            if (!_criticalPoints.ContainsKey(criticalID))
                            {
                                _criticalPoints.Add(criticalID, new List<Vector3>());
                            }
                            _criticalPoints[criticalID].Add(mergedPoint);
                        }
                        //else if (neighbors.Length == 1)
                        //{
                        //    PointData tempData = neighbors.First().Value;
                        //    long criticalID = GenerateCriticalID(itemColor, tempData.PointColor);
                        //    if(!_criticalPoints[criticalID].Contains(tempData.Position))
                        //    _criticalPoints[criticalID].Add(tempData.Position);
                        //}
                    }
                }
            }

            //foreach (Color itemColor in colors.Keys)
            //{
            //    List<Color> neighbors = colors_neigbor[itemColor]; // 当前颜色对应的相邻颜色列表

            //    // 当前颜色区域的点集（可能包含多个子集或连通块）
            //    List<List<Vector3>> selfPoints = meshArray[itemColor];

            //    // 将当前颜色的数据放入临时字典
            //    tempMeshArray.Add(itemColor, selfPoints);
            //    // 遍历当前颜色区域内的所有子集
            //    int subsetCount = selfPoints.Count;
            //    for (int i = 0; i < subsetCount; i++)
            //    {
            //        List<Vector3> subsetVectors = selfPoints[i];
            //        int subsetPointsCount = subsetVectors.Count;

            //        // 遍历子集中的每个点
            //        for (int pointIndex = 0; pointIndex < subsetPointsCount; pointIndex++)
            //        {
            //            Vector3 vTemp = subsetVectors[pointIndex];
            //            bool merged = false;

            //            // 遍历所有相邻颜色
            //            foreach (Color neighborColor in neighbors)
            //            {
            //                // 防止可能存在的键不存在情况
            //                if (!meshArray.ContainsKey(neighborColor))
            //                    continue;
            //                HashSet<(Vector3, float)> nearbyPoint = HashSetPool<(Vector3, float)>.Get();
            //                float distance = 0;
            //                // 遍历相邻区域的所有子集
            //                foreach (List<Vector3> neighborSubset in meshArray[neighborColor])
            //                {
            //                    // 遍历子集中所有点
            //                    foreach (Vector3 neighborPoint in neighborSubset)
            //                    {
            //                        distance = Vector3.Distance(neighborPoint, vTemp);
            //                        if (distance < mergeThreshold)
            //                        {
            //                            nearbyPoint.Add((neighborPoint, distance));
            //                            //if (subsetVectors.Contains(neighborPoint))
            //                            //{
            //                            //    continue;
            //                            //}

            //                            //// 当两点之间距离小于阈值，则将当前点合并为邻居的该点
            //                            //subsetVectors[pointIndex] = neighborPoint;
            //                            //Int64 temp = GenerateCriticalID(itemColor, neighborColor);
            //                            //if (!_criticalPoints.ContainsKey(temp))
            //                            //{
            //                            //    _criticalPoints.Add(temp, new List<Vector3>());
            //                            //}
            //                            //_criticalPoints[temp].Add(neighborPoint);
            //                            //merged = true;
            //                            //break; // 找到一个合并点后，跳出邻居点遍历
            //                        }
            //                    }
            //                    //if (merged) break; // 退出当前邻居子集
            //                }
            //                //if (merged) break; // 退出当前相邻颜色的遍历
            //                if (nearbyPoint.Count > 1)
            //                {
            //                    Vector3 sum = Vector3.zero;
            //                    foreach (var item in nearbyPoint)
            //                    {
            //                        sum += item.Item1;
            //                    }
            //                    Vector3 tpos = sum / nearbyPoint.Count;
            //                    subsetVectors[pointIndex] = tpos;
            //                    Int64 temp = GenerateCriticalID(itemColor, neighborColor);
            //                    if (!_criticalPoints.ContainsKey(temp))
            //                    {
            //                        _criticalPoints.Add(temp, new List<Vector3>());
            //                    }
            //                    _criticalPoints[temp].Add(tpos);
            //                }
            //                nearbyPoint.Clear();
            //                HashSetPool<(Vector3, float)>.Release(nearbyPoint);
            //            }
            //        }
            //    }
            //    yield return null;
            //}
            Debug.Log($"交界线数量为{_criticalPoints.Count}");
            Transform junctionDad = new GameObject("JunctionDad").transform;
            _criticalGameobjects.Clear();
            foreach (var item in _criticalPoints)
            {
                //超过这个阈值则判定存在两国交界不唯一的情况。
                float maxDis = 10;
                List<List<Vector3>> allCritical = new List<List<Vector3>>() 
                {
                    new List<Vector3>()
                };
                int acIndex = 0;
                allCritical[acIndex].Add(item.Value[0]);
                for (int i =1; i < item.Value.Count; i++)
                {
                    if (Vector3.Distance(item.Value[i - 1] , item.Value[i]) > maxDis)
                    {
                        acIndex++;
                        allCritical.Add(new List<Vector3>());
                    }
                    allCritical[acIndex].Add(item.Value[i]);
                }
                _criticalGameobjects.Add(item.Key, new List<GameObject>());

                foreach (var cps in allCritical)
                {
                    LineRenderer LR1 = Instantiate(LR);
                    LR1.startWidth = LineWidth;
                    LR1.endWidth = LineWidth;
                    LR1.material.color = ParseCriticalID(item.Key).Item1;
                    LR1.positionCount = cps.Count;
                    LR1.transform.parent = junctionDad;
                    LR1.SetPositions(cps.ToArray());
                    _criticalGameobjects[item.Key].Add(LR1.gameObject);
                    LR1.gameObject.SetActive(false);
                }

            }

            #endregion

            meshDad = new GameObject("MeshDad").transform;
            lineDad = new GameObject("LineDad").transform;
            float depth = -0.1f;
            c2m.Clear();
            foreach (var item in tempMeshArray)
            {
                Color lineColor = item.Key;
                lineColor.a = 0.5f;
                List<List<Vector3>> linePoints = item.Value;
                c2m.Add(item.Key, new List<MeshRenderer>());

                foreach (var Points in linePoints)
                {
                    // 从字典中取得当前颜色及对应的边界数据

                    List<List<Vector3>> boundaries = item.Value;

                    // 根据边界数据生成 Mesh
                    //Mesh mesh = GenerateMeshFromBoundaries(Points);
                    Mesh mesh = GenerateMeshFromPolygon(Points);
                    
                    // 创建新 GameObject 用于显示该 Mesh
                    GameObject gg = new GameObject("Mesh_" + lineColor.ToString());

                    // 添加 MeshFilter 和 MeshRenderer 组件
                    MeshFilter mf = gg.AddComponent<MeshFilter>();
                    MeshRenderer mr = gg.AddComponent<MeshRenderer>();

                    // 赋值生成的 Mesh
                    mf.mesh = mesh;

                    // 创建材质实例，并设置颜色
                    mr.material = new Material(gridMat) { color = lineColor };
                    gg.transform.parent = meshDad;
                    c2m[item.Key].Add(mr);
                    // 如果边界数据比较多，不影响帧率的话可以用 yield return null 让协程分帧执行
                    yield return null;


                    //#region 数据去尾并移除边界

                    //List<Vector3> linePoints1 = new List<Vector3>(Points);
                    //linePoints1.RemoveAt(linePoints1.Count - 1);
                    //List<Color> neigborsColor = colors_neigbor[item.Key];
                    //foreach (var itemC in neigborsColor)
                    //{
                    //    Int64 boundID = GenerateCriticalID(item.Key,itemC);
                    //    List<Vector3> criticalPos = _criticalPoints[boundID];
                    //    foreach (var iiii in criticalPos)
                    //    {
                    //        // 先查找该点在 linePoints1 中的位置
                    //        int index = linePoints1.IndexOf(iiii);
                    //        // 只删除既不是首点也不是尾点的点
                    //        if (index > 0 && index < linePoints1.Count - 1)
                    //        {
                    //            linePoints1.Remove(iiii);
                    //        }
                    //    }
                    //}
                    //#endregion

                    LineRenderer LR1 = Instantiate(LR);
                    LR1.startWidth = LineWidth;
                    LR1.endWidth = LineWidth;
                    //LR1.material.color = lineColor;
                    LR1.material.color = Color.black;
                    LR1.positionCount = Points.Count;
                    LR1.transform.parent = lineDad;
                    LR1.SetPositions(Points.ToArray());

                }
                depth -= 0.1f;
            }


            //// 将字符串转换为字节数组（使用UTF8编码）
            //byte[] bytes = Encoding.UTF8.GetBytes(sb.ToString());

            //// 写入字节数组到文件中
            //fs.Write(bytes, 0, bytes.Length);
            //fs.Close();

            //foreach (var item in colors)
            //{
            //    LineRenderer LR1 = Instantiate(LR);
            //    LR1.startWidth = LineWidth;
            //    LR1.endWidth = LineWidth;
            //    Parse(item, LR1);
            //    yield return null;
            //}
        }

        #region 凸包算法
        public static List<Vector3> ComputeConcaveHull(List<Vector3> points, int k = 5)
        {
            if (points.Count < 3) return points;

            var hull = new List<Vector3>();
            var used = new HashSet<Vector3>();
            var current = points.OrderBy(p => p.x).ThenBy(p => p.y).First();
            hull.Add(current);
            used.Add(current);

            int runCount = 0;
            const int maxRunCount = 500;

            while (true)
            {
                // 获取最近的 k 个邻居，同时过滤掉已经使用的点
                var nearest = FindNearestNeighbors(points, current, k, used);

                // 如果没有找到候选点，则结束循环
                if (nearest == null || nearest.Count == 0)
                {
                    Debug.LogWarning("No candidate points found. Ending hull computation.");
                    break;
                }

                // 选择候选点中角度最小的一个
                var next = nearest.OrderBy(p => AngleBetween(current, p)).FirstOrDefault();

                // 如果返回默认值或者找回起始点，则结束循环
                if (next.Equals(default(Vector3)) || next.Equals(hull[0]))
                    break;

                // 如果该点已经存在于 hull 中（避免重复），也退出
                if (hull.Contains(next))
                {
                    Debug.LogWarning("Repeated point encountered. Ending hull computation.");
                    break;
                }

                hull.Add(next);
                used.Add(next);
                current = next;

                runCount++;
                if (runCount >= maxRunCount)
                {
                    Debug.LogWarning("Max run count reached. Ending hull computation.");
                    break;
                }
            }

            return hull;
        }

        private static List<Vector3> FindNearestNeighbors(List<Vector3> points, Vector3 point, int k, HashSet<Vector3> used)
        {
            return points
                .Where(p => !used.Contains(p))
                .OrderBy(p => Vector3.Distance(point, p))
                .Take(k)
                .ToList();
        }

        private static float AngleBetween(Vector3 from, Vector3 to)
        {
            return Mathf.Atan2(to.y - from.y, to.x - from.x);
        }
        #endregion
        #region FloodFill/DFS分组算法
        public static List<List<Vector3>> ExtractRegions(List<Vector3> points, float threshold = 1.0f)
        {
            var unvisited = new HashSet<Vector3>(points);
            var regions = new List<List<Vector3>>();

            while (unvisited.Count > 0)
            {
                var currentRegion = new List<Vector3>();
                var stack = new Stack<Vector3>();

                var startPoint = GetAnyPoint(unvisited);
                stack.Push(startPoint);
                unvisited.Remove(startPoint);

                while (stack.Count > 0)
                {
                    var point = stack.Pop();
                    currentRegion.Add(point);

                    foreach (var neighbor in GetNeighbors(point, unvisited, threshold))
                    {
                        stack.Push(neighbor);
                        unvisited.Remove(neighbor);
                    }
                }

                regions.Add(currentRegion);
            }

            return regions;
        }

        private static Vector3 GetAnyPoint(HashSet<Vector3> points)
        {
            foreach (var point in points)
                return point;

            return Vector3.zero;
        }

        private static List<Vector3> GetNeighbors(Vector3 point, HashSet<Vector3> unvisited, float threshold)
        {
            var neighbors = new List<Vector3>();

            foreach (var other in unvisited)
            {
                if (Vector3.Distance(point, other) <= threshold)
                {
                    neighbors.Add(other);
                }
            }

            return neighbors;
        }
        #endregion


        public Material gridMat;
        private void Parse(/*Color targetColor , LineRenderer lr*/)
        {

            List<List<Vector3Int>> sortedContours = ExtractContours(Vector2Ints);
            if (sortedContours.Count >= Max)
            {
                return;
            }
            foreach (var region in sortedContours)
            {
                List<Vector3> temp = region.Select(v => (Vector3)v).ToList();
                temp = Simplify(temp, Area);
                //Catmull-Rom 无法消除锯齿边界
                //boundary = GetCatmullRomSpline(boundary, InsertPointCount);
                temp = GetClosedCatmullRomSpline(temp, InsertPointCount);

                LineRenderer LR1 = Instantiate(LR);
                LR1.startWidth = LineWidth;
                LR1.endWidth = LineWidth;
                LR1.positionCount = temp.Count;
                LR1.SetPositions(temp.ToArray());
                // 显示每个区域的点数量
                //Debug.Log($"Region Size: {region.Count}, Boundary Size: {boundary.Count}");
            }

            //Vector2Ints = GetColorEdges(ASD, targetColor, Thresold);

            //var regions = ExtractRegions(Vector2Ints , AreaMinDis);
            //if (regions.Count >= Max)
            //{
            //    return;
            //}
            //foreach (var region in regions)
            //{
            //    var boundary = ComputeConcaveHull(region);
            //    boundary = Simplify(boundary, Area);
            //    //Catmull-Rom 无法消除锯齿边界
            //    //boundary = GetCatmullRomSpline(boundary, InsertPointCount);
            //    boundary = GetClosedCatmullRomSpline(boundary, InsertPointCount);

            //    LineRenderer LR1 = Instantiate(LR);
            //    LR1.startWidth = LineWidth;
            //    LR1.endWidth = LineWidth;
            //    LR1.positionCount = boundary.Count;
            //    LR1.SetPositions(boundary.ToArray());
            //    // 显示每个区域的点数量
            //    //Debug.Log($"Region Size: {region.Count}, Boundary Size: {boundary.Count}");
            //}



            //Vector2Ints = Simplify(Vector2Ints, Area);
            //Catmull-Rom 无法消除锯齿边界
            //Vector2Ints = GetCatmullRomSpline(Vector2Ints, InsertPointCount);
            //Vector2Ints = GetClosedCatmullRomSpline(Vector2Ints, InsertPointCount);
            //LineRenderer LR1 = Instantiate(LR);
            //LR1.startWidth = LineWidth;
            //LR1.endWidth = LineWidth;
            //LR1.positionCount = Vector2Ints.Count;
            //LR1.SetPositions(Vector2Ints.ToArray());
            //// 提取多个区域
            //Debug.Log($"regions.Count {regions.Count}");
        }



    }
}
