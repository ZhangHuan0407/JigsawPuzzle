using JigsawPuzzle.Models;
using System;
using System.IO;
using System.Text;
using System.Web;
using System.Web.SessionState;

namespace JigsawPuzzle.Analysis
{
    public class Log
    {
        /* field */
        private readonly StringBuilder Builder;
        private readonly object LockFactor;
        internal readonly FileInfo LogFile;

        /* ctor */
        public Log(string fileName)
        {
            Builder = new StringBuilder();
            LockFactor = new object();
            LogFile = new FileInfo($"{PortConfig.Value.DataDirectory}/{nameof(Log)}/{fileName}");
        }

        /* func */
        public void WriteData(HttpSessionState httpSession, params object[] data)
        {
            lock (LockFactor)
            {
                Builder.AppendLine($"SessionID : {httpSession?.SessionID} Time : {DateTime.Now}");
                foreach (object item in data)
                    Builder.Append(item).Append("\t");
                Builder.AppendLine();
                Builder.AppendLine();
            }
            if (Builder.Length > 8000)
                WriteOut();
        }
        public void WriteOut()
        {
            lock (LockFactor)
            {
                using (FileStream fileStream = new FileStream(LogFile.FullName, FileMode.Append, FileAccess.Write))
                {
                    byte[] buffer = Encoding.UTF8.GetBytes(Builder.ToString());
                    Builder.Clear();
                    fileStream.Write(buffer, 0, buffer.Length);
                }
            }
        }

        public static Log CreateAppLog()
        {
            Log log = new Log($"{DateTime.Now:yyyy_MM_dd}.txt");
            log.LogFile.Directory.Create();
            return log;
        }
    }
}