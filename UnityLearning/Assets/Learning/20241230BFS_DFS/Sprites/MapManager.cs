using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TEN.GLOBAL;
using UnityEngine.Pool;

namespace TEN.LEARNING.ALGORITHM
{
	/// <summary>
	///项目 : TEN
	///日期：2024/12/30 21:43:20 
	///创建者：Michael Corleone
	///类用途：研究DFS  BFS
	/// </summary>
	public class MapManager : MonoBehaviour
	{
        [SerializeField]
        private uint _x;
        [SerializeField]
        private uint _y;
        [SerializeField]
        private GameObject _player;

        private Block _begining;
        private Block _terminal;
        private Dictionary<Vector2Int, Block> _blockMap = new Dictionary<Vector2Int, Block>();
        private bool _interaction = false;
        private bool _goToGoal = false;
        private bool _nextStep = false;
        private Block _curBlock;
        private bool dfs_complete = false;
        private int _tryMaxCount = 5;
        private int _tryCount = 0;

        private void Awake()
        {
            InitMap();
            ResetPath();
        }

        private void InitMap()
        {
            //对map中的每个对象都添加自定义的block。block中包含一个容器，用于存放自己的所有邻居
            for (int i = 0; i < transform.childCount; i++)
            {
                Block block = transform.GetChild(i).gameObject.AddComponent<Block>();
                _blockMap.TryAdd(new Vector2Int(Mathf.RoundToInt(block.transform.position.x) , Mathf.RoundToInt(block.transform.position.z)) , block);
            }

            Block neighbor;
            foreach (var item in _blockMap)
            {
                if (_blockMap.TryGetValue(item.Key + Vector2Int.right , out neighbor))
                {
                    item.Value.Neighbors.AddNeighbor(neighbor);
                    neighbor.Neighbors.AddNeighbor(item.Value);
                }

                if (_blockMap.TryGetValue(item.Key + Vector2Int.up, out neighbor))
                {
                    item.Value.Neighbors.AddNeighbor(neighbor);
                    neighbor.Neighbors.AddNeighbor(item.Value);
                }
            }
        }

        private void RandomTerminal()
        {
            if (_tryCount >= _tryMaxCount)
            {
                _terminal = transform.GetChild(transform.childCount - 1).gameObject.GetComponent<Block>();
                return;
            }
            _tryCount++;

            int x = System.Convert.ToInt32(Random.Range(0, _x+1));
            int y = (int)Random.Range(0, _y+1);
            if (!_blockMap.TryGetValue(new Vector2Int(x, y), out _terminal))
            {
                RandomTerminal();
            }
            if (_terminal == _begining)
            {
                RandomTerminal();
            }
            _tryCount = 0;
            _terminal.GetComponent<MeshRenderer>().material.color = Color.magenta;
        }

        private void ResetPath()
        {
            if (!_blockMap.TryGetValue(new Vector2Int(_player.transform.position.RoundToInt().x , _player.transform.position.RoundToInt().z), out _begining))
            {
                _begining = transform.GetChild(0).gameObject.GetComponent<Block>();
            }
            _curBlock = _begining;
            RandomTerminal();
        }

        private IEnumerator WaitInteraction()
        {
            if (!_goToGoal)
            {
                while (!_nextStep)
                    yield return null;
                _nextStep = false;
            }
        }

        private IEnumerator BFS()
        {
            if (_interaction)
            {
                yield break;
            }
            _interaction = true;


            Dictionary<Block , Block> path = DictionaryPool<Block,Block>.Get();
            HashSet<Block> visitedNeighbor = HashSetPool<Block>.Get();
            Queue<Block> neighbor = new Queue<Block>();

            //初始值，起点或终点
            neighbor.Enqueue(_terminal);
            while (neighbor.TryDequeue(out Block block))
            {
                //交互方式，根据点击的按钮，确定是否要按步执行。用于了解算法的工作原理。
                yield return StartCoroutine(WaitInteraction());

                if (block == _begining)
                {
                    Global.MDebug.Log($"path find");
                    break;
                }
                Global.MDebug.Log($"{block.name}",  GLOBAL.ENUM.MColor.YELLOW);

                block.gameObject.GetComponent<MeshRenderer>().material.color = Color.yellow;
                //开始访问邻居
                foreach (var item in block.Neighbors)
                {
                    if (!visitedNeighbor.Contains(item))
                    {
                        //将当前邻居标记为已访问过
                        visitedNeighbor.Add(item);
                        //保存来时路，用于确定路径。
                        path[item] = block;
                        //将邻居的邻居存入访问队列
                        neighbor.Enqueue(item);
                        item.gameObject.GetComponent<MeshRenderer>().material.color = Color.cyan;
                    }
                }

            }


            while (_curBlock != _terminal)
            {
                yield return StartCoroutine(WaitInteraction());
                Global.MDebug.Log($"{_curBlock.name}");
                Block block = path[_curBlock];
                //TODO 移动效果
                _player.transform.position = block.transform.position + Vector3.up;
                yield return new WaitForSeconds(0.5f);
                block.gameObject.GetComponent<MeshRenderer>().material.color = Color.white;
                _curBlock = block;
            }

            //移动结束，BFS寻路完成
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).GetComponent<MeshRenderer>().material.color = Color.black;
            }
            ResetPath();
            _goToGoal = false;
            _interaction = false;
            DictionaryPool<Block, Block>.Release(path);
            HashSetPool<Block>.Release(visitedNeighbor);
        }

        private IEnumerator DFS()
        {
            //DFS寻路入口
            if (_interaction)
            {
                yield break;
            }
            _interaction = true;
            //访问过的对象
            HashSet<Block> visitedNeighbor = HashSetPool<Block>.Get();
            //需要访问的对象
            Stack<Block> blocks = new Stack<Block>();
            //路径
            Stack<Block> path = new Stack<Block>();
            //从终点开始查找，方便
            path.Push(_terminal);
            blocks.Push(_terminal);
            visitedNeighbor.Add(_terminal);
            yield return StartCoroutine(DFS_Tranverse(visitedNeighbor , blocks,path));
            while (_curBlock != _terminal)
            {
                yield return StartCoroutine(WaitInteraction());
                Global.MDebug.Log($"{_curBlock.name}");
                Block block = path.Pop();
                //TODO 移动效果
                _player.transform.position = block.transform.position + Vector3.up;
                yield return new WaitForSeconds(0.5f);
                block.gameObject.GetComponent<MeshRenderer>().material.color = Color.white;
                _curBlock = block;
            }
            //移动结束，DFS寻路完成
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).GetComponent<MeshRenderer>().material.color = Color.black;
            }
            ResetPath();
            _goToGoal = false;
            _interaction = false;
            dfs_complete = false;
            HashSetPool<Block>.Release(visitedNeighbor);
        }
        private IEnumerator DFS_Tranverse(HashSet<Block> pIn_VisitedNeighbor, Stack<Block> pIn_Blocks , Stack<Block> pIn_Path)
        {
            //弹出一个block，并遍历其邻居。（这里存在问题，关于邻居的选择，当前用的是如果A的邻居存在B，那么B的邻居中也必定存在A。）
            Block block = pIn_Blocks.Pop();
            foreach (var item in block.Neighbors)
            {
                yield return new WaitForSeconds(0.2f);
                if (item == _begining)
                {
                    //找到起点，退出所有协程。
                    dfs_complete = true;
                    yield break;
                }
                if (!pIn_VisitedNeighbor.Contains(item))
                {
                    //将当前对象标记为访问过
                    pIn_VisitedNeighbor.Add(item);
                    pIn_Blocks.Push(item);
                    pIn_Path.Push(item);
                    item.gameObject.GetComponent<MeshRenderer>().material.color = Color.cyan;
                    //一条路走到黑
                    yield return StartCoroutine(DFS_Tranverse(pIn_VisitedNeighbor, pIn_Blocks, pIn_Path));
                }
                if (dfs_complete)
                {
                    yield break;
                }
            }
            //两种情况可以走到这里。1、当前block不存在邻居。2、当前block的所有邻居都不是起点。
            pIn_Path.Pop().gameObject.GetComponent<MeshRenderer>().material.color = Color.red;
        }
        private IEnumerator AStar()
        {
            //AS寻路入口
            if (_interaction)
            {
                yield break;
            }
            _interaction = true;

            //好像存在路径中断的情况，且将修改全部颜色的函数放置此处
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).GetComponent<MeshRenderer>().material.color = Color.black;
            }

            HashSet<Block> visitedNeighbor = HashSetPool<Block>.Get();
            //需要访问的对象
            Block nextStep;
            //路径
            Stack<Block> path = new Stack<Block>();
            //从终点开始查找，方便
            nextStep = _terminal;
            path.Push(_terminal);
            while (nextStep != null)
            {
                //单路径时死循环
                //path.Push(block);
                yield return new WaitForSeconds(0.2f);
                Block minDisBlock = nextStep.GetMinDisBlockToGoal(_begining, visitedNeighbor);
                if (minDisBlock != null)
                {
                    //路径可能断掉
                    //path.Push(minDisBlock);
                    path.Push(nextStep);
                    nextStep.gameObject.GetComponent<MeshRenderer>().material.color = Color.cyan;
                    nextStep = minDisBlock;
                    if (minDisBlock == _begining)
                    {
                        break;
                    }
                }
                else
                {
                    if (path.TryPop(out Block temp))
                    {
                        nextStep = temp;
                        temp.gameObject.GetComponent<MeshRenderer>().material.color = Color.red;
                    }
                }
            }


            while (_curBlock != _terminal)
            {
                yield return StartCoroutine(WaitInteraction());
                Global.MDebug.Log($"{_curBlock.name}");
                Block block = path.Pop();
                //TODO 移动效果
                _player.transform.position = block.transform.position + Vector3.up;
                yield return new WaitForSeconds(0.5f);
                block.gameObject.GetComponent<MeshRenderer>().material.color = Color.white;
                _curBlock = block;
            }
            //移动结束，DFS寻路完成

            ResetPath();
            _goToGoal = false;
            _interaction = false;
            HashSetPool<Block>.Release(visitedNeighbor);
        }

        public void OnASGoToGoalClick()
        {
            _goToGoal = true;
            StartCoroutine(AStar());
        }
        public void OnDFSGoToGoalClick()
        {
            _goToGoal = true;
            StartCoroutine(DFS());
        }
        public void OnGoToGoalClick()
        {
            _goToGoal = true;
            StartCoroutine(BFS());
        }
        public void OnNextStep()
        {
            _nextStep = true;
        }
    }
}
