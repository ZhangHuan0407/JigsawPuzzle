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
    [ShareScript]
    public sealed class JPTaskConnector : IEnumerable<ControllerAction>
    {
        /* const */
#if UNITY_EDITOR
        public const string Path = "Assets/Editor Default Resources/JigsawPuzzle/ServerRouteConfig.json";
#else
        public const string Path = "~/App_Data/ServerRouteConfig.json";
#endif

        /* field */
        internal readonly ServerRouteConfig ServerRouteConfig;
        private readonly HttpClient Client;

        private static readonly Dictionary<string, Func<object, HttpContent>> ObjectToHttpContent = new Dictionary<string, Func<object, HttpContent>>()
        {
            { "System.String", StringToContent },
        };

        /* ctor */
        /// <summary>
        /// 创建新的 <see cref="JPTaskConnector"/> 实例，承担 JigsawPuzzle 数据传输任务与远程访问
        /// </summary>
        /// <param name="strUri">发送请求时使用的 Uri 地址</param>
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

        public Task Get(string controller, string action,
            Action<HttpResponseMessage> success = null, 
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
                bool needCallback = true;
                try
                {
                    responseMessage.Wait();
                    needCallback = false;
                    if (responseMessage.IsCompleted)
                        success?.Invoke(responseMessage.Result);
                    else
                        failed?.Invoke(responseMessage.Result);
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
                    if (needCallback)
                        failed?.Invoke(responseMessage.Result);
                }
            });
        }
        public void Post(string controller, string action,
            Dictionary<string, object> data,
            Action<HttpResponseMessage> success = null,
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
                bool needCallback = false;
                Task<HttpResponseMessage> responseMessage = null;
                try
                {
                    MultipartFormDataContent form = new MultipartFormDataContent();
                    for (int index = 0; index < controllerAction.FormKeys.Length; index++)
                    {
                        if (!data.TryGetValue(controllerAction.FormKeys[index], out object obj))
                            throw new ArgumentNullException($"{controllerAction.FormKeys[index]} is null.");
                        form.Add(ObjectToHttpContent[controllerAction.FormValues[index]](obj), controllerAction.FormKeys[index]);
                    }
                    responseMessage = Client.PostAsync($"{controller}/{action}", form);

                    needCallback = true;
                    responseMessage.Wait();
                    needCallback = false;
                    if (responseMessage.IsCompleted)
                        success?.Invoke(responseMessage.Result);
                    else
                        failed?.Invoke(responseMessage.Result);
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
                    if (needCallback && responseMessage != null)
                        failed?.Invoke(responseMessage.Result);
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

        /* IEnumerable */
        public IEnumerator<ControllerAction> GetEnumerator()
        {
            foreach (ControllerAction controllerAction in ServerRouteConfig.WebAPI)
                yield return controllerAction;
        }
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    }
}