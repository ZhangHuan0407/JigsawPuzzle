using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Web;

namespace JigsawPuzzle.Models
{
    public class PortConfig
    {
        /* const */
        public const string Path = "~/App_Data/PortConfig.json";

        public static PortConfig Value { get; private set; }

        /* field */
        /// <summary>
        /// 端口自称，此名称在工作组中应当确定唯一
        /// </summary>
        public string PortName { get; set; }
        /// <summary>
        /// 数据文件夹路径
        /// <para>配置文件中写入相对路径，读取时转为绝对路径</para>
        /// </summary>
        public string DataDirectory { get; set; }
        /// <summary>
        /// 同时在运行的最大并发任务数
        /// </summary>
        [Obsolete("当前此字段不具有实际功能")]
        public int MaxOfConcurrentTasks { get; set; }
        /// <summary>
        /// 进程自我约束的内存上限
        /// </summary>
        [Obsolete("当前此字段不具有实际功能")]
        public int MaxOfProcessMemory { get; set; }
        /// <summary>
        /// 服务器 IPv4 地址
        /// </summary>
        public IPAddress ServerIPAddress { get; private set; }
        /// <summary>
        /// 服务器 IPv6 地址
        /// </summary>
        public IPAddress ServerIPAddressV6 { get; private set; }

        /* func */
        public static void ReloadPortConfig()
        {
            string path = HttpContext.Current.Server.MapPath(Path);
            if (!File.Exists(path))
                throw new FileNotFoundException($"The key file \"{path}\" not found.");
            
            string jsonContent = File.ReadAllText(path);
            Value = JsonConvert.DeserializeObject<PortConfig>(jsonContent);
            if (Value.MaxOfConcurrentTasks < 1)
                throw new ArgumentException(nameof(MaxOfConcurrentTasks));
            if (Value.MaxOfProcessMemory < 100)
                throw new ArgumentException(nameof(MaxOfConcurrentTasks));
            Value.DataDirectory = HttpContext.Current.Server.MapPath(Value.DataDirectory);
            Directory.CreateDirectory(Value.DataDirectory);

            foreach (IPAddress iPAddress in Dns.GetHostAddresses(Dns.GetHostName()))
            {
                if (iPAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    Value.ServerIPAddress = iPAddress;
                else if (iPAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
                    Value.ServerIPAddressV6 = iPAddress;
            }
        }

        /* operator */
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder()
                .AppendLine($"{nameof(PortName)} : {PortName}")
                .AppendLine($"{nameof(DataDirectory)} : {DataDirectory}")
                .AppendLine($"{nameof(MaxOfConcurrentTasks)} : {MaxOfConcurrentTasks}")
                .AppendLine($"{nameof(MaxOfProcessMemory)} : {MaxOfProcessMemory}")
                .AppendLine($"{nameof(ServerIPAddress)} : {ServerIPAddress}")
                .AppendLine($"{nameof(ServerIPAddressV6)} : {ServerIPAddressV6}");
            return builder.ToString();
        }
    }
}