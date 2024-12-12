using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TEN.GLOBAL
{
    public static class Path
    {
        public const string MainSceneConfig = @"\Config\SceneConfig\MainScene\interface_main_scene.xml";
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
        }
    }
    namespace ENUM
    {
        public enum EUIType
        {
            CANVAS = 0,

        }
    }
    namespace STRUCT
    {
        //大爷的，C#为毛不能结构体继承

        /// <summary>
        /// 界面属性
        /// </summary>
        public class SInterface
        {
            public Transform Parent;
            public string Name;
            public int Layout;
            public string ImagePath;

            public virtual void Log()
            {
                Debug.Log($"Name : {Name} , Layout : {Layout}");
            }
            public virtual void Reset()
            {
                Parent = null;
                Name = "";
                Layout = 0;

            }
        }

        public class SButtonData : SInterface
        {
            public Vector2 Achor;
            public Vector2 Pos;
            public Vector2 Size;
            public string EventName;
            public string EventParameter;
            public override void Reset()
            {
                base.Reset();
                Achor = Vector2.zero;
                Pos = Vector2.zero;
                Size = Vector2.zero;
            }
            public override void Log()
            {
                Debug.Log($"Name : {Name} , Layout : {Layout} , Achor : {Achor} , Pos : {Pos} , Size : {Size} , ImagePath : {ImagePath} , EventName : {EventName} , EventParameter : {EventParameter}");
            }
        }
    }
}