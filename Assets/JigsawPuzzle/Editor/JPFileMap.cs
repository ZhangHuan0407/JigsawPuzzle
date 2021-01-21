using System;
using System.Collections.Generic;
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
        public string[] ClientTask;

        /* func */
        public static string GetFileName(string fileName)
        {
            int index = fileName.IndexOf('.');
            if (index == -1)
                return fileName;
            else
                return fileName.Substring(0, index);
        }

        /* operator */
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder()
                .AppendLine($"{nameof(Task)} Length : {Task?.Length}");
            foreach (string fileName in Task ?? new string[0])
                builder.AppendLine($"    {nameof(fileName)} : {fileName}");
            return builder.ToString();
        }
    }
}