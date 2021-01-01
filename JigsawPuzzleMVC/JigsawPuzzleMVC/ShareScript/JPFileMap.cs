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
        /// Task 文件夹下
        /// </summary>
        public string[] Task;


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