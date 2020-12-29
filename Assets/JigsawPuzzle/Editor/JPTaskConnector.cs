using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

#if UNITY_EDITOR
using UnityEngine;
#else
using Newtonsoft.Json;
#endif

namespace JigsawPuzzle
{
    /// <summary>
    /// 与 JigsawPuzzle MVC 项目交互的任务连接器，可以由 Unity 或者 MVC 发起
    /// <para>应当尽可能少的持有此类型实例，仅负载均衡服务器可以持有复数实例</para>
    /// </summary>
    [ShareScript]
    public sealed class JPTaskConnector : IEnumerable<ControllerAction>
    {
        /* const */
#if UNITY_EDITOR
        /// <summary>
        /// Unity 项目内服务器公开路由配置
        /// </summary>
        public const string Path = "Assets/Editor Default Resources/JigsawPuzzle/ServerRouteConfig.json";
#else
        /// <summary>
        /// MVC 项目内服务器公开路由配置
        /// </summary>
        public const string Path = "~/App_Data/ServerRouteConfig.json";
#endif

        /* field */
        internal readonly ServerRouteConfig ServerRouteConfig;
        private readonly HttpClient Client;

        private static readonly Dictionary<string, Func<object, HttpContent>> ObjectToHttpContent = new Dictionary<string, Func<object, HttpContent>>()
        {
            { "System.String", StringToContent },
        };

        private static readonly Dictionary<string, Func<HttpContent, object>> HttpContentToObject = new Dictionary<string, Func<HttpContent, object>>()
        {
            { "System.String", ContentToString },
        };

        /* ctor */
        /// <summary>
        /// 创建新的 <see cref="JPTaskConnector"/> 实例，承担 JigsawPuzzle 数据传输任务与远程访问
        /// </summary>
        /// <param name="serverRouteContent">发送请求时使用的 Uri 地址</param>
        /// <param name="timeoutSeconds">超时时间，默认 5 秒</param>
        public JPTaskConnector(string serverRouteContent, int timeoutSeconds = 5)
        {
            if (string.IsNullOrWhiteSpace(serverRouteContent))
                throw new ArgumentException($"“{nameof(serverRouteContent)}”不能为 Null 或空白", nameof(serverRouteContent));

#if UNITY_EDITOR
            // Bugnity 读 json 数据的时候，如果有注释居然会报错 JSON parse error: Invalid value??
            // Unity 这么大公司，你们的模板数据里面一行注释都没有??
            // 你们代码可读性有0分吗?
            // 目前支持单行注释
            StringBuilder builder = new StringBuilder();
            foreach (string line in serverRouteContent.Split('\n'))
            {
                if (line.TrimStart().StartsWith("//"))
                    continue;
                else
                    builder.AppendLine(line);
            }
            serverRouteContent = builder.ToString();
            ServerRouteConfig = JsonUtility.FromJson<ServerRouteConfig>(serverRouteContent);
            // OnAfterDeserialize Invoke by unity
#else
            ServerRouteConfig = JsonConvert.DeserializeObject<ServerRouteConfig>(serverRouteContent);
            ServerRouteConfig.OnAfterDeserialize();
#endif

            Client = new HttpClient()
            {
                BaseAddress = ServerRouteConfig.BaseAddressUri,
                MaxResponseContentBufferSize = 20 * 1024 * 1024,
                Timeout = TimeSpan.FromSeconds(timeoutSeconds),
            };
        }

        /// <summary>
        /// 发送一个受到监视的 Get 请求，如果传输的数据格式不正确，不会执行回掉
        /// <para>如果需要一个明确的任务结束标记，可以等待返回任务结束</para>
        /// </summary>
        /// <param name="controller">MVC 公开控制器</param>
        /// <param name="action">MVC 公开 HttpGet 行为</param>
        /// <param name="success">服务器回复数据，检查通过</param>
        /// <param name="failed">链接中出现错误，或服务器返回执行不通过的状态码</param>
        /// <returns>执行任务</returns>
        public Task Get(string controller, string action,
            Action<object> success, 
            Action<HttpResponseMessage> failed = null)
        {
            if (string.IsNullOrWhiteSpace(controller))
                throw new ArgumentException($"{nameof(controller)}Can not be Null or white space", nameof(controller));
            if (string.IsNullOrWhiteSpace(action))
                throw new ArgumentException($"{nameof(action)}Can not be Null or white space", nameof(action));
            ControllerAction controllerAction = ServerRouteConfig[controller, action];
            
            if (controllerAction is null)
                throw new Exception($"Not found {controller}/{action} in {nameof(ServerRouteConfig)}");
            else if (!controllerAction.Type.Equals("HttpGet"))
                throw new Exception($"{controller}/{action} type is not equal.");

            Task<HttpResponseMessage> responseMessage = Client.GetAsync($"{controller}/{action}");
            return Task.Run(() => 
            {
                try
                {
                    responseMessage.Wait();
                    object resultObject = null;
                    if (responseMessage.Result.IsSuccessStatusCode)
                    {
                        resultObject = HttpContentToObject[controllerAction.ReturnType](responseMessage.Result.Content);
                        success?.Invoke(resultObject);
                    }
                    else
                    {
                        Action<HttpResponseMessage> copy = failed;
                        failed = null;
                        copy?.Invoke(responseMessage.Result);
                    }
                }
                catch (Exception e)
                {
                    // Log : Controller/Action
#if UNITY_EDITOR
                    Debug.LogError($"{e.Message}\n{e.StackTrace}");
#else
                    Console.WriteLine($"{e.Message}\n{e.StackTrace}");
#endif
                }
                finally
                {
                    failed?.Invoke(responseMessage?.Result);
                }
            });
        }
        /// <summary>
        /// 发送一个受到监视的 Post 请求，如果传输的数据格式不正确，不会执行回掉
        /// <para>如果需要一个明确的任务结束标记，可以等待返回任务结束</para>
        /// </summary>
        /// <param name="controller">MVC 公开控制器</param>
        /// <param name="action">MVC 公开 HttpPost 行为</param>
        /// <param name="data">发送的数据</param>
        /// <param name="success">服务器回复数据，检查通过</param>
        /// <param name="failed">链接中出现错误，或服务器返回执行不通过的状态码</param>
        /// <returns>执行任务</returns>
        public void Post(string controller, string action,
            Dictionary<string, object> data,
            Action<object> success = null,
            Action<HttpResponseMessage> failed = null)
        {
            if (string.IsNullOrWhiteSpace(controller))
                throw new ArgumentException($"{nameof(controller)}Can not be Null or white space", nameof(controller));
            if (string.IsNullOrWhiteSpace(action))
                throw new ArgumentException($"{nameof(action)}Can not be Null or white space", nameof(action));
            if (data is null)
                throw new ArgumentNullException(nameof(data));

            ControllerAction controllerAction = ServerRouteConfig[controller, action];
            if (controllerAction is null)
                throw new Exception($"Not found {controller}/{action} in {nameof(ServerRouteConfig)}");
            else if (!controllerAction.Type.Equals("HttpPost"))
                throw new Exception($"{controller}/{action} type is not equal.");

            Task.Run(() =>
            {
                Task<HttpResponseMessage> responseMessage = null;
                try
                {
                    MultipartFormDataContent form = new MultipartFormDataContent();
                    for (int index = 0; index < controllerAction.FormKeys.Length; index++)
                    {
                        string itemName = controllerAction.FormKeys[index];
                        if (!data.TryGetValue(itemName, out object obj))
                            throw new ArgumentNullException($"{itemName} is null.");
                        string itemType = controllerAction.FormValues[index];
                        form.Add(ObjectToHttpContent[itemType](obj), itemName);
                    }
                    responseMessage = Client.PostAsync($"{controller}/{action}", form);

                    responseMessage.Wait();
                    object resultObject = null;
                    if (responseMessage.Result.IsSuccessStatusCode)
                    {
                        resultObject = HttpContentToObject[controllerAction.ReturnType](responseMessage.Result.Content);
                        success?.Invoke(resultObject);
                    }
                    else
                    {
                        Action<HttpResponseMessage> copy = failed;
                        failed = null;
                        copy?.Invoke(responseMessage.Result);
                    }
                }
                catch (Exception e)
                {
                    // Log : Controller/Action
#if UNITY_EDITOR
                    Debug.LogError($"{e.Message}\n{e.StackTrace}");
#else
                    Console.WriteLine($"{e.Message}\n{e.StackTrace}");
#endif
                }
                finally
                {
                    failed?.Invoke(responseMessage?.Result);
                }
            });
        }

        private static HttpContent StringToContent(object obj)
        {
            if (obj is string str)
                return new StringContent(str);
            else
                throw new ArgumentException($"Argument type error, {nameof(obj)} :{obj?.GetType().FullName}");
        }
        private static object ContentToString(HttpContent httpContent)
        {
            if (httpContent is StringContent stringContent)
                return stringContent.ReadAsStringAsync().Result;
            else
                throw new ArgumentException($"Argument type error, {nameof(httpContent)} :{httpContent?.GetType().FullName}");
        }

        /* IEnumerable */
        public IEnumerator<ControllerAction> GetEnumerator()
        {
            foreach (ControllerAction controllerAction in ServerRouteConfig.WebAPI)
                yield return controllerAction;
        }
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    }
}