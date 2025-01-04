using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;

namespace TEN.GLOBAL
{
    public static class Path
    {
        //添加新场景修改内容 2/4
        public static class XMLCongigPath
        {
            public const string MainSceneConfig = @"\Config\SceneConfig\MainScene\interface_main_scene.xml";
            public const string ForkSceneConfig = @"\Config\SceneConfig\MainScene\interface_fork_scene.xml";
            public const string SelfPracticeSceneConfig = @"\Config\SceneConfig\MainScene\interface_self_practice_scene.xml";
            public const string RoundedCornerSceneConfig = @"\Config\SceneConfig\MainScene\interface_rounded_corner_scene.xml";
            public const string GPUInstancingSceneConfig = @"\Config\SceneConfig\MainScene\interface_GPU_instancing_scene.xml";
            public const string TransparentFollowMouseSceneConfig = @"\Config\SceneConfig\MainScene\interface_Transparent_Follow_Mouse_scene.xml";
            public const string RubiksCubeSceneConfig = @"\Config\SceneConfig\MainScene\interface_rubiks_cube_scene.xml";
            public const string StencilTestSceneConfig = @"\Config\SceneConfig\MainScene\interface_stencil_test_scene.xml";
            public const string DreamTickerSceneConfig = @"\Config\SceneConfig\MainScene\interface_dream_ticker_scene.xml";
            public const string WayFindingSceneConfig = @"\Config\SceneConfig\MainScene\interface_find_way_scene.xml";

        }
    }
    public static class Global
    {
        public static class Convert
        {
            public static bool ConvertString(out int pIn_Receive, string vIn_Value)
            {
                if (int.TryParse(vIn_Value, out pIn_Receive))
                {
                    return true;
                }
                else
                {
                    Debug.Log($"Convert Error {vIn_Value}");
                    return false;
                }
            }
            public static bool ConvertString(out float pIn_Receive, string vIn_Value)
            {
                if (float.TryParse(vIn_Value, out pIn_Receive))
                {
                    return true;
                }
                else
                {
                    Debug.Log($"Convert Error {vIn_Value}");
                    return false;
                }
            }
            public static bool ConvertString(out bool pIn_Receive, string vIn_Value)
            {
                if (bool.TryParse(vIn_Value, out pIn_Receive))
                {
                    return true;
                }
                else
                {
                    Debug.Log($"Convert Error {vIn_Value}");
                    return false;
                }
            }

            public static bool ConvertInt2Enum<T>(out T pIn_Enum , int vIn_Value)
                where T : System.Enum
            {
                if (System.Enum.IsDefined(typeof(T), vIn_Value))
                {
                    pIn_Enum = (T)(object)vIn_Value;
                    return true;
                }
                else
                {
                    pIn_Enum = default;
                    return false;
                }
            }
        }
        public static class BitManager
        {
            public static void SetBitToOne(ref int value, int position)
            {
                 value |= (1 << position);
            }
            public static void SetBitToZero(ref int value, int position)
            {
                 value &= ~(1 << position);
            }
            public static int GetBit(int value, int position)
            {
                return (value & (1 << position)) != 0 ? 1 : 0;
            }
            public static void SetBitByBool(ref int pIn_Value, bool vIn_Result, int vIn_Position)
            {
                if (vIn_Result)
                {
                    SetBitToZero(ref pIn_Value, vIn_Position);
                }
                else
                {
                    SetBitToOne(ref pIn_Value, vIn_Position);
                }
            }
        }
        public static class GameobjectOpreate
        {
            public static void SetRectTransform(RectTransform pIn_RT , STRUCT.SBaseData vIn_BaseData)
            {
                pIn_RT.anchorMin = vIn_BaseData.Achor;
                pIn_RT.anchorMax = vIn_BaseData.Achor;
                pIn_RT.sizeDelta = vIn_BaseData.Size;
                pIn_RT.anchoredPosition = vIn_BaseData.Pos;
            }
        }
        public static class ParsingXMLAgreementData
        {
            private static string GetX(ENUM.EXMLAgreement vIn_Agreement)
            {
                switch (vIn_Agreement)
                {
                    case ENUM.EXMLAgreement.xyzw:
                        return "x";
                    case ENUM.EXMLAgreement.XYZW:
                        return "X";
                    case ENUM.EXMLAgreement.wh:
                        return "width";
                    case ENUM.EXMLAgreement.WH:
                        return "Width";
                    case ENUM.EXMLAgreement.rgba:
                        return "r";
                    case ENUM.EXMLAgreement.RGBA:
                        return "R";
                    default:
                        return "x";
                }
            }
            private static string GetY(ENUM.EXMLAgreement vIn_Agreement)
            {
                switch (vIn_Agreement)
                {
                    case ENUM.EXMLAgreement.xyzw:
                        return "y";
                    case ENUM.EXMLAgreement.XYZW:
                        return "Y";
                    case ENUM.EXMLAgreement.wh:
                        return "height";
                    case ENUM.EXMLAgreement.WH:
                        return "Height";
                    case ENUM.EXMLAgreement.rgba:
                        return "g";
                    case ENUM.EXMLAgreement.RGBA:
                        return "G";
                    default:
                        return "y";
                }
            }
            private static string GetZ(ENUM.EXMLAgreement vIn_Agreement)
            {
                switch (vIn_Agreement)
                {
                    case ENUM.EXMLAgreement.xyzw:
                        return "z";
                    case ENUM.EXMLAgreement.XYZW:
                        return "Z";
                    case ENUM.EXMLAgreement.rgba:
                        return "b";
                    case ENUM.EXMLAgreement.RGBA:
                        return "B";
                    default:
                        return "z";
                }
            }
            private static string GetW(ENUM.EXMLAgreement vIn_Agreement)
            {
                switch (vIn_Agreement)
                {
                    case ENUM.EXMLAgreement.xyzw:
                        return "w";
                    case ENUM.EXMLAgreement.XYZW:
                        return "w";
                    case ENUM.EXMLAgreement.rgba:
                        return "a";
                    case ENUM.EXMLAgreement.RGBA:
                        return "A";
                    default:
                        return "w";
                }
            }
            public static void Parsing(ref int pIn_Temp, ref int pIn_TempPos, XmlNode contetnNode, out Vector2 pIn_Result , ENUM.EXMLAgreement vIn_Agreement)
            {
                BitManager.SetBitByBool(ref pIn_Temp, Convert.ConvertString(out pIn_Result.x, contetnNode.Attributes[GetX(vIn_Agreement)].Value), ++pIn_TempPos);
                BitManager.SetBitByBool(ref pIn_Temp, Convert.ConvertString(out pIn_Result.y, contetnNode.Attributes[GetY(vIn_Agreement)].Value), ++pIn_TempPos);
            }
            public static void Parsing(ref int pIn_Temp, ref int pIn_TempPos, XmlNode contetnNode, out Vector3 pIn_Result, ENUM.EXMLAgreement vIn_Agreement)
            {
                BitManager.SetBitByBool(ref pIn_Temp, Convert.ConvertString(out pIn_Result.x, contetnNode.Attributes[GetX(vIn_Agreement)].Value), ++pIn_TempPos);
                BitManager.SetBitByBool(ref pIn_Temp, Convert.ConvertString(out pIn_Result.y, contetnNode.Attributes[GetY(vIn_Agreement)].Value), ++pIn_TempPos);
                BitManager.SetBitByBool(ref pIn_Temp, Convert.ConvertString(out pIn_Result.z, contetnNode.Attributes[GetZ(vIn_Agreement)].Value), ++pIn_TempPos);
            }
            public static void Parsing(ref int pIn_Temp, ref int pIn_TempPos, XmlNode contetnNode, out Vector4 pIn_Result, ENUM.EXMLAgreement vIn_Agreement)
            {
                BitManager.SetBitByBool(ref pIn_Temp, Convert.ConvertString(out pIn_Result.x, contetnNode.Attributes[GetX(vIn_Agreement)].Value), ++pIn_TempPos);
                BitManager.SetBitByBool(ref pIn_Temp, Convert.ConvertString(out pIn_Result.y, contetnNode.Attributes[GetY(vIn_Agreement)].Value), ++pIn_TempPos);
                BitManager.SetBitByBool(ref pIn_Temp, Convert.ConvertString(out pIn_Result.z, contetnNode.Attributes[GetZ(vIn_Agreement)].Value), ++pIn_TempPos);
                BitManager.SetBitByBool(ref pIn_Temp, Convert.ConvertString(out pIn_Result.w, contetnNode.Attributes[GetW(vIn_Agreement)].Value), ++pIn_TempPos);
            }
            public static void Parsing(ref int pIn_Temp, ref int pIn_TempPos, XmlNode contetnNode, out Color pIn_Result, ENUM.EXMLAgreement vIn_Agreement)
            {
                Vector4 result = Vector4.zero;
                Parsing(ref pIn_Temp, ref pIn_TempPos, contetnNode, out result, vIn_Agreement);
                pIn_Result = result;
            }
            public static void Parsing<T>(ref int pIn_Temp, ref int pIn_TempPos, XmlNode contetnNode, out T pIn_Result, string vIn_Agreement = "Type")
                where T : System.Enum
            {
                TEN.GLOBAL.Global.BitManager.SetBitByBool(ref pIn_Temp, Convert.ConvertString(out int type, contetnNode[vIn_Agreement].InnerText), ++pIn_TempPos);
                if (TEN.GLOBAL.Global.BitManager.GetBit(pIn_Temp, pIn_TempPos) == 0)
                {
                    TEN.GLOBAL.Global.BitManager.SetBitByBool(ref pIn_Temp, Convert.ConvertInt2Enum(out pIn_Result, type), ++pIn_TempPos);
                }
                else
                {
                    pIn_Result = default;
                }
            }
        }

        public static class MVector3
        {
            //return a <= b
            public static bool Less(Vector3 a, Vector3 b , float e = 0.01f)
            {
                if ((a.x - b.x)>e)
                {
                    return false;
                }
                else if ((a.y - b.y) > e )
                {
                    return false;
                }
                else if ((a.z - b.z) > e )
                {
                    return false;
                }
                return true;
            }

            public static Vector3 Quantize(Vector3 v, float step = 0.01f)
            {
                return new Vector3(
                    Mathf.Round(v.x / step) * step,
                    Mathf.Round(v.y / step) * step,
                    Mathf.Round(v.z / step) * step
                );
            }
        }

        public static class MDebug
        {
            public static readonly Dictionary<ENUM.MColor, string> Color2String = new Dictionary<ENUM.MColor, string>()
            {
                { ENUM.MColor.BLUE , "blue"},
                { ENUM.MColor.RED , "red"},
                { ENUM.MColor.YELLOW , "yellow"},
            };
            public static string key = "TENLog";
            public static void Log(string s, ENUM.MColor color = ENUM.MColor.RED)
            {
                Debug.Log($"<color={Color2String[color]}>{key}    {s} </color>");
            }
        }
    }
    namespace ENUM
    {
        //添加新场景修改内容 1/4
        public enum EScene
        {
            MAIN_SCENE = 0,
            FORK_SCENE = 1,
            PRACTICE_SCENE = 2,
            ROUNDED_CORNER = 3,
            GPU_INSTANCING = 4,
            TRANSPARENT_FOLLOW_MOUSE = 5,
            FORK_RUBIKS_CUBE = 6,//枘凿六合
            STENCIL_TEST = 7,
            DREAM_TICKER = 8,
            WAY_FINDING = 9,
        }
        public enum EWindowsType
        {
            CANVAS = 0,
            BUTTON = 1,
            IMAGE = 2,
            INFINITE_SCROLLING = 3,//无限滚动窗口
            SLIDER = 4,
            TEXT_MESH_PRO = 5,

        }
        public enum ESliderMapType
        {
            SHADER = 0,
            SHADER_IMAGE = 1,
            GAMEOBJECT_POSITION_X = 2,
        }
        public enum EXMLAgreement
        {
            //小写xyzw
            xyzw,
            XYZW,
            //小写width、height
            wh,
            WH,
            //小写rgba
            rgba,
            RGBA,
        }
        public enum ERubiksCubeInstanceState
        {
            //不透明状态
            NORMAL,
            //透明状态，用于表示被选中的状态
            TRANSPARENT_A,
            //透明状态，用于表示未被选中的状态
            TRANSPARENT_B
        }
        public enum ERubiksCubeInstanceLocation
        {
            FLU,
            FRU,
            FLD,
            FRD,
            BLU,
            BRU,
            BLD,
            BRD
        }
        [System.Flags]
        public enum BlockProjectedShapes
        {
            None = 0,
            LeftUpperTriangle = 1 << 0,
            MiddleUpperTriangle = 1 << 1,
            RightUpperTriangle = 1 << 2,
            LeftLowerTriangle = 1 << 3,
            MiddleLowerTriangle = 1 << 4,
            RightLowerTriangle = 1 << 5,
            Walkable = LeftUpperTriangle | MiddleUpperTriangle,
            FullHexagon = LeftUpperTriangle | MiddleUpperTriangle | RightUpperTriangle | LeftLowerTriangle | MiddleLowerTriangle | RightLowerTriangle,
        }
        public enum BlockCategory
        {
            BeforeMirror,
            InMirror,
            BehindMirror,
        }
        public enum MColor
        {
            RED,
            BLUE,
            YELLOW
        }
    }
    namespace STRUCT
    {
        //大爷的，C#为毛不能结构体继承

        public struct GameobjectMessage
        {
            public Vector3 Location;
            public Quaternion Attitude;
            public Vector3 Scale;
            public GameobjectMessage(Vector3 vIn_Location , Quaternion vIn_Attitude , Vector3 vIn_Scale)
            {
                Location = vIn_Location;
                Attitude = vIn_Attitude;
                Scale = vIn_Scale;
            }
        }

        /// <summary>
        /// 界面属性
        /// </summary>
        public class SInterface
        {
            public Transform Parent;
            public string Name;
            public int Layout;
            public string BackroundImagePath;
            public ENUM.EWindowsType WindowType;
            public SBaseData SBaseData;

            public virtual void Log()
            {
                Debug.Log($"Name : {Name} , Layout : {Layout} WindowType:{WindowType}");
            }
            public virtual void Reset()
            {
                Parent = null;
                Name = "";
                Layout = 0;
                SBaseData.Reset();
            }
        }

        public struct SBaseData
        {
            public Vector2 Achor;
            public Vector2 Pos;
            public Vector2 Size;

            public void Reset()
            {
                Achor = Vector2.zero;
                Pos = Vector2.zero;
                Size = Vector2.zero;
            }
        }

        public class SButtonData : SInterface
        {
            public string EventName;
            public string EventParameter;
            public override void Reset()
            {
                base.Reset();
                
            }
            public override void Log()
            {
                Debug.Log($"Name : {Name} , Layout : {Layout} , Achor : {SBaseData.Achor} , Pos : {SBaseData.Pos} , Size : {SBaseData.Size} , ImagePath : {BackroundImagePath} , EventName : {EventName} , EventParameter : {EventParameter}");
            }
        }

        public class SSliderData : SInterface
        {
            public ENUM.ESliderMapType MapType;
            public string ObjectName;
            public string AttributeName;
            public Vector2 MapRange;
            public override void Reset()
            {
                base.Reset();
                MapType = ENUM.ESliderMapType.SHADER;
                ObjectName = "";
                AttributeName = "";
                MapRange = Vector2.zero;
            }
            public override void Log()
            {
                Debug.Log($"Name : {Name} , Layout : {Layout} , Achor : {SBaseData.Achor} , Pos : {SBaseData.Pos} , Size : {SBaseData.Size} , ImagePath : {BackroundImagePath} , MapType : {MapType} , ObjectName : {ObjectName} , AttributeName : {AttributeName} , MapRange : {MapRange}");
            }
        }

        public class SScrollViewData : SInterface
        {
            public Vector2 ContentSize;
            public List<SInterface> ContentData = new List<SInterface>();
            public bool Horizontal;
            public bool Vertical;
            public ENUM.EWindowsType ChildWindowType;
            public override void Reset()
            {
                base.Reset();
                ContentData.Clear();
            }
            public override void Log()
            {
                
            }
        }

        public class SImageData : SInterface
        {
            public Color Color;
            //public UnityEngine.UI.Image.Type Sprite;

            public override void Reset()
            {
                base.Reset();
            }
            public override void Log()
            {
                Debug.Log($"Name : {Name} , Layout : {Layout} , Achor : {SBaseData.Achor} , Pos : {SBaseData.Pos} , Size : {SBaseData.Size} , ImagePath : {BackroundImagePath} , Color : {Color}");
            }
        }
        public class STextMeshProData : SInterface
        {
            public Color Color;
            public bool AutoSize;
            public int MinSize;
            public int MaxSize;
            public string Text;
            //public UnityEngine.UI.Image.Type Sprite;

            public override void Reset()
            {
                base.Reset();
            }
            public override void Log()
            {
                Debug.Log($"Name : {Name} , Layout : {Layout} , Achor : {SBaseData.Achor} , Pos : {SBaseData.Pos} , Size : {SBaseData.Size} , ImagePath : {BackroundImagePath} , Color : {Color} , Text{Text}");
            }
        }
    }
}