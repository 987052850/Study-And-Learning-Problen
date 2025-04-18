using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using LibTessDotNet;

namespace TEN
{
    public partial class PaseMaster
    {


        bool SegmentsAreConnected(List<Vector3> segA, List<Vector3> segB, float threshold)
        {
            var endpointsA = new Vector3[] { segA[0], segA[segA.Count - 1] };
            var endpointsB = new Vector3[] { segB[0], segB[segB.Count - 1] };

            foreach (var a in endpointsA)
                foreach (var b in endpointsB)
                    if (Vector3.Distance(a, b) <= threshold)
                        return true;

            return false;
        }


        Mesh GenerateMeshForPolygonXY(List<Vector3> polygon, float meshHeight)
        {
            if (ComputeSignedAreaXY(polygon) < 0)
                polygon.Reverse();

            Tess tess = new Tess();

            tess.AddContour(polygon.Select(v => new ContourVertex
            {
                Position = new Vec3 { X = v.x, Y = v.y, Z = meshHeight }
            }).ToArray());

            tess.Tessellate(WindingRule.EvenOdd, ElementType.Polygons, 3);

            Mesh mesh = new Mesh
            {
                vertices = tess.Vertices.Select(v => new Vector3(v.Position.X, v.Position.Y, meshHeight)).ToArray(),
                triangles = tess.Elements
            };

            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            return mesh;
        }

        // XY平面面积计算
        float ComputeSignedAreaXY(List<Vector3> polygon)
        {
            float area = 0;
            int j = polygon.Count - 1;

            for (int i = 0; i < polygon.Count; i++)
            {
                area += (polygon[j].x + polygon[i].x) * (polygon[j].y - polygon[i].y);
                j = i;
            }

            return area * 0.5f;
        }

        Mesh GenerateMeshWithHoles(List<List<Vector3>> polygons)
        {
            Tess tess = new Tess();
            var areas = CalculatePolygonsAreas(polygons);
            List<Vector3> outer = FindOuterContour(polygons);
            if (outer == null)
            {
                Debug.Log("Mesh is null");
                return new Mesh();
            }

            // 外轮廓必须是顺时针
            if (ComputeSignedArea(outer) < 0)
                outer.Reverse();

            tess.AddContour(outer.Select(v => new ContourVertex
            {
                Position = new Vec3 { X = v.x, Y = v.y, Z = 0 },
                //Position = new Vec3 { X = v.x, Y = MeshHeight, Z = v.z },
            }).ToArray());

            // 孔洞轮廓（逆时针）
            foreach (var (poly, _) in areas)
            {
                if (poly == outer)
                    continue;

                if (IsPolygonInsideAnother(poly, outer))
                {
                    if (ComputeSignedArea(poly) > 0)
                        poly.Reverse();

                    tess.AddContour(poly.Select(v => new ContourVertex
                    {
                        Position = new Vec3 { X = v.x, Y = v.y, Z = 0 }
                        //Position = new Vec3 { X = v.x, Y = MeshHeight, Z = v.z }
                    }).ToArray());
                }
            }

            tess.Tessellate(WindingRule.EvenOdd, ElementType.Polygons, 3);

            Mesh mesh = new Mesh
            {
                vertices = tess.Vertices.Select(v => new Vector3(v.Position.X, v.Position.Y, 0)).ToArray(),
                triangles = tess.Elements
            };
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            return mesh;
        }


        // 计算多个多边形的面积，并返回每个多边形及其面积的元组列表
        List<(List<Vector3> polygon, float area)> CalculatePolygonsAreas(List<List<Vector3>> polygons)
        {
            var results = new List<(List<Vector3>, float)>();

            foreach (var polygon in polygons)
            {
                float area = ComputeSignedArea1(polygon);
                results.Add((polygon, area));
            }

            return results;
        }

        // 多边形的带符号面积计算函数（XZ平面）
        float ComputeSignedArea1(List<Vector3> polygon)
        {
            float area = 0;
            int j = polygon.Count - 1;

            for (int i = 0; i < polygon.Count; i++)
            {
                area += (polygon[j].x + polygon[i].x) * (polygon[j].z - polygon[i].z);
                j = i;
            }

            return area * 0.5f;
        }

        List<Vector3> FindOuterContour(List<List<Vector3>> polygons)
        {
            float maxArea = float.MinValue;
            List<Vector3> outer = null;
            foreach (var poly in polygons)
            {
                float area = Mathf.Abs(ComputeSignedArea(poly));
                if (area > maxArea)
                {
                    maxArea = area;
                    outer = poly;
                }
            }
            return outer;
        }

        bool IsPolygonInsideAnother(List<Vector3> inner, List<Vector3> outer)
        {
            // 使用多边形内一点判断是否在外轮廓内
            Vector3 testPoint = inner[0];
            return IsPointInPolygon(testPoint, outer);
        }

        bool IsPointInPolygon(Vector3 p, List<Vector3> polygon)
        {
            int count = polygon.Count;
            bool inside = false;
            for (int i = 0, j = count - 1; i < count; j = i++)
            {
                if (((polygon[i].z > p.z) != (polygon[j].z > p.z)) &&
                    (p.x < (polygon[j].x - polygon[i].x) * (p.z - polygon[i].z) / (polygon[j].z - polygon[i].z) + polygon[i].x))
                    inside = !inside;
            }
            return inside;
        }



        Mesh GenerateMeshFromPolygons(List<List<Vector3>> polygons)
        {
            Tess tess = new Tess();

            foreach (var polygon in polygons)
            {
                if (ComputeSignedArea(polygon) < 0)
                    polygon.Reverse();

                ContourVertex[] contour = polygon.Select(p => new ContourVertex
                {
                    Position = new Vec3 { X = p.x, Y = p.y, Z = 0 },
                    //Position = new Vec3 { X = p.x, Y = MeshHeight, Z = p.z },
                }).ToArray();

                tess.AddContour(contour, ContourOrientation.Original);
            }

            tess.Tessellate(WindingRule.EvenOdd, ElementType.Polygons, 3);

            Vector3[] meshVertices = tess.Vertices
                //.Select(v => new Vector3(v.Position.X, MeshHeight, v.Position.Z))
                .Select(v => new Vector3(v.Position.X, v.Position.Y, v.Position.Z))
                .ToArray();

            Mesh mesh = new Mesh
            {
                vertices = meshVertices,
                triangles = tess.Elements
            };

            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            return mesh;
        }


        /// <summary>
        /// 利用 LibTessDotNet 根据闭合边界生成填充三角形，并生成 Unity 的 Mesh。
        /// </summary>
        Mesh GenerateMeshFromPolygon(List<Vector3> boundary)
        {

            if (ComputeSignedArea(boundary) < 0)
            {
                boundary.Reverse();
                //数组逆序
            }
            // 用 LibTessDotNet 进行三角剖分
            Tess tess = new Tess();

            // 将多边形边界转换为 ContourVertex 数组
            ContourVertex[] contour = new ContourVertex[boundary.Count];
            for (int i = 0; i < boundary.Count; i++)
            {
                // 这里假设多边形位于 XZ 平面，使用 x 和 z 分量，y 用于高度（这里直接赋值）
                Vector3 p = boundary[i];
                contour[i] = new ContourVertex()
                {
                    //Position = new Vec3 { X = p.x, Y = p.y, Z = 0 },
                    Position = new Vec3 { X = p.x, Y = p.y, Z = p.z },
                    Data = null
                };
            }

            // 添加轮廓，ContourOrientation.Original 表示使用原始给定的顶点顺序
            tess.AddContour(contour, ContourOrientation.Original);
            //tess.AddContour(contour, ContourOrientation.CounterClockwise);

            // 进行三角化，WindingRule 可根据需求选择（这里使用 EvenOdd 规则），目标生成多边形（三角形）
            tess.Tessellate(WindingRule.EvenOdd, ElementType.Polygons, 3);

            // 获取结果顶点和三角形索引
            var tessVertices = tess.Vertices;       // LibTessDotNet.Vec3[] 数组
            var tessIndices = tess.Elements;          // int[] 数组，每 3 个为一个三角形的顶点索引
            //Debug.Log($"tessVertices = {tessVertices.Length}");
            // 准备 Unity Mesh 数据
            Vector3[] meshVertices = new Vector3[tessVertices.Length];
            for (int i = 0; i < tessVertices.Length; i++)
            {
                // 将 LibTessDotNet 的 Vec3 转换到 Unity 的 Vector3
                meshVertices[i] = new Vector3(tessVertices[i].Position.X, tessVertices[i].Position.Y, tessVertices[i].Position.Z);
                //meshVertices[i] = new Vector3(tessVertices[i].Position.X, tessVertices[i].Position.Y, 0);
            }

            Mesh mesh = new Mesh();
            mesh.vertices = meshVertices;
            mesh.triangles = tessIndices;
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            return mesh;
        }

        public Mesh GenerateMeshFromBoundaries(List<Vector3> boundaries)
        {
            Mesh mesh = new Mesh();

            List<Vector3> allVertices = new List<Vector3>();
            List<int> allTriangles = new List<int>();

            // 遍历每一个闭合曲线
            // 三角化当前闭合曲线，返回的是相对于 poly 内部索引的三角形索引列表
            List<int> polyTriangles = TriangulatePolygon(boundaries);

            // 当前 poly 的顶点数
            int baseIndex = allVertices.Count;
            allVertices.AddRange(boundaries);

            // 调整索引，加入全局 triangles 列表中
            foreach (int index in polyTriangles)
            {
                allTriangles.Add(baseIndex + index);
            }
            mesh.SetVertices(allVertices);
            mesh.SetTriangles(allTriangles, 0);
            mesh.RecalculateNormals();
            mesh.RecalculateBounds(); // 强制更新包围盒
            //Debug.Log($"{mesh.triangles.Length}");
            return mesh;
        }

        /// <summary>
        /// 对一个闭合多边形（位于 XZ 平面）使用耳剪法进行三角剖分，返回三角形顶点索引（局部索引）
        /// </summary>
        List<int> TriangulatePolygon(List<Vector3> poly)
        {
            List<int> indices = new List<int>();
            int n = poly.Count;
            if (n < 3)
            {
                return indices;
            }

            // 将多边形投影为 2D 点集，假设取 x,z 分量
            List<Vector2> poly2D = new List<Vector2>();
            for (int i = 0; i < n; i++)
            {
                poly2D.Add(new Vector2(poly[i].x, poly[i].y));
            }

            // 检查顶点顺序，如果多边形的有向面积小于 0，则顺序为顺时针，需要反转顺序
            if (ComputeSignedArea(poly2D) < 0)
            {
                poly2D.Reverse();
                poly.Reverse();  // 根据需求决定是否反转原始的 poly
            }

            // 存储多边形当前的顶点索引（初始为 0~n-1）
            List<int> vertIndices = new List<int>();
            for (int i = 0; i < n; i++)
            {
                vertIndices.Add(i);
            }

            // 循环摘除耳朵，直到剩余3个顶点
            while (vertIndices.Count > 3)
            {
                bool earFound = false;
                // 遍历每个顶点，判断是否为耳朵
                for (int i = 0; i < vertIndices.Count; i++)
                {
                    int prevIndex = vertIndices[(i - 1 + vertIndices.Count) % vertIndices.Count];
                    int currIndex = vertIndices[i];
                    int nextIndex = vertIndices[(i + 1) % vertIndices.Count];

                    Vector2 a = poly2D[prevIndex];
                    Vector2 b = poly2D[currIndex];
                    Vector2 c = poly2D[nextIndex];

                    // 判断角是否为凸角（在逆时针顺序下，叉积应为正）
                    if (!IsConvex(a, b, c))
                        continue;

                    // 检查是否有其他点位于三角形内部
                    bool hasPointInside = false;
                    for (int j = 0; j < vertIndices.Count; j++)
                    {
                        int testIndex = vertIndices[j];
                        if (testIndex == prevIndex || testIndex == currIndex || testIndex == nextIndex)
                            continue;

                        Vector2 p = poly2D[testIndex];
                        if (PointInTriangle(p, a, b, c))
                        {
                            hasPointInside = true;
                            break;
                        }
                    }

                    if (hasPointInside)
                        continue;

                    // 如果满足条件，则认为该顶点为耳朵，生成一个三角形
                    indices.Add(prevIndex);
                    indices.Add(currIndex);
                    indices.Add(nextIndex);

                    // 将耳朵顶点移除
                    vertIndices.RemoveAt(i);
                    earFound = true;
                    break;
                }

                // 如果没有找到耳朵（可能多边形有问题）则退出循环，防止死循环
                if (!earFound)
                {
                    Debug.LogWarning("没有找到可摘除的耳朵，可能多边形数据有问题！");
                    break;
                }
            }

            // 剩下的3个顶点构成最后一个三角形
            if (vertIndices.Count == 3)
            {
                indices.Add(vertIndices[0]);
                indices.Add(vertIndices[1]);
                indices.Add(vertIndices[2]);
            }

            return indices;
        }

        /// <summary>
        /// 计算投影到 XZ 平面的多边形有向面积
        /// </summary>
        float ComputeSignedArea(List<Vector2> poly2D)
        {
            float area = 0;
            int count = poly2D.Count;
            for (int i = 0; i < count; i++)
            {
                Vector2 current = poly2D[i];
                Vector2 next = poly2D[(i + 1) % count];
                area += (current.x * next.y - next.x * current.y);
            }
            return area * 0.5f;
        }


        /// <summary>
        /// 判断三角形 abc 是否为凸角（在 2D 中，当多边形为逆时针顺序时，叉积>0 则为凸角）
        /// </summary>
        bool IsConvex(Vector2 a, Vector2 b, Vector2 c)
        {
            return Cross(a, b, c) > 0;
        }

        /// <summary>
        /// 计算向量 ab 与 ac 的叉积（注意：返回的是标量，其正负表明方向）
        /// </summary>
        float Cross(Vector2 a, Vector2 b, Vector2 c)
        {
            return (b.x - a.x) * (c.y - a.y) - (b.y - a.y) * (c.x - a.x);
        }

        /// <summary>
        /// 检查点 p 是否位于三角形 abc 内（采用重心坐标法）
        /// </summary>
        bool PointInTriangle(Vector2 p, Vector2 a, Vector2 b, Vector2 c)
        {
            // 计算向量
            Vector2 v0 = c - a;
            Vector2 v1 = b - a;
            Vector2 v2 = p - a;

            float dot00 = Vector2.Dot(v0, v0);
            float dot01 = Vector2.Dot(v0, v1);
            float dot02 = Vector2.Dot(v0, v2);
            float dot11 = Vector2.Dot(v1, v1);
            float dot12 = Vector2.Dot(v1, v2);

            // 计算重心系数
            float invDenom = 1 / (dot00 * dot11 - dot01 * dot01);
            float u = (dot11 * dot02 - dot01 * dot12) * invDenom;
            float v = (dot00 * dot12 - dot01 * dot02) * invDenom;

            // 如果 u, v 大于等于 0 并且 u+v 小于等于 1，则在三角形内
            return (u >= 0) && (v >= 0) && (u + v <= 1);
        }
    }
}

