using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

#if UNITY_EDITOR
using UnityEngine;
#else
using Newtonsoft.Json;
#endif

namespace JigsawPuzzle
{
    [ShareScript]
    public sealed class JPTaskConnector
    {
        /* const */
#if UNITY_EDITOR
        public const string Path = "Assets/Editor Default Resources/JigsawPuzzle/ServerRouteConfig.json";
#else
        public const string Path = "~/App_Data/ServerRouteConfig.json";
#endif

        /* field */
        private readonly ServerRouteConfig ServerRoute;
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
        public JPTaskConnector(string serverRouteConfig, int timeoutSeconds = 5)
        {
            if (string.IsNullOrWhiteSpace(serverRouteConfig))
                throw new ArgumentException($"“{nameof(serverRouteConfig)}”不能为 Null 或空白", nameof(serverRouteConfig));

#if UNITY_EDITOR
            ServerRoute = JsonUtility.FromJson<ServerRouteConfig>(serverRouteConfig);
#else
            ServerRoute = JsonConvert.DeserializeObject<ServerRouteConfig>(serverRouteConfig);
#endif
            Client = new HttpClient()
            {
                BaseAddress = ServerRoute.BaseAddress,
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
            ControllerAction controllerAction = ServerRoute[controller, action];
            // get check
            if (controllerAction is null)
                throw new Exception($"Not found {controller}/{action} in {nameof(ServerRoute)}");

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

            ControllerAction controllerAction = ServerRoute[controller, action];
            if (controllerAction is null)
                throw new Exception($"Not found {controller}/{action} in {nameof(ServerRoute)}");

            Task.Run(() =>
            {
                bool needCallback = false;
                Task<HttpResponseMessage> responseMessage = null;
                try
                {
                    MultipartFormDataContent form = new MultipartFormDataContent();
                    foreach (KeyValuePair<string, string> parameterInfo in controllerAction)
                    {
                        if (!data.TryGetValue(parameterInfo.Key, out object obj))
                            throw new ArgumentNullException($"{parameterInfo.Key} is null.");
                        form.Add(ObjectToHttpContent[parameterInfo.Value](obj), parameterInfo.Key);
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
    }
}