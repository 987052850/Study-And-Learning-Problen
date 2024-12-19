using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TEN.GLOBAL;
using UnityEngine.UI;
using static TEN.GLOBAL.Global.MVector3;
using TEN.GLOBAL.ENUM;

namespace TEN.LEARNING
{
	/// <summary>
	///项目 : TEN
	///日期：2024/12/16 19:02:58 
	///创建者：Michael Corleone
	///类用途：
	/// </summary>
	public class RubikCube : MonoBehaviour
	{
        private GameObject _flu;
        private GameObject _fru;
        private GameObject _fld;
        private GameObject _frd;

        private GameObject _blu;
        private GameObject _bru;
        private GameObject _bld;
        private GameObject _brd;


        private Button _upFace;
        private GameObject[] _upFaceGameobjects;
        private UTILS.RotateAroundCenter _upRotate = new UTILS.RotateAroundCenter();

        private Button _downFace;
        private GameObject[] _downFaceGameobjects;
        private UTILS.RotateAroundCenter _downRotate = new UTILS.RotateAroundCenter();

        private Button _forwardFace;
        private GameObject[] _forwardFaceGameobjects;
        private UTILS.RotateAroundCenter _forwardRotate = new UTILS.RotateAroundCenter();

        private Button _backFace;
        private GameObject[] _backFaceGameobjects;
        private UTILS.RotateAroundCenter _backRotate = new UTILS.RotateAroundCenter();

        private Button _rightFace;
        private GameObject[] _rightFaceGameobjects;
        private UTILS.RotateAroundCenter _rightRotate = new UTILS.RotateAroundCenter();

        private Button _leftFace;
        private GameObject[] _leftFaceGameobjects;
        private UTILS.RotateAroundCenter _leftRotate = new UTILS.RotateAroundCenter();

        private GameObject[] _chooseFace;
        private GameObject[] _unChooseFace;
        private UTILS.RotateAroundCenter _chooseRotateManager;

        private Button _positiveButton;
        private Button _reverseButton;

        private MANAGER.ButtonsMonitor _buttonsMonitor;
        private bool _animationRunning = false;
        private float _rotateTime = 2;
        private float _curTime = 0;
        List<GameObject> _allCubeData = new List<GameObject>();
        private MANAGER.AlignManger _alignManger1;
        private MANAGER.AlignManger _alignManger2;

        private void Awake()
        {
            _upFace = GameObject.Find("Canvas/UpFace").GetComponent<Button>();
            _upFace.onClick.AddListener(ChooseFaceUP);
            _downFace = GameObject.Find("Canvas/DownFace").GetComponent<Button>();
            _downFace.onClick.AddListener(ChooseFaceDown);

            _forwardFace = GameObject.Find("Canvas/ForwardFace").GetComponent<Button>();
            _forwardFace.onClick.AddListener(ChooseFaceForward);
            _backFace = GameObject.Find("Canvas/BackFace").GetComponent<Button>();
            _backFace.onClick.AddListener(ChooseFaceBack);

            _rightFace = GameObject.Find("Canvas/RightFace").GetComponent<Button>();
            _rightFace.onClick.AddListener(ChooseFaceRight);
            _leftFace = GameObject.Find("Canvas/LeftFace").GetComponent<Button>();
            _leftFace.onClick.AddListener(ChooseFaceLeft);

            _positiveButton = GameObject.Find("Canvas/Positive").GetComponent<Button>();
            _positiveButton.onClick.AddListener(PositiveRotate);
            _reverseButton = GameObject.Find("Canvas/Reverse").GetComponent<Button>();
            _reverseButton.onClick.AddListener(ReverseRotate);

            _buttonsMonitor = new MANAGER.ButtonsMonitor();

            _upFaceGameobjects      = new GameObject[4];
            _downFaceGameobjects    = new GameObject[4];
            _forwardFaceGameobjects = new GameObject[4];
            _backFaceGameobjects = new GameObject[4];
            _rightFaceGameobjects = new GameObject[4];
            _leftFaceGameobjects = new GameObject[4];

            _chooseFace   = null;
            _unChooseFace = null;

            UpdateAllFace();
            UpdateShaderState();
        }
        private void UpdateShaderState()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                MANAGER.RubikxCubeShaderManager.Instance.SetState(transform.GetChild(i).gameObject , ERubiksCubeInstanceState.TRANSPARENT_B);
            }
        }

        private void UpdateAllFace()
        {
            _flu = null;
            _fru = null;
            _fld = null;
            _frd = null;
            _blu = null;
            _bru = null;
            _bld = null;
            _brd = null;
            //设置后右上、前左上的默认值，这样遍历时便可以避免第一个对象是前左上而导致的无法定位到的情况发生
            _bru = transform.GetChild(0).gameObject;
            _flu = _bru;
            //坐标值全部最小的为 后右下 的cube
            //坐标值全部最大的为 前左上 的cube
            Vector3 minCoord = _bru.transform.position;
            Vector3 maxCoord = transform.GetChild(0).position;
            Vector3 tempVector3 = Vector3.zero;
            _allCubeData.Clear();
            foreach (var item in transform.GetComponentsInChildren<Transform>())
            {
                if (item == transform)
                {
                    continue;
                }
                tempVector3 = item.position;
                if (Less(tempVector3, minCoord))
                {
                    minCoord = tempVector3;
                    _brd = item.gameObject;
                }else
                if (Less(maxCoord, tempVector3))
                {
                    maxCoord = tempVector3;
                    _flu = item.gameObject;
                }
                _allCubeData.Add(item.gameObject);
            }
            Vector3 span = maxCoord - _brd.transform.position;
            int monitor = 0;
            int monitorPos = 0;

            tempVector3 = new Vector3(maxCoord.x - span.x, maxCoord.y, maxCoord.z);
            //tempVector3 = Quantize(tempVector3);
            Global.BitManager.SetBitByBool(ref monitor, TryGetValue(tempVector3, out _fru), monitorPos++);

            tempVector3 = new Vector3(maxCoord.x, maxCoord.y - span.y, maxCoord.z);
            Global.BitManager.SetBitByBool(ref monitor, TryGetValue(tempVector3, out _fld), monitorPos++);

            tempVector3 = new Vector3(maxCoord.x - span.x, maxCoord.y - span.y, maxCoord.z);
            Global.BitManager.SetBitByBool(ref monitor, TryGetValue(tempVector3, out _frd), monitorPos++);

            tempVector3 = new Vector3(minCoord.x + span.x, minCoord.y, minCoord.z);
            Global.BitManager.SetBitByBool(ref monitor, TryGetValue(tempVector3, out _bld), monitorPos++);

            tempVector3 = new Vector3(minCoord.x, minCoord.y + span.y, minCoord.z);
            Global.BitManager.SetBitByBool(ref monitor, TryGetValue(tempVector3, out _bru), monitorPos++);

            tempVector3 = new Vector3(minCoord.x + span.x, minCoord.y + span.y, minCoord.z);
            Global.BitManager.SetBitByBool(ref monitor, TryGetValue(tempVector3, out _blu), monitorPos++);

            if (monitor != 0)
            {
                Debug.Log($"ERROR : {System.Convert.ToString(monitor, 2).PadLeft(32 , '0')}");
            }

            SetFaceUp();
            SetFaceDown();
            SetFaceForward();
            SetFaceBack();
            SetFaceRight();
            SetFaceLeft();

            _alignManger1 = GameObject.Find("Align").GetComponent<MANAGER.AlignManger>();
            _alignManger2 = GameObject.Find("Align1").GetComponent<MANAGER.AlignManger>();
        }

        private bool TryGetValue(Vector3 vIn_Pos , out GameObject pIn_Gameobject , float vIn_Epsilon = 0.0001f)
        {
            Vector3 temp = Vector3.zero;
            Vector3 temp1;
            foreach (var item in _allCubeData)
            {
                temp = item.transform.position;
                temp1 = temp - vIn_Pos;
                if (Vector3.Dot(temp1, temp1) <= vIn_Epsilon)
                {
                    pIn_Gameobject = item;
                    return true;
                }
            }
            pIn_Gameobject = null;
            return false;
        }

        private void SetFaceUp()
        {
            _upFaceGameobjects[0] = _flu;
            _upFaceGameobjects[1] = _fru;
            _upFaceGameobjects[2] = _bru;
            _upFaceGameobjects[3] = _blu;
            _upRotate.Reset(_upFaceGameobjects , Vector3.up);
        }
        private void ResetFaceUp(float vIn_Degree)
        {
            if (vIn_Degree > 0)
            {
                GameObject temp = _fru;
                _fru = _bru;
                _bru = _blu;
                _blu = _flu;
                _flu = temp;
            }
            else
            {
                //TODE
            }
        }

        private void SetFaceDown()
        {
            _downFaceGameobjects[0] = _fld;
            _downFaceGameobjects[1] = _frd;
            _downFaceGameobjects[2] = _brd;
            _downFaceGameobjects[3] = _bld;
            _downRotate.Reset(_downFaceGameobjects , Vector3.up);
        }
        private void SetFaceForward()
        {
            _forwardFaceGameobjects[0] = _flu;
            _forwardFaceGameobjects[1] = _fru;
            _forwardFaceGameobjects[2] = _frd;
            _forwardFaceGameobjects[3] = _fld;
            _forwardRotate.Reset(_forwardFaceGameobjects , -Vector3.forward);
        }
        private void SetFaceBack()
        {
            _backFaceGameobjects[0] = _bld;
            _backFaceGameobjects[1] = _brd;
            _backFaceGameobjects[2] = _bru;
            _backFaceGameobjects[3] = _blu;
            _backRotate.Reset(_backFaceGameobjects , -Vector3.forward);
        }
        private void SetFaceRight()
        {
            _rightFaceGameobjects[0] = _fru;
            _rightFaceGameobjects[1] = _bru;
            _rightFaceGameobjects[2] = _brd;
            _rightFaceGameobjects[3] = _frd;
            _rightRotate.Reset(_rightFaceGameobjects , Vector3.right);
        }
        private void SetFaceLeft()
        {
            _leftFaceGameobjects[0] = _flu;
            _leftFaceGameobjects[1] = _blu;
            _leftFaceGameobjects[2] = _bld;
            _leftFaceGameobjects[3] = _fld;
            _leftRotate.Reset(_leftFaceGameobjects , Vector3.right);
        }

        private void UpdateFace()
        {
            foreach (var item in _chooseFace)
            {
                //item.GetComponent<MeshRenderer>().enabled = true;
                ERubiksCubeInstanceState type = _buttonsMonitor.Monitor ?
                    ERubiksCubeInstanceState.TRANSPARENT_A : ERubiksCubeInstanceState.TRANSPARENT_B;
                MANAGER.RubikxCubeShaderManager.Instance.SetState(item, type);
            }
            foreach (var item in _unChooseFace)
            {
                //item.GetComponent<MeshRenderer>().enabled = !_buttonsMonitor.Monitor;
                MANAGER.RubikxCubeShaderManager.Instance.SetState(item, ERubiksCubeInstanceState.TRANSPARENT_B);
            }
            if (!_buttonsMonitor.Monitor)
            {
                _chooseFace = null;
            }
        }
        public void ChooseFaceUP()
        {
            if (_animationRunning)
            {
                return;
            }
            _buttonsMonitor.OnChildClick(_upFace);
            _chooseFace = _upFaceGameobjects;
            _chooseRotateManager = _upRotate;
            _unChooseFace = _downFaceGameobjects;
            UpdateFace();
        }
        public void ChooseFaceDown()
        {
            if (_animationRunning)
            {
                return;
            }
            _buttonsMonitor.OnChildClick(_downFace);
            _chooseFace = _downFaceGameobjects;
            _chooseRotateManager = _downRotate;
            _unChooseFace = _upFaceGameobjects;
            UpdateFace();
        }

        public void ChooseFaceForward()
        {
            if (_animationRunning)
            {
                return;
            }
            _buttonsMonitor.OnChildClick(_forwardFace);
            _chooseFace = _forwardFaceGameobjects;
            _chooseRotateManager = _forwardRotate;
            _unChooseFace = _backFaceGameobjects;
            UpdateFace();
        }
        public void ChooseFaceBack()
        {
            if (_animationRunning)
            {
                return;
            }
            _buttonsMonitor.OnChildClick(_backFace);
            _chooseFace = _backFaceGameobjects;
            _chooseRotateManager = _backRotate;
            _unChooseFace = _forwardFaceGameobjects;
            UpdateFace();
        }

        public void ChooseFaceRight()
        {
            if (_animationRunning)
            {
                return;
            }
            _buttonsMonitor.OnChildClick(_rightFace);
            _chooseFace = _rightFaceGameobjects;
            _chooseRotateManager = _rightRotate;
            _unChooseFace = _leftFaceGameobjects;
            UpdateFace();
        }
        public void ChooseFaceLeft()
        {
            if (_animationRunning)
            {
                return;
            }
            _buttonsMonitor.OnChildClick(_leftFace);
            _chooseFace = _leftFaceGameobjects;
            _chooseRotateManager = _leftRotate;
            _unChooseFace = _rightFaceGameobjects;
            UpdateFace();
        }
        public void PositiveRotate()
        {
            if (_animationRunning || _chooseFace == null)
            {
                return;
            }
            _animationRunning = true;
            StartCoroutine(Rotate(90));
        }
        public void ReverseRotate()
        {
            if (_animationRunning || _chooseFace == null)
            {
                return;
            }
            _animationRunning = true;
            StartCoroutine(Rotate(-90));
        }
        IEnumerator Rotate(float vIn_Degree)
        {
            _curTime = 0;
            float velocity = vIn_Degree / _rotateTime;
            List<Vector3> tempList = new List<Vector3>();
            foreach (var item in _chooseFace)
            {
                tempList.Add(item.transform.position);
            }
            while (_curTime < _rotateTime)
            {
                _chooseRotateManager.Rotate(velocity * Time.deltaTime);
                _curTime += Time.deltaTime;
                yield return null;
            }
            //for (int i = 0; i < _chooseFace.Length - 2; i++)
            //{
            //    _chooseFace[i].transform.position = tempList[i + 1];
            //}
            //_chooseFace[_chooseFace.Length - 1].transform.position = tempList[0];
            UpdateAllFace();
            _animationRunning = false;
            Detected();
        }
        private void Log(GameObject[] pIn_GameObjects , string pIn_Tag = "")
        {
            foreach (var item in pIn_GameObjects)
            {
                Debug.Log($"{pIn_Tag}   {item.name}");
            }
        }
        private void Detected()
        {
            if (_alignManger1.Detected())
            {
                if (_alignManger2.Detected())
                {
                    //TODO Complete
                    Debug.Log("complete !!!");
                }
            }
        }
    }
}
