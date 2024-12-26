using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;
using System.Linq;
using TEN.GLOBAL;
using TEN.GLOBAL.ENUM;

namespace TEN.LEARNING.DREAMTICKER
{
	/// <summary>
	///项目 : TEN
	///日期：2024/12/22 8:57:38 
	///创建者：Michael Corleone
	///类用途：
	/// </summary>
    [DisallowMultipleComponent]
	public class BlockManager : MonoBehaviour
	{
        public GameObject BeforeMirror;
        public GameObject InMirror;
        public GameObject BehindMirror;
        public MirrorPlane Mirror;
        public UnityEvent OnBlockGraphRebuilt = new UnityEvent();
        [NonSerialized]
        public HashSet<Vector3Int> AllBlocksInWorld = new HashSet<Vector3Int>();
        public static BlockManager Instance { get; private set; }
        private void Awake()
        {
            if (Instance)
            {
                Destroy(this);
                return;
            }
            RebuildBlockGraph();
            Instance = this;
        }
        private uint _disableInteract = 0;
        public void EnableInteract() => _disableInteract--;
        public void DisableInteract() => _disableInteract++;
        public bool CanInteract => _disableInteract == 0;
        public void RebuildBlockGraph()
        {
            AllBlocksInWorld.Clear();
            Block[] beforeMirrorBlocks = BeforeMirror.GetComponentsInChildren<Block>();
            Block[] inMirrorBlocks = InMirror.GetComponentsInChildren<Block>();
            Block[] behindMirrorBlocks = BehindMirror.GetComponentsInChildren<Block>();

            Matrix4x4 viewMat = Camera.main.worldToCameraMatrix;
            Vector2 axisX = viewMat.MultiplyVector(Vector3.right);
            Vector2 axisZ = viewMat.MultiplyVector(Vector3.forward);
            Vector2 origin = viewMat.MultiplyPoint(beforeMirrorBlocks[0].UpperCenter);
            Debug.Log($"Origin Block {beforeMirrorBlocks[0].transform.position.RoundToInt()}", beforeMirrorBlocks[0]);

            /*
             |a b|   的逆矩阵为      |d -b| * 1/det
             |c d|                   |-c a|
             应该是个公式
             */
            float det = axisX.x * axisZ.y - axisX.y * axisZ.x;
            Matrix4x4 transientMat = new Matrix4x4(
                new Vector4(axisZ.y, -axisX.y) / det,
                new Vector4(-axisZ.x, axisX.x) / det,
                Vector4.zero,
                Vector4.zero
                );

            Dictionary<Vector2Int, BlockGroup> bMap = new Dictionary<Vector2Int, BlockGroup>();

            foreach (var mapBlock in beforeMirrorBlocks.Concat(inMirrorBlocks).Concat(behindMirrorBlocks))
            {
                //坐标转换
                Vector3 vp = viewMat.MultiplyPoint(mapBlock.UpperCenter);
                Vector2 p = transientMat * ((Vector2)vp - origin);

                int x = UnityEngine.Mathf.RoundToInt(p.x);
                int y = UnityEngine.Mathf.RoundToInt(p.y);
                Vector2Int key = new Vector2Int(x, y);

                AllBlocksInWorld.Add(mapBlock.transform.position.RoundToInt());

                if (!bMap.TryGetValue(key , out BlockGroup group))
                {
                    group = new BlockGroup();
                    bMap[key] = group;
                }

                mapBlock.ProjectedXY = key;
                mapBlock.ViewSpaceUpperCenterZ = vp.z;
                group.AddBlock(mapBlock);
            }

            Vector2 planeMaxPoint = viewMat.MultiplyPoint(Mirror.PlaneMaxPosition);
            planeMaxPoint = transientMat * (planeMaxPoint - origin);
            int lineVMaxB = Mathf.RoundToInt(planeMaxPoint.x + planeMaxPoint.y);
            float lineHMaxB = Mathf.RoundToInt(planeMaxPoint.y + 0.5f) - 0.5f;
            Debug.Log($"VMax: {lineVMaxB}");
            Debug.Log($"HMax: {lineHMaxB}");

            CullBlocksByMirror(lineVMaxB, lineHMaxB: lineHMaxB, beforeMirrorBlocks, BlockCategory.BeforeMirror);
            CullBlocksByMirror(lineVMaxB, lineHMaxB: lineHMaxB, inMirrorBlocks, BlockCategory.InMirror);
            CullBlocksByMirror(lineVMaxB, lineHMaxB: lineHMaxB, behindMirrorBlocks, BlockCategory.BehindMirror);
            CullBlocksByViewSpaceZ(bMap);

            OnBlockGraphRebuilt.Invoke();
        }

        private void CullBlocksByMirror(int lineVMaxB, float lineHMaxB, Block[] blocks, BlockCategory cat)
        {
            foreach (var mapBlock in blocks)
            {
                mapBlock.ProjectedShapes = BlockProjectedShapes.None;

                if (cat == BlockCategory.BeforeMirror)
                {
                    mapBlock.ProjectedShapes = BlockProjectedShapes.FullHexagon;
                }
                else if (cat == BlockCategory.InMirror)
                {
                    if (IsInMirror(mapBlock.ProjectedXY + new Vector2(1 / 6f, 1 / 6f), lineVMaxB, lineHMaxB))
                    {
                        mapBlock.ProjectedShapes |= BlockProjectedShapes.MiddleUpperTriangle;
                    }

                    if (IsInMirror(mapBlock.ProjectedXY - new Vector2(1 / 6f, 1 / 6f), lineVMaxB, lineHMaxB))
                    {
                        mapBlock.ProjectedShapes |= BlockProjectedShapes.LeftUpperTriangle;
                    }

                    if (IsInMirror(mapBlock.ProjectedXY - new Vector2(1 / 6f - 1, 1 / 6f), lineVMaxB, lineHMaxB))
                    {
                        mapBlock.ProjectedShapes |= BlockProjectedShapes.RightUpperTriangle;
                    }

                    if (IsInMirror(mapBlock.ProjectedXY + new Vector2(1 / 6f, 1 / 6f - 1), lineVMaxB, lineHMaxB))
                    {
                        mapBlock.ProjectedShapes |= BlockProjectedShapes.LeftLowerTriangle;
                    }

                    if (IsInMirror(mapBlock.ProjectedXY + new Vector2(1 / 6f + 1, 1 / 6f - 1), lineVMaxB, lineHMaxB))
                    {
                        mapBlock.ProjectedShapes |= BlockProjectedShapes.RightLowerTriangle;
                    }

                    if (IsInMirror(mapBlock.ProjectedXY - new Vector2(1 / 6f - 1, 1 / 6f + 1), lineVMaxB, lineHMaxB))
                    {
                        mapBlock.ProjectedShapes |= BlockProjectedShapes.MiddleLowerTriangle;
                    }
                }
                else
                {
                    if (!IsInMirror(mapBlock.ProjectedXY + new Vector2(1 / 6f, 1 / 6f), lineVMaxB, lineHMaxB))
                    {
                        mapBlock.ProjectedShapes |= BlockProjectedShapes.MiddleUpperTriangle;
                    }

                    if (!IsInMirror(mapBlock.ProjectedXY - new Vector2(1 / 6f, 1 / 6f), lineVMaxB, lineHMaxB))
                    {
                        mapBlock.ProjectedShapes |= BlockProjectedShapes.LeftUpperTriangle;
                    }

                    if (!IsInMirror(mapBlock.ProjectedXY - new Vector2(1 / 6f - 1, 1 / 6f), lineVMaxB, lineHMaxB))
                    {
                        mapBlock.ProjectedShapes |= BlockProjectedShapes.RightUpperTriangle;
                    }

                    if (!IsInMirror(mapBlock.ProjectedXY + new Vector2(1 / 6f, 1 / 6f - 1), lineVMaxB, lineHMaxB))
                    {
                        mapBlock.ProjectedShapes |= BlockProjectedShapes.LeftLowerTriangle;
                    }

                    if (!IsInMirror(mapBlock.ProjectedXY + new Vector2(1 / 6f + 1, 1 / 6f - 1), lineVMaxB, lineHMaxB))
                    {
                        mapBlock.ProjectedShapes |= BlockProjectedShapes.RightLowerTriangle;
                    }

                    if (!IsInMirror(mapBlock.ProjectedXY - new Vector2(1 / 6f - 1, 1 / 6f + 1), lineVMaxB, lineHMaxB))
                    {
                        mapBlock.ProjectedShapes |= BlockProjectedShapes.MiddleLowerTriangle;
                    }
                }
            }
        }
        private bool IsInMirror(Vector2 point, int lineVMaxB, float lineHMaxB)
        {
            float lineHMinB = lineHMaxB - Mirror.Height;
            if (point.x < lineHMinB || point.y > lineHMaxB)
            {
                return false;
            }

            int lineVMinB = lineVMaxB - Mirror.Width;
            float pos1 = -point.x - point.y + lineVMinB;
            float pos2 = -point.x - point.y + lineVMaxB;
            return pos1 * pos2 < 0;
        }
        private void CullBlocksByViewSpaceZ(Dictionary<Vector2Int, BlockGroup> bMap)
        {

        }

        private static void SetZMap(Dictionary<Vector2Int, float> zMap, Vector2Int key, float z)
        {
            if (!zMap.TryGetValue(key, out float depth))
            {
                zMap[key] = z;
            }
            else
            {
                zMap[key] = Mathf.Max(depth, z);
            }
        }

        private void ConnectBlocks(Dictionary<Vector2Int, BlockGroup> bMap)
        {

        }
    }
}
