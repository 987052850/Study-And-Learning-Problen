using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UIElements;

namespace TEN
{
    //排序相关
    public partial class PaseMaster 
    {
        public static int Radius = 3;
        // 定义 8 邻域（顺时针顺序）
        private static readonly Vector3Int[] Neighbors = new Vector3Int[]
        {
        new Vector3Int(0, 1 , 0),    // 上
        //new Vector3Int(1, 1 , 0),    // 右上
        new Vector3Int(1, 0 , 0),    // 右
        //new Vector3Int(1, -1 , 0),   // 右下
        new Vector3Int(0, -1 , 0),   // 下
        //new Vector3Int(-1, -1 , 0),  // 左下
        new Vector3Int(-1, 0 , 0),   // 左
        //new Vector3Int(-1, 1 , 0)    // 左上
        };


        public static List<List<Vector3>> SplitByDistance(List<Vector3> points, float maxGap = 2.0f, bool autoClose = true)
        {
            List<List<Vector3>> result = new List<List<Vector3>>();

            if (points == null || points.Count == 0)
                return result;

            List<Vector3> current = new List<Vector3>();
            current.Add(points[0]);

            for (int i = 1; i < points.Count; i++)
            {
                float dist = Vector3.Distance(points[i], points[i - 1]);

                if (dist > maxGap && current.Count > 2)
                {
                    if (autoClose && Vector3.Distance(current[0], current[^1]) <= maxGap)
                        current.Add(current[0]); // 自动闭合

                    result.Add(new List<Vector3>(current));
                    current.Clear();
                }

                current.Add(points[i]);
            }

            if (current.Count > 2)
            {
                if (autoClose && Vector3.Distance(current[0], current[^1]) <= maxGap)
                    current.Add(current[0]);

                result.Add(current);
            }

            return result;
        }


        public static List<List<Vector3>> TraceContours(List<Vector3> points, float tolerance = 0.1f)
        {
            HashSet<Vector3> unvisited = new HashSet<Vector3>(points);
            List<List<Vector3>> contours = new List<List<Vector3>>();

            while (unvisited.Count > 0)
            {
                Vector3 start = unvisited.First();
                List<Vector3> contour = new List<Vector3>();
                Stack<Vector3> mulNeigNode = new Stack<Vector3>();

                contour.Add(start);
                unvisited.Remove(start);

                Vector3 current = start;
                Vector3? nextPoint = null;

                while (true)
                {
                    bool foundNeighbor = false;

                    foreach (var offset in Neighbors)
                    {
                        Vector3 candidate = current + new Vector3(offset.x, offset.y, 0);

                        // 用 tolerance 判断“近似相等”，以适应浮点精度问题
                        Vector3 match = unvisited.FirstOrDefault(p => Vector3.Distance(p, candidate) <= tolerance);
                        if (match != default)
                        {
                            if (foundNeighbor)
                            {
                                mulNeigNode.Push(current);
                                break;
                            }

                            nextPoint = match;
                            foundNeighbor = true;
                        }
                    }

                    if (!foundNeighbor)
                    {
                        // 尝试回溯处理多邻点分支
                        bool backtracked = false;
                        while (mulNeigNode.Count > 0)
                        {
                            Vector3 temp = mulNeigNode.Pop();

                            foreach (var offset in Neighbors)
                            {
                                Vector3 candidate = temp + new Vector3(offset.x, offset.y, 0);
                                Vector3 match = unvisited.FirstOrDefault(p => Vector3.Distance(p, candidate) <= tolerance);
                                if (match != default)
                                {
                                    nextPoint = temp;
                                    backtracked = true;
                                    break;
                                }
                            }

                            if (backtracked)
                                break;
                        }

                        if (!backtracked)
                            break;
                    }

                    if (nextPoint.HasValue && Vector3.Distance(nextPoint.Value, start) <= tolerance && contour.Count >= 3)
                    {
                        contour.Add(start); // 闭合路径
                        break;
                    }
                    else if (nextPoint.HasValue)
                    {
                        contour.Add(nextPoint.Value);
                        unvisited.Remove(nextPoint.Value);
                        current = nextPoint.Value;
                    }
                    else
                    {
                        break;
                    }
                }

                if (contour.Count >= 3)
                {
                    contours.Add(contour);
                }
            }

            return contours;
        }

        //public static Dictionary<Vector3Int, HashSet<Vector3Int>> BidirectionalGraph = new Dictionary<Vector3Int, HashSet<Vector3Int>>();
        //public IEnumerator GenertorBidirectionalGraph(int width, int height)
        //{
        //    BidirectionalGraph.Clear();
        //    for (int x = 0; x < width; x++)
        //    {
        //        for (int y = 0; y < height; y++)
        //        {
        //            Vector3Int pos = new Vector3Int(x, y, 0);
        //            BidirectionalGraph.Add(pos, new HashSet<Vector3Int>());
        //            for (int i = 0; i < 8; i++)
        //            {
        //                Vector3Int neighbor = pos + Neighbors[i];
        //                if (!CrossBorderDetection(neighbor.x, neighbor.y, width, height))
        //                {
        //                    BidirectionalGraph[pos].Add(neighbor);
        //                }
        //            }
        //        }
        //        yield return null;
        //    }
        //    Debug.Log("构图完毕");
        //}
        //true 代表在范围内
        public static bool Range(int x , int y , int width , int height)
        {
            return (x >= 0 && y >= 0 && x < width && y < height);
        }
        public static Queue<Vector3Int> GetNeigbors(Vector3Int center , int width , int height )
        {
            Queue<Vector3Int> outValue = new Queue<Vector3Int>();
            for (int i = 0; i < 8; i++)
            {
                Vector3Int neighbor = center + Neighbors[i];
                if (Range(neighbor.x, neighbor.y, width, height))
                {
                    outValue.Enqueue(neighbor);
                }
            }
            return outValue;
        }


        /// <summary>
        /// 从无序点集（像素坐标）中提取所有封闭区域的边界，并按顺序排列边界点。
        /// </summary>
        /// <param name="points">区域内的像素点集合</param>
        /// <returns>每个封闭区域的边界点序列</returns>
        public static List<List<Vector3Int>> ExtractContours(List<Vector3Int> lpoints)
        {
            HashSet<Vector3Int> points = new HashSet<Vector3Int>(lpoints);
            // 复制一份用于处理，避免修改原始数据
            HashSet<Vector3Int> unvisited = new HashSet<Vector3Int>(points);
            List<List<Vector3Int>> contours = new List<List<Vector3Int>>();

            while (unvisited.Count > 0)
            {
                // 随机取出一个点作为当前区域的起点
                Vector3Int start = unvisited.First();
                List<Vector3Int> contour = TraceContour(ref unvisited, start);
                if (contour != null && contour.Count > 0)
                {
                    contours.Add(contour);
                }
            }

            return contours;
        }

        private static void PushAll(ref Stack<Vector3Int> sv , HashSet<Vector3Int> neigbor, HashSet<Vector3Int> readed)
        {
            foreach (var item in neigbor)
            {
                if (!readed.Contains(item))
                {
                    sv.Push(item);
                    readed.Add(item);
                }
            }
        }

        private static Vector3Int BFS_FindNearest(int maxFindCount , Vector3Int center ,ref HashSet<Vector3Int> unvisitor)
        {
            HashSet<Vector3Int> temp = new HashSet<Vector3Int>(unvisitor);
            Queue<Vector3Int> visited = GetNeigbors(center , _reslution.x , _reslution.y );
            //StringBuilder sb = new StringBuilder();
            //foreach (var item in visited)
            //{
            //    sb.Append(item);
            //    sb.Append("    ");
            //}
            //Debug.Log(sb);
            int timer = 0;
            while (visited.Count > 0 && timer <= maxFindCount)
            {
                center = visited.Dequeue();
                if (unvisitor.Contains(center))
                {

                    return center;
                }
                visited = new Queue<Vector3Int>(visited.Concat(GetNeigbors(center, _reslution.x, _reslution.y)));
                timer++;
            }


            //if (BidirectionalGraph.ContainsKey(center))
            //{
            //    int now = 0;
            //    Stack<Vector3Int> sn = new Stack<Vector3Int>();
            //    HashSet<Vector3Int> hv = new HashSet<Vector3Int>();
            //    PushAll(ref sn , BidirectionalGraph[center] , hv);
            //    hv.Add(center);
            //    Vector3Int target = Vector3Int.zero;
            //    while (sn.Count > 0)
            //    {
            //        target = sn.Pop();
            //        if (source.Contains(target))
            //        {
            //            return target;
            //        }
            //        if (BidirectionalGraph.ContainsKey(target))
            //        {
            //            PushAll(ref sn, BidirectionalGraph[target], hv);
            //        }
            //        now++;
            //        if (now >= maxFindCount)
            //        {
            //            break;
            //        }
            //    }
            //}
            return Vector3Int.zero;
        }

        /// <summary>
        /// 使用邻域追踪算法从起点追踪出一个封闭区域的边界点序列。
        ///  .
        /// ......    当像素点如左图所示意
        ///  .
        /// </summary>
        /// <param name="unvisited">未处理的点集合，会在追踪过程中移除已访问点</param>
        /// <param name="start">追踪起点</param>
        /// <param name="source">所有区域内的点集合，用于判断候选点是否属于区域</param>
        /// <returns>
        /// 边界点的顺序排列（起点会在末尾重复一次以表示闭合），
        /// 如果追踪失败（例如无法形成闭合区域），则返回 null
        /// </returns>
        private static List<Vector3Int> TraceContour(ref HashSet<Vector3Int> unvisited, Vector3Int start)
        {
            // 用于保存追踪得到的轮廓点序列
            List<Vector3Int> contour = new List<Vector3Int>();

            // 添加起点到轮廓，并将其从未访问集合中移除
            contour.Add(start);
            unvisited.Remove(start);

            Stack<Vector3Int> mulNeigNode = new Stack<Vector3Int>();

            // 当前追踪位置
            Vector3Int current = start;
            int step = 0;
            bool ddd = false;

            // 开始追踪过程
            while (true)
            {
                bool foundNeighbor = false;
                Vector3Int nextPoint = new Vector3Int();
                // 按照顺时针方向，从 searchDir 开始扫描所有邻域位置
                for (int i = 0; i < Neighbors.Length; i++)
                {
                    int idx = (i % Neighbors.Length);
                    Vector3Int candidate = current + Neighbors[idx];

                    // 检查候选点是否属于目标区域
                    if (unvisited.Contains(candidate) && Range(candidate.x, candidate.y, _reslution.x, _reslution.y))
                    {
                        //Debug.Log($"找到啦 {candidate} , {Range(candidate.x, candidate.y, _reslution.x, _reslution.y)} , {unvisited.Contains(candidate)} ， {idx}");
                        if (foundNeighbor)
                        {
                            mulNeigNode.Push(current);
                            break;
                        }
                        nextPoint = candidate;
                        // 更新下一次搜索方向：从当前方向的前一位开始搜索
                        foundNeighbor = true;
                    }
                }
                // 如果当前点没有找到任何有效的邻域点，则追踪失败，返回 null
                if (!foundNeighbor)
                {
                    if (mulNeigNode.Count >  0)
                    {
                        bool found = false;
                        //nextPoint = mulNeigNode.Pop();
                        while (mulNeigNode.Count > 0)
                        {
                            Vector3Int temp = mulNeigNode.Pop();
                            //判断是否还存在邻居
                            for (int i = 0; i < Neighbors.Length; i++)
                            {
                                int idx = (i % Neighbors.Length);
                                Vector3Int candidate = temp + Neighbors[idx];

                                // 检查候选点是否属于目标区域
                                if (unvisited.Contains(candidate) && Range(candidate.x, candidate.y, _reslution.x, _reslution.y))
                                {
                                    nextPoint = temp;
                                    found = true;
                                    break;
                                }
                            }
                            if (!found)
                            {
                                //Debug.Log($"{temp} 邻居全噶了");
                            }
                            else { break; }
                        }
                        if (!found)
                        {
                            return contour;
                        }
                    }
                    else
                        return null;
                }
                // 如果找到了一个邻域点，并且该点就是起点（且至少有 3 个点），说明已经闭合
                if (nextPoint == start && contour.Count >= 3)
                {
                    contour.Add(start); // 添加起点作为闭合标志
                    break;
                }
                else
                {
                    if (nextPoint.Equals(Vector3Int.zero))
                    {
                        return contour;
                    }
                    // 添加这个点到轮廓，并从未访问集合中移除
                    contour.Add(nextPoint);
                    unvisited.Remove(nextPoint);
                    current = nextPoint;
                }
                //step++;
                //if (step >= 2 && !ddd)
                //{
                //    unvisited.Add(start);
                //    ddd = true;
                //}
            }
            return contour;
        }

        private static List<Vector3Int> TraceContour(ref List<Vector3Int> unvisited, Vector3Int start)
        {
            // 用于保存追踪得到的轮廓点序列
            List<Vector3Int> contour = new List<Vector3Int>();

            // 添加起点到轮廓，并将其从未访问集合中移除
            contour.Add(start);
            unvisited.Remove(start);

            Stack<Vector3Int> mulNeigNode = new Stack<Vector3Int>();

            // 当前追踪位置
            Vector3Int current = start;
            int step = 0;
            bool ddd = false;

            // 开始追踪过程
            while (true)
            {
                bool foundNeighbor = false;
                Vector3Int nextPoint = new Vector3Int();
                // 按照顺时针方向，从 searchDir 开始扫描所有邻域位置
                for (int i = 0; i < Neighbors.Length; i++)
                {
                    int idx = (i % Neighbors.Length);
                    Vector3Int candidate = current + Neighbors[idx];

                    // 检查候选点是否属于目标区域
                    if (unvisited.Contains(candidate) && Range(candidate.x, candidate.y, _reslution.x, _reslution.y))
                    {
                        //Debug.Log($"找到啦 {candidate} , {Range(candidate.x, candidate.y, _reslution.x, _reslution.y)} , {unvisited.Contains(candidate)} ， {idx}");
                        if (foundNeighbor)
                        {
                            mulNeigNode.Push(current);
                            break;
                        }
                        nextPoint = candidate;
                        // 更新下一次搜索方向：从当前方向的前一位开始搜索
                        foundNeighbor = true;
                    }
                }
                // 如果当前点没有找到任何有效的邻域点，则追踪失败，返回 null
                if (!foundNeighbor)
                {
                    if (mulNeigNode.Count > 0)
                    {
                        bool found = false;
                        //nextPoint = mulNeigNode.Pop();
                        while (mulNeigNode.Count > 0)
                        {
                            Vector3Int temp = mulNeigNode.Pop();
                            //判断是否还存在邻居
                            for (int i = 0; i < Neighbors.Length; i++)
                            {
                                int idx = (i % Neighbors.Length);
                                Vector3Int candidate = temp + Neighbors[idx];

                                // 检查候选点是否属于目标区域
                                if (unvisited.Contains(candidate) && Range(candidate.x, candidate.y, _reslution.x, _reslution.y))
                                {
                                    nextPoint = temp;
                                    found = true;
                                    break;
                                }
                            }
                            if (!found)
                            {
                                //Debug.Log($"{temp} 邻居全噶了");
                            }
                            else { break; }
                        }
                        if (!found)
                        {
                            return contour;
                        }
                    }
                    else
                        return null;
                }
                // 如果找到了一个邻域点，并且该点就是起点（且至少有 3 个点），说明已经闭合
                if (nextPoint == start && contour.Count >= 3)
                {
                    contour.Add(start); // 添加起点作为闭合标志
                    break;
                }
                else
                {
                    if (nextPoint.Equals(Vector3Int.zero))
                    {
                        return contour;
                    }
                    // 添加这个点到轮廓，并从未访问集合中移除
                    contour.Add(nextPoint);
                    unvisited.Remove(nextPoint);
                    current = nextPoint;
                }
                //step++;
                //if (step >= 2 && !ddd)
                //{
                //    unvisited.Add(start);
                //    ddd = true;
                //}
            }
            return contour;
        }


        /// <summary>
        /// 使用邻域追踪算法从起点追踪出一个封闭区域的边界点序列。
        /// </summary>
        /// <param name="unvisited">未处理的点集合，会在追踪过程中移除已访问点</param>
        /// <param name="start">追踪起点</param>
        /// <returns>边界点的顺序排列（起点会在末尾重复一次以表示闭合），如果追踪失败则返回 null</returns>
        //private static List<Vector3Int> TraceContour(ref HashSet<Vector3Int> unvisited, Vector3Int start, HashSet<Vector3Int> source)
        //{
        //    List<Vector3Int> contour = new List<Vector3Int>();
        //    contour.Add(start);

        //    Vector3Int current = start;
        //    int maxCount = 1000;
        //    int iiii = 0;
        //    while (true)
        //    {
        //        unvisited.Remove(current);

        //        current = BFS_FindNearest((int)Mathf.Pow(1 + (Radius * 2), 2) , current ,ref unvisited );
        //        if (current.Equals(Vector3Int.zero))
        //        {
        //            contour.Add(start);
        //            //Radius中未找到未访问过的相邻对象
        //            break;
        //        }
        //        contour.Add(current);
        //        iiii++;
        //        if (iiii >= maxCount)
        //        {
        //            Debug.Log("GGGGGGGGGGGGGGGGGG");
        //            break;
        //        }
        //    }



        //    //// 初始搜索方向（可以设为 0，即从 Neighbors[0] 开始搜索）
        //    //int searchDir = 0;

        //    //// 追踪，直到回到起点（形成闭合环）
        //    //while (true)
        //    //{
        //    //    bool foundNeighbor = false;
        //    //    Vector3Int nextPoint = Vector3Int.zero;
        //    //    int nextSearchDir = 0;



        //    //    // 从当前设定的搜索方向开始，顺时针依次检查所有邻域
        //    //    for (int i = 0; i < Neighbors.Length; i++)
        //    //    {
        //    //        int idx = (searchDir + i) % Neighbors.Length;
        //    //        Vector3Int candidate = current + Neighbors[idx];
        //    //        if (pointsContain(candidate, unvisited))
        //    //        {
        //    //            Debug.Log("存在邻居");
        //    //            nextPoint = candidate;
        //    //            // 更新下一次搜索方向：通常从该方向的前一位开始（保证追踪连续性）
        //    //            nextSearchDir = (idx + Neighbors.Length - 1) % Neighbors.Length;
        //    //            foundNeighbor = true;
        //    //            break;
        //    //        }
        //    //    }

        //    //    if (!foundNeighbor)
        //    //    {
        //    //        // 如果在当前点找不到任何邻居，则可能区域不封闭，退出追踪
        //    //        break;
        //    //    }

        //    //    // 如果找到了一个点，检查是否回到起点（且轮廓上至少有 3 个点）
        //    //    if (nextPoint == start && contour.Count >= 3)
        //    //    {
        //    //        Debug.Log("闭合标记");
        //    //        contour.Add(start); // 添加起点作为闭合标记
        //    //        break;
        //    //    }
        //    //    else
        //    //    {
        //    //        contour.Add(nextPoint);
        //    //        // 移除该点（避免重复追踪）
        //    //        unvisited.Remove(nextPoint);
        //    //        // 更新当前点及搜索方向
        //    //        current = nextPoint;
        //    //        searchDir = nextSearchDir;
        //    //    }
        //    //}

        //    //// 这里可以检查是否形成了闭合区域（例如最后一个点与起点是否相同）
        //    //if (contour.First() != contour.Last())
        //    //{
        //    //    Debug.Log("未闭合标记");
        //    //    // 如果未形成闭合区域，可以选择返回 null 或者将其丢弃
        //    //    return null;
        //    //}

        //    //Debug.Log($"contour{contour.Count}");
        //    return contour;
        //}

        /// <summary>
        /// 判断 candidate 是否存在于集合中。
        /// 这里抽象成一个方法，便于后续扩展（例如检查边界范围等）。
        /// </summary>
        private static bool pointsContain(Vector3Int candidate, HashSet<Vector3Int> set)
        {
            return set.Contains(candidate);
        }

    }
}
