using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;

namespace TEN
{
    public partial class PaseMaster
    {
        public static string WritePath = "D:\\Z\\Work\\py\\TTT.txt";
        public static void WriteList(List<Vector3Int> list)
        {
            using (FileStream fs = File.OpenWrite(WritePath))
            {
                StringBuilder sb = new StringBuilder();
                foreach (var item in list)
                {
                    sb.Append($"{item.x},{item.y},{item.z}\n");
                }
                byte[] bytes = Encoding.UTF8.GetBytes(sb.ToString());
                fs.Write(bytes, 0, bytes.Length);
            }
        }


        public static void WriteHashSet(HashSet<Vector3Int> list)
        {
            using (FileStream fs = File.OpenWrite(WritePath))
            {
                StringBuilder sb = new StringBuilder();
                foreach (var item in list)
                {
                    sb.Append($"{item.x},{item.y},{item.z}\n");
                }
                byte[] bytes = Encoding.UTF8.GetBytes(sb.ToString());
                fs.Write(bytes, 0, bytes.Length);
            }
        }
    }
}
