using System;
using System.Text;

namespace JigsawPuzzle
{
    [ShareScript]
    [Serializable]
    public class JPFileMap
    {
        /* field */
        /// <summary>
        /// 远程服务器 Task 文件夹下
        /// </summary>
        public string[] Task;

        /// <summary>
        /// 本地 Task 文件夹下
        /// </summary>
        [NonSerialized]
        public string[] Client;

        /* operator */
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder()
                .AppendLine($"{nameof(Task)} Length : {Task?.Length}");
            foreach (string fileName in Task)
                builder.AppendLine($"    {nameof(fileName)} : {fileName}");
            return builder.ToString();
        }
    }
}