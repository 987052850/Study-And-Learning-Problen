using System.Collections;
using System.Xml;
using UnityEngine;
using UnityEngine.Networking;
using static TEN.GLOBAL.Global.Convert;

public class ReadConfig : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(LoadXML());
    }

    private IEnumerator LoadXML()
    {
        // 获取 StreamingAssets 路径
        string filePath = Application.streamingAssetsPath + TEN.GLOBAL.Path.MainSceneConfig;

        string xmlContent;

        // Android 平台需要用 UnityWebRequest 读取
        if (Application.platform == RuntimePlatform.Android)
        {
            UnityWebRequest request = UnityWebRequest.Get(filePath);
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                xmlContent = request.downloadHandler.text;
            }
            else
            {
                Debug.LogError("Failed to load XML: " + request.error);
                yield break;
            }
        }
        else
        {
            // 其他平台直接读取
            xmlContent = System.IO.File.ReadAllText(filePath);
        }

        // 加载 XML 内容
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(xmlContent);

        // 解析 XML 数据
        ParseXML(xmlDoc);
    }

    private void SetBitByBool(ref int pIn_Value , bool vIn_Result , int vIn_Position)
    {
            if (vIn_Result)
            {
                TEN.GLOBAL.Global.BitManager.SetBitToZero(ref pIn_Value, vIn_Position);
            }
            else
            {
                TEN.GLOBAL.Global.BitManager.SetBitToOne(ref pIn_Value, vIn_Position);
            }
    }

    private void ParseXML(XmlDocument xmlDoc)
    {
        // 获取 Interface 节点
        XmlNode interfaceNode = xmlDoc.SelectSingleNode("InterfaceConfig/Interface");
        Transform tCanvas = null;
        TEN.GLOBAL.STRUCT.SInterface sInterface = new TEN.GLOBAL.STRUCT.SInterface();
        int temp = 0;

        if (interfaceNode != null)
        {
            sInterface.Reset();
            sInterface.Name = interfaceNode["Name"].InnerText;
            sInterface.ImagePath = interfaceNode["Image"].Attributes["path"].Value;
            SetBitByBool(ref temp, ConvertString(out int type, interfaceNode["Layout"].InnerText), 0);
            SetBitByBool(ref temp, ConvertString(out sInterface.Layout, interfaceNode["Layout"].InnerText), 1);

            if (temp != 0)
            {
                Debug.Log($"XML Parsing Error , {System.Convert.ToString(temp, 2).PadLeft(32, '0')}");
            }
            tCanvas = TEN.MANAGER.InstantiateManager.Instance.InstantiateCanvas(sInterface).transform;
            sInterface.Log();
        }

        // 获取 ButtonArray 节点
        XmlNodeList buttonNodes = xmlDoc.SelectNodes("InterfaceConfig/ButtonArray/Button");
        TEN.GLOBAL.STRUCT.SButtonData sButtonData = new TEN.GLOBAL.STRUCT.SButtonData();
        foreach (XmlNode buttonNode in buttonNodes)
        {
            sButtonData.Reset();
            temp = 0;
            sButtonData.Parent = tCanvas;
            sButtonData.Name = buttonNode["Name"].InnerText;
            sButtonData.ImagePath = buttonNode["Image"].Attributes["path"].Value;
            SetBitByBool(ref temp, ConvertString(out sButtonData.Achor.x, buttonNode["Anchor"].Attributes["x"].Value), 2);
            SetBitByBool(ref temp, ConvertString(out sButtonData.Achor.y, buttonNode["Anchor"].Attributes["y"].Value), 3);
            SetBitByBool(ref temp, ConvertString(out sButtonData.Pos.x, buttonNode["Position"].Attributes["x"].Value), 4);
            SetBitByBool(ref temp, ConvertString(out sButtonData.Pos.y, buttonNode["Position"].Attributes["y"].Value), 5);
            SetBitByBool(ref temp, ConvertString(out sButtonData.Size.x, buttonNode["Size"].Attributes["width"].Value), 6);
            SetBitByBool(ref temp, ConvertString(out sButtonData.Size.y, buttonNode["Size"].Attributes["height"].Value), 7);
            sButtonData.EventName = buttonNode["Event"].Attributes["name"].Value;
            sButtonData.EventParameter = buttonNode["Event"].Attributes["parameter"].Value;
            if (temp != 0)
            {
                Debug.Log($"XML Parsing Error , {System.Convert.ToString(temp, 2).PadLeft(32, '0')}");
            }
            TEN.MANAGER.InstantiateManager.Instance.InstantiateButton(sButtonData);
            sButtonData.Log();
        }
    }
}
