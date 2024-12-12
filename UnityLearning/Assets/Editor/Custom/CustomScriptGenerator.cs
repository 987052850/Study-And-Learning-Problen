using UnityEditor;
using UnityEngine;
using System.IO;

public class CustomScriptGenerator : UnityEditor.AssetModificationProcessor
{
    const string Author = "Michael Corleone";
    public static void OnWillCreateAsset(string path)
    {
        // 检查是否是 C# 脚本
        path = path.Replace(".meta", "");
        if (path.EndsWith(".cs"))
        {
            string fullPath = Path.GetFullPath(path);
            string content = File.ReadAllText(fullPath);

            // 替换 #BBB# 为当前日期
            content = content.Replace("#AAA#", "TEN");
            content = content.Replace("#BBB#", System.DateTime.Now.ToString("G"));
            content = content.Replace("#CCC#", Author);
            // 写回文件
            File.WriteAllText(fullPath, content);

            // 刷新 AssetDatabase
            AssetDatabase.Refresh();
        }
    }
}
