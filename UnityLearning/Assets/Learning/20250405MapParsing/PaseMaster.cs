using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace TEN
{
	/// <summary>
	///项目 : TEN
	///日期：2025/4/5 14:27:01 
	///创建者：Michael Corleone
	///类用途：
	/// </summary>
	public class PaseMaster : MonoBehaviour
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
        public static List<UnityEngine.Vector3> GetColorEdges(Texture2D texture, Color targetColor, float colorTolerance = 0.2f)
        {
            int width = texture.width;
            int height = texture.height;

            int count = 0;
            // 创建颜色掩码
            bool[,] colorMask = new bool[width, height];
            List<UnityEngine.Vector3> edges = new List<UnityEngine.Vector3>();

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
                            edges.Add(new UnityEngine.Vector3(x , y));
                    }
                }
            }
            UnityEngine.Vector3 center = UnityEngine.Vector3.zero;
            foreach (var p in edges)
            {
                center += p;
            }
            center /= edges.Count;
            //center = Vector3.zero;
            //int diss = 0;
            //foreach (var item in collection)
            //{
            //    if (Vector3.Distance())
            //    {

            //    }
            //}
            edges = SortByPolarAngle(edges, center);
            return edges;

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
            return UnityEngine.Vector3.Dot(greenColor, new UnityEngine.Vector3(color.r , color.g , color.b));
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
            uvs[0] = pos + new UnityEngine.Vector2Int(-1,-1);
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
            if (points == null || points.Count < 3)
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

        //Area of the excluded triangle
        public float Area = 10f;
        public float Thresold = 0.1f;
        List<UnityEngine.Vector3> Vector2Ints;
        private void Awake()
        {
            //ASD = DuplicateTexture(ASD);
            _reslution = new UnityEngine.Vector2Int(ASD.width, ASD.height );
            Vector2Ints = GetColorEdges(ASD, new Color(148f/255f,255f/255f,111f/255f , 1), Thresold);

            Vector2Ints = Simplify(Vector2Ints , Area);
            //Catmull-Rom 无法消除锯齿边界
            //Vector2Ints = GetCatmullRomSpline(Vector2Ints , 3 );

            Vector2Ints = GetClosedCatmullRomSpline(Vector2Ints, 5);

            LR.positionCount = Vector2Ints.Count;
            
            LR.SetPositions(Vector2Ints.ToArray());
            //int i = 0;
            //foreach (var item in Vector2Ints)
            //{
            //    GameObject temp = new GameObject($"{i++}");
            //    temp.transform.position = item;
            //}
        }
    }
}
