using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TEN.LEARNING.DREAMTICKER
{
	/// <summary>
	///项目 : TEN
	///日期：2024/12/29 10:32:05 
	///创建者：Michael Corleone
	///类用途：
	/// </summary>
    [DisallowMultipleComponent]
	public class Player : MonoBehaviour
	{
        public Vector3 PositionOffset = new Vector3(0, 1.5f, 0);
        public float MoveTimePerBlock = 0.1f;
        public Trail PathDisplayTrail;
        public Block CurrentBlock;
        public GameObject GoalHintPrefab;
        public Block[] GoalBlocks;

        private bool _isMoving = false;
        private int _moveGoalIndex = 0;
        private readonly HashSet<Block> _moveVis = new HashSet<Block>();
        private readonly Queue<Block> _moveQueue = new Queue<Block>();
        private readonly Dictionary<Block, Block> _moveNext = new Dictionary<Block, Block>();

        private void Start()
        {
            SetPlayerPosition(CurrentBlock.transform.position);
            TryPlaceGoalHint();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                TryMove();
            }
        }

        public void TryMove()
        {
            Debug.Log("00001");
            if (_isMoving || _moveGoalIndex >= GoalBlocks.Length)
            {
                return;
            }

            _moveVis.Clear();
            _moveQueue.Clear();
            _moveNext.Clear();

            bool ok = false;
            Block goal = GoalBlocks[_moveGoalIndex];
            _moveQueue.Enqueue(goal);
            Debug.Log("00002");
            while (_moveQueue.TryDequeue(out Block top))
            {
                _moveVis.Add(top);

                if (top == CurrentBlock)
                {
                    ok = true;
                    break;
                }
                Debug.Log($"00003{top.AdjBlocks.Count}");
                foreach (var adj in top.AdjBlocks)
                {
                    if (!_moveVis.Contains(adj))
                    {
                        _moveQueue.Enqueue(adj);
                        _moveNext[adj] = top;
                    }
                }
            }
            Debug.Log($"00003 {ok}");
            if (ok)
            {
                _moveGoalIndex++;
                StartCoroutine(Move(_moveNext, goal));
            }
        }

        private IEnumerator Move(Dictionary<Block, Block> next, Block goal)
        {
            _isMoving = true;
            BlockManager.Instance.DisableInteract();

            yield return StartCoroutine(PathDisplayTrail.Move(next, CurrentBlock, goal));

            while (CurrentBlock != goal)
            {
                Block block = next[CurrentBlock];

                float time = 0;
                while (time < MoveTimePerBlock)
                {
                    time += Time.deltaTime;
                    float p = time / MoveTimePerBlock;
                    Vector3 pos = Vector3.Lerp(CurrentBlock.transform.position, block.transform.position, p);
                    SetPlayerPosition(pos);
                    yield return null;
                }
                CurrentBlock = block;
            }

            for (int i = 0; i < CurrentBlock.transform.childCount; i++)
            {
                Destroy(CurrentBlock.transform.GetChild(i).gameObject);
            }

            TryPlaceGoalHint();
            _isMoving = false;
            BlockManager.Instance.EnableInteract();

            TryMove();
        }

        private void SetPlayerPosition(Vector3 blockPosition)
        {
            transform.position = blockPosition + PositionOffset;
        }

        private void TryPlaceGoalHint()
        {
            if (_moveGoalIndex >= GoalBlocks.Length)
            {
                return;
            }

            Transform goal = GoalBlocks[_moveGoalIndex].transform;
            Instantiate(GoalHintPrefab, goal, false);
        }
    }
}
