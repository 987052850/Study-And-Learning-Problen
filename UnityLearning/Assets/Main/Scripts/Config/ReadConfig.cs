using System.Collections;
using System.Xml;
using UnityEngine;
using UnityEngine.Networking;
using TEN.GLOBAL.ENUM;
using static TEN.GLOBAL.Global.Convert;
using static TEN.GLOBAL.Global.ParsingXMLAgreementData;
using static TEN.GLOBAL.Global.BitManager;

public class ReadConfig : MonoBehaviour
{
    public static ReadConfig ConfigReader;
    private void Awake()
    {
        if (ConfigReader)
        {
            Destroy(gameObject);
            return;
        }
        ConfigReader = this;
        DontDestroyOnLoad(this);
    }
    private void Start()
    {
        ReadXML(Application.streamingAssetsPath + TEN.GLOBAL.Path.XMLCongigPath.MainSceneConfig);
    }

    public void ReadXML(string pIn_XMLPath)
    {
        StartCoroutine(LoadXML(pIn_XMLPath));
    }

    private IEnumerator LoadXML(string pIn_XMLPath)
    {
        // 获取 StreamingAssets 路径
        string filePath = pIn_XMLPath;

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
            if (!System.IO.File.Exists(filePath))
            {
                Debug.Log($"XML Reader Log : Error {filePath} 不存在");
                yield break;
            }
            // 其他平台直接读取
            xmlContent = System.IO.File.ReadAllText(filePath);
        }

        // 加载 XML 内容
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(xmlContent);

        // 解析 XML 数据
        ParseXML(xmlDoc);
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
            sInterface.BackroundImagePath = interfaceNode["Image"].Attributes["path"].Value;
            TEN.GLOBAL.Global.BitManager.SetBitByBool(ref temp, ConvertString(out int type, interfaceNode["Type"].InnerText), 0);
            TEN.GLOBAL.Global.BitManager.SetBitByBool(ref temp, ConvertString(out sInterface.Layout, interfaceNode["Layout"].InnerText), 1);
            if (TEN.GLOBAL.Global.BitManager.GetBit(temp, 0) == 0)
            {
                TEN.GLOBAL.Global.BitManager.SetBitByBool(ref temp, ConvertInt2Enum(out sInterface.WindowType, type), 2);
            }
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
            sButtonData.BackroundImagePath = buttonNode["Image"].Attributes["path"].Value;
            TEN.GLOBAL.Global.BitManager.SetBitByBool(ref temp, ConvertString(out int type, buttonNode["Type"].InnerText), 0);
            if (TEN.GLOBAL.Global.BitManager.GetBit(temp, 0) == 0)
            {
                TEN.GLOBAL.Global.BitManager.SetBitByBool(ref temp, ConvertInt2Enum(out sButtonData.WindowType, type), 1);
            }
            TEN.GLOBAL.Global.BitManager.SetBitByBool(ref temp, ConvertString(out sButtonData.SBaseData.Achor.x, buttonNode["Anchor"].Attributes["x"].Value), 2);
            TEN.GLOBAL.Global.BitManager.SetBitByBool(ref temp, ConvertString(out sButtonData.SBaseData.Achor.y, buttonNode["Anchor"].Attributes["y"].Value), 3);
            TEN.GLOBAL.Global.BitManager.SetBitByBool(ref temp, ConvertString(out sButtonData.SBaseData.Pos.x, buttonNode["Position"].Attributes["x"].Value), 4);
            TEN.GLOBAL.Global.BitManager.SetBitByBool(ref temp, ConvertString(out sButtonData.SBaseData.Pos.y, buttonNode["Position"].Attributes["y"].Value), 5);
            TEN.GLOBAL.Global.BitManager.SetBitByBool(ref temp, ConvertString(out sButtonData.SBaseData.Size.x, buttonNode["Size"].Attributes["width"].Value), 6);
            TEN.GLOBAL.Global.BitManager.SetBitByBool(ref temp, ConvertString(out sButtonData.SBaseData.Size.y, buttonNode["Size"].Attributes["height"].Value), 7);
            sButtonData.EventName = buttonNode["Event"].Attributes["name"].Value;
            sButtonData.EventParameter = buttonNode["Event"].Attributes["parameter"].Value;
            if (temp != 0)
            {
                Debug.Log($"XML Parsing Error , {System.Convert.ToString(temp, 2).PadLeft(32, '0')}");
            }
            TEN.MANAGER.InstantiateManager.Instance.InstantiateButton(sButtonData);
            sButtonData.Log();
        }

        ParsingScrollViewData(xmlDoc , tCanvas);
        ParsingSliderArray(xmlDoc, tCanvas);
        ParsingStatesWindow(xmlDoc, tCanvas);
    }

    private void ParsingStatesWindow(XmlDocument vIn_xmlDoc, Transform vIn_Parent)
    {
        XmlNode sliderArray = vIn_xmlDoc.SelectSingleNode("InterfaceConfig/StateWindow");
        if (sliderArray == null) return;
        XmlNode DeltaTimeNode = sliderArray.SelectSingleNode("DeltaTime");
        ConvertString(out float updateStep, DeltaTimeNode.InnerText);
        XmlNode BackgroundNode = sliderArray.SelectSingleNode("Image");
        TEN.GLOBAL.STRUCT.SImageData ImageData = new TEN.GLOBAL.STRUCT.SImageData();
        ImageData.Parent = vIn_Parent;
        ParsingImage(BackgroundNode, ImageData);
        Transform image = TEN.MANAGER.InstantiateManager.Instance.InstantiateImage(ImageData).transform;
        XmlNodeList textMeshProArray = sliderArray.SelectNodes("TextMeshProArray/TextMeshPro");
        TEN.GLOBAL.STRUCT.STextMeshProData textMeshProData = new TEN.GLOBAL.STRUCT.STextMeshProData();
        foreach (XmlNode item in textMeshProArray)
        {
            textMeshProData.Reset();
            textMeshProData.Parent = image;
            ParsingTextMeshPro(item, textMeshProData);
            TEN.MANAGER.InstantiateManager.Instance.InstantiateTextMeshPro(textMeshProData);
        }
        image.gameObject.AddComponent<TEN.MANAGER.StateWindowManager>().SetTimeSpan(updateStep/1000.0f);
    }
    private bool ParsingTextMeshPro(XmlNode contetnNode, TEN.GLOBAL.STRUCT.STextMeshProData pIn_TextMeshProData)
    {
        //用于通过位运算判断解析是否成功的变量，等于0为解析成功
        int temp = 0;
        //用于监控当前解析位置的变量，默认为-1，因为在解析时均使用++i操作
        int tempPos = -1;
        ParsingBaseDate(ref temp, ref tempPos, contetnNode, pIn_TextMeshProData);
        Parsing(ref temp, ref tempPos, contetnNode["Color"], out pIn_TextMeshProData.Color, EXMLAgreement.rgba);
        SetBitByBool(ref temp, ConvertString(out pIn_TextMeshProData.AutoSize, contetnNode["Font"].Attributes["aotoSize"].Value), ++tempPos);
        SetBitByBool(ref temp, ConvertString(out pIn_TextMeshProData.MinSize, contetnNode["Font"].Attributes["minSize"].Value), ++tempPos);
        SetBitByBool(ref temp, ConvertString(out pIn_TextMeshProData.MaxSize, contetnNode["Font"].Attributes["maxSize"].Value), ++tempPos);
        pIn_TextMeshProData.Text = contetnNode["Text"].InnerText;
        if (temp != 0)
        {
            Debug.Log($"XML Parsing Error , {System.Convert.ToString(temp, 2).PadLeft(32, '0')}");
            return false;
        }
        return true;
    }
    private bool ParsingImage(XmlNode contetnNode, TEN.GLOBAL.STRUCT.SImageData pIn_ImageData)
    {
        //用于通过位运算判断解析是否成功的变量，等于0为解析成功
        int temp = 0;
        //用于监控当前解析位置的变量，默认为-1，因为在解析时均使用++i操作
        int tempPos = -1;
        ParsingBaseDate(ref temp , ref tempPos , contetnNode , pIn_ImageData);
        Parsing(ref temp, ref tempPos, contetnNode["Color"], out pIn_ImageData.Color, EXMLAgreement.rgba);
        if (temp != 0)
        {
            Debug.Log($"XML Parsing Error , {System.Convert.ToString(temp, 2).PadLeft(32, '0')}");
            return false;
        }
        return true;
    }
    private void ParsingBaseDate(ref int pIn_Temp, ref int pIn_TempPos ,XmlNode contetnNode, TEN.GLOBAL.STRUCT.SInterface pIn_Base)
    {
        pIn_Base.Name = contetnNode["Name"].InnerText;
        if(contetnNode["Image"] != null)
            pIn_Base.BackroundImagePath = contetnNode["Image"].Attributes["path"].Value;
        Parsing(ref pIn_Temp, ref pIn_TempPos, contetnNode, out pIn_Base.WindowType,"Type");
        Parsing(ref pIn_Temp, ref pIn_TempPos , contetnNode["Anchor"] , out pIn_Base.SBaseData.Achor , EXMLAgreement.xyzw);
        Parsing(ref pIn_Temp, ref pIn_TempPos, contetnNode["Position"], out pIn_Base.SBaseData.Pos, EXMLAgreement.xyzw);
        Parsing(ref pIn_Temp, ref pIn_TempPos, contetnNode["Size"], out pIn_Base.SBaseData.Size, EXMLAgreement.wh);
    }


    private void ParsingSliderArray(XmlDocument vIn_xmlDoc, Transform vIn_Parent)
    {
        XmlNode sliderArray = vIn_xmlDoc.SelectSingleNode("InterfaceConfig/SliderArray");
        if (sliderArray == null) return;
        XmlNodeList sliderNodes = sliderArray.SelectNodes("Slider");
        TEN.GLOBAL.STRUCT.SSliderData SliderData = new TEN.GLOBAL.STRUCT.SSliderData();
        foreach (XmlNode item in sliderNodes)
        {
            SliderData.Reset();
            SliderData.Parent = vIn_Parent;
            ParsingSlider(item , SliderData);
            TEN.MANAGER.InstantiateManager.Instance.InstantiateSlider(SliderData);
        }
    }

    private bool ParsingSlider(XmlNode sliderNode, TEN.GLOBAL.STRUCT.SSliderData pIn_sSliderData)
    {
        int temp = 0;
        int tempPos = 0;
        pIn_sSliderData.Name = sliderNode["Name"].InnerText;
        pIn_sSliderData.ObjectName = sliderNode["Map"].Attributes["name"].Value;
        pIn_sSliderData.AttributeName = sliderNode["Map"].Attributes["attribute"].Value;
        TEN.GLOBAL.Global.BitManager.SetBitByBool(ref temp, ConvertString(out int type, sliderNode["Type"].InnerText), tempPos);
        if (TEN.GLOBAL.Global.BitManager.GetBit(temp, tempPos) == 0)
        {
            TEN.GLOBAL.Global.BitManager.SetBitByBool(ref temp, ConvertInt2Enum(out pIn_sSliderData.WindowType, type), ++tempPos);
        }
        TEN.GLOBAL.Global.BitManager.SetBitByBool(ref temp, ConvertString(out int mapType, sliderNode["Map"].Attributes["type"].InnerText), ++tempPos);
        if (TEN.GLOBAL.Global.BitManager.GetBit(temp, tempPos) == 0)
        {
            TEN.GLOBAL.Global.BitManager.SetBitByBool(ref temp, ConvertInt2Enum(out pIn_sSliderData.MapType, mapType), ++tempPos);
        }
        TEN.GLOBAL.Global.BitManager.SetBitByBool(ref temp, ConvertString(out pIn_sSliderData.SBaseData.Achor.x, sliderNode["Anchor"].Attributes["x"].Value), ++tempPos);
        TEN.GLOBAL.Global.BitManager.SetBitByBool(ref temp, ConvertString(out pIn_sSliderData.SBaseData.Achor.y, sliderNode["Anchor"].Attributes["y"].Value), ++tempPos);
        TEN.GLOBAL.Global.BitManager.SetBitByBool(ref temp, ConvertString(out pIn_sSliderData.SBaseData.Pos.x, sliderNode["Position"].Attributes["x"].Value), ++tempPos);
        TEN.GLOBAL.Global.BitManager.SetBitByBool(ref temp, ConvertString(out pIn_sSliderData.SBaseData.Pos.y, sliderNode["Position"].Attributes["y"].Value), ++tempPos);
        TEN.GLOBAL.Global.BitManager.SetBitByBool(ref temp, ConvertString(out pIn_sSliderData.SBaseData.Size.x, sliderNode["Size"].Attributes["width"].Value), ++tempPos);
        TEN.GLOBAL.Global.BitManager.SetBitByBool(ref temp, ConvertString(out pIn_sSliderData.SBaseData.Size.y, sliderNode["Size"].Attributes["height"].Value), ++tempPos);
        TEN.GLOBAL.Global.BitManager.SetBitByBool(ref temp, ConvertString(out pIn_sSliderData.MapRange.x, sliderNode["Map"].Attributes["min"].Value), ++tempPos);
        TEN.GLOBAL.Global.BitManager.SetBitByBool(ref temp, ConvertString(out pIn_sSliderData.MapRange.y, sliderNode["Map"].Attributes["max"].Value), ++tempPos);
        if (temp != 0)
        {
            Debug.Log($"XML Parsing Error , {System.Convert.ToString(temp, 2).PadLeft(32, '0')}");
            return false;
        }
        return true;
    }

    private void ParsingScrollViewData(XmlDocument vIn_xmlDoc, Transform vIn_Parent)
    {
        XmlNode scrollView = vIn_xmlDoc.SelectSingleNode("InterfaceConfig/ScrollView");
        if (scrollView == null) return;
        TEN.GLOBAL.STRUCT.SScrollViewData scrollViewData = new TEN.GLOBAL.STRUCT.SScrollViewData();
        int temp = 0;
        TEN.INSTANCE.InfiniteScrolling infiniteScrolling;

        scrollViewData.Parent = vIn_Parent;
        scrollViewData.Name = scrollView["Name"].InnerText;
        scrollViewData.BackroundImagePath = scrollView["Image"].Attributes["path"].Value;
        TEN.GLOBAL.Global.BitManager.SetBitByBool(ref temp, ConvertString(out int type, scrollView["Type"].InnerText), 0);
        if (TEN.GLOBAL.Global.BitManager.GetBit(temp, 0) == 0)
        {
            TEN.GLOBAL.Global.BitManager.SetBitByBool(ref temp, ConvertInt2Enum(out scrollViewData.WindowType, type), 1);
        }
        TEN.GLOBAL.Global.BitManager.SetBitByBool(ref temp, ConvertString(out scrollViewData.SBaseData.Achor.x, scrollView["Anchor"].Attributes["x"].Value), 2);
        TEN.GLOBAL.Global.BitManager.SetBitByBool(ref temp, ConvertString(out scrollViewData.SBaseData.Achor.y, scrollView["Anchor"].Attributes["y"].Value), 3);
        TEN.GLOBAL.Global.BitManager.SetBitByBool(ref temp, ConvertString(out scrollViewData.SBaseData.Pos.x, scrollView["Position"].Attributes["x"].Value), 4);
        TEN.GLOBAL.Global.BitManager.SetBitByBool(ref temp, ConvertString(out scrollViewData.SBaseData.Pos.y, scrollView["Position"].Attributes["y"].Value), 5);
        TEN.GLOBAL.Global.BitManager.SetBitByBool(ref temp, ConvertString(out scrollViewData.SBaseData.Size.x, scrollView["Size"].Attributes["width"].Value), 6);
        TEN.GLOBAL.Global.BitManager.SetBitByBool(ref temp, ConvertString(out scrollViewData.SBaseData.Size.y, scrollView["Size"].Attributes["height"].Value), 7);
        TEN.GLOBAL.Global.BitManager.SetBitByBool(ref temp, ConvertString(out scrollViewData.ContentSize.x, scrollView["ContentData"].Attributes["width"].Value), 8);
        TEN.GLOBAL.Global.BitManager.SetBitByBool(ref temp, ConvertString(out scrollViewData.ContentSize.y, scrollView["ContentData"].Attributes["height"].Value), 9);
        TEN.GLOBAL.Global.BitManager.SetBitByBool(ref temp, ConvertString(out scrollViewData.Horizontal, scrollView["State"].Attributes["Horizontal"].Value), 8);
        TEN.GLOBAL.Global.BitManager.SetBitByBool(ref temp, ConvertString(out scrollViewData.Vertical, scrollView["State"].Attributes["Vertical"].Value), 9);
        TEN.GLOBAL.Global.BitManager.SetBitByBool(ref temp, ConvertString(out int childType, scrollView["ChildType"].InnerText), 10);
        if (TEN.GLOBAL.Global.BitManager.GetBit(temp, 10) == 0)
        {
            TEN.GLOBAL.Global.BitManager.SetBitByBool(ref temp, ConvertInt2Enum(out scrollViewData.WindowType, childType), 11);
        }
        if (temp != 0)
        {
            Debug.Log($"XML Parsing Error , {System.Convert.ToString(temp, 2).PadLeft(32, '0')}");
        }
        infiniteScrolling = TEN.MANAGER.InstantiateManager.Instance.InstantiateScrollView(scrollViewData).GetComponent<TEN.INSTANCE.InfiniteScrolling>();
        // 获取 Content 节点
        XmlNodeList contentNodes = vIn_xmlDoc.SelectNodes("InterfaceConfig/ScrollView/Content");
        Debug.Log($"scrollViewData.WindowType {scrollViewData.WindowType}");
        switch (scrollViewData.WindowType)
        {
            case TEN.GLOBAL.ENUM.EWindowsType.BUTTON:
                ParsingButtonList(infiniteScrolling , contentNodes);
                break;
            default:
                break;
        }
        
    }

    void ParsingButtonList(TEN.INSTANCE.InfiniteScrolling pIn_InfiniteScrolling , XmlNodeList pIn_ContentNodes)
    {
        System.Collections.Generic.List<TEN.GLOBAL.STRUCT.SButtonData> contentData = new System.Collections.Generic.List<TEN.GLOBAL.STRUCT.SButtonData>();
        foreach (XmlNode contetnNode in pIn_ContentNodes)
        {
            TEN.GLOBAL.STRUCT.SButtonData sButtonData = new TEN.GLOBAL.STRUCT.SButtonData
            {
                Parent = pIn_InfiniteScrolling.GetContent()
            };
            ParsingButton(contetnNode, sButtonData);
            contentData.Add(sButtonData);
            //TEN.MANAGER.InstantiateManager.Instance.InstantiateButton(sButtonData);
        }
        pIn_InfiniteScrolling.SetContent(contentData);
    }

    bool ParsingButton(XmlNode contetnNode , TEN.GLOBAL.STRUCT.SButtonData pIn_sButtonData)
    {
        int temp = 0;
        pIn_sButtonData.Name = contetnNode["Name"].InnerText;
        pIn_sButtonData.BackroundImagePath = contetnNode["Image"].Attributes["path"].Value;
        TEN.GLOBAL.Global.BitManager.SetBitByBool(ref temp, ConvertString(out int type, contetnNode["Type"].InnerText), 0);
        if (TEN.GLOBAL.Global.BitManager.GetBit(temp, 0) == 0)
        {
            TEN.GLOBAL.Global.BitManager.SetBitByBool(ref temp, ConvertInt2Enum(out pIn_sButtonData.WindowType, type), 1);
        }
        TEN.GLOBAL.Global.BitManager.SetBitByBool(ref temp, ConvertString(out pIn_sButtonData.SBaseData.Achor.x, contetnNode["Anchor"].Attributes["x"].Value), 2);
        TEN.GLOBAL.Global.BitManager.SetBitByBool(ref temp, ConvertString(out pIn_sButtonData.SBaseData.Achor.y, contetnNode["Anchor"].Attributes["y"].Value), 3);
        TEN.GLOBAL.Global.BitManager.SetBitByBool(ref temp, ConvertString(out pIn_sButtonData.SBaseData.Pos.x, contetnNode["Position"].Attributes["x"].Value), 4);
        TEN.GLOBAL.Global.BitManager.SetBitByBool(ref temp, ConvertString(out pIn_sButtonData.SBaseData.Pos.y, contetnNode["Position"].Attributes["y"].Value), 5);
        TEN.GLOBAL.Global.BitManager.SetBitByBool(ref temp, ConvertString(out pIn_sButtonData.SBaseData.Size.x, contetnNode["Size"].Attributes["width"].Value), 6);
        TEN.GLOBAL.Global.BitManager.SetBitByBool(ref temp, ConvertString(out pIn_sButtonData.SBaseData.Size.y, contetnNode["Size"].Attributes["height"].Value), 7);
        pIn_sButtonData.EventName = contetnNode["Event"].Attributes["name"].Value;
        pIn_sButtonData.EventParameter = contetnNode["Event"].Attributes["parameter"].Value;
        if (temp != 0)
        {
            Debug.Log($"XML Parsing Error , {System.Convert.ToString(temp, 2).PadLeft(32, '0')}");
            return false;
        }
        return true;
    }
}
