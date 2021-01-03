using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Reflection;
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
        internal readonly string UserName;

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

            ServerRouteConfig = JsonFuck.FromJsonToObject<ServerRouteConfig>(serverRouteContent);

            Client = new HttpClient()
            {
                BaseAddress = ServerRouteConfig.BaseAddressUri,
                MaxResponseContentBufferSize = 20 * 1024 * 1024,
                Timeout = TimeSpan.FromSeconds(timeoutSeconds),
            };

            UserName = Dns.GetHostName();
        }

        /* func */
        /// <summary>
        /// 发送一个受到监视的 Get 请求，如果传输的数据格式不正确，不会执行回掉
        /// <para>如果需要一个明确的任务结束标记，可以等待返回任务结束</para>
        /// </summary>
        /// <param name="action">MVC 公开 HttpGet 行为</param>
        /// <param name="controller">MVC 公开控制器</param>
        /// <param name="success">服务器回复数据，检查通过</param>
        /// <param name="failed">链接中出现错误，或服务器返回执行不通过的状态码</param>
        /// <returns>执行任务</returns>
        public Task Get(string action, string controller,
            Action<object> success,
            Action<HttpResponseMessage> failed = null)
        {
            ControllerAction controllerAction = ServerRouteConfig.GetControllerAction(controller, action, "HttpGet");

            Task<HttpResponseMessage> responseMessage = Client.GetAsync($"{controller}/{action}");
            return Task.Run(() =>
            {
                bool needCallback = false;
                try
                {
                    responseMessage.Wait();
                    needCallback = true;
                    object resultObject = null;
                    if (!responseMessage.Result.IsSuccessStatusCode)
                        goto FailCallback;
                    else if (HttpContentConverter.HttpContentToObject.TryGetValue(controllerAction.ReturnType, out Func<HttpContent, object> converter))
                    {
                        resultObject = converter(responseMessage.Result.Content);
                        goto SuccessCallback;
                    }

                    Type returnType = controllerAction.GetSerializedReturnType();
                    if (returnType == null)
                        throw new ArgumentNullException($"{controllerAction.ReturnType} is not defined converter.");
                    else
                    {
                        string jsonResult = responseMessage.Result.Content.ReadAsStringAsync().Result;
                        resultObject = JsonFuck.FromJsonToObject(jsonResult, returnType);
                        goto SuccessCallback;
                    }

                SuccessCallback:
                    if (needCallback)
                    {
                        needCallback = false;
                        success?.Invoke(resultObject);
                    }
                FailCallback:
                    if (needCallback)
                    {
                        needCallback = false;
                        failed?.Invoke(responseMessage.Result);
                    }
                }
                catch (Exception e)
                {
                    // Log : Controller/Action
#if UNITY_EDITOR
                    Debug.LogError($"{controllerAction.Controller}/{controllerAction.Action}\n{e.Message}\n{e.StackTrace}");
#else
                    Console.WriteLine($"{e.Message}\n{e.StackTrace}");
#endif
                }
                finally
                {
                    if (needCallback)
                    {
                        needCallback = false;
                        failed?.Invoke(responseMessage?.Result);
                    }
                }
            });
        }
        /// <summary>
        /// 发送一个受到监视的 Post 请求，如果传输的数据格式不正确，不会执行回掉
        /// <para>如果需要一个明确的任务结束标记，可以等待返回任务结束</para>
        /// </summary>
        /// <param name="action">MVC 公开 HttpPost 行为</param>
        /// <param name="controller">MVC 公开控制器</param>
        /// <param name="data">发送的数据</param>
        /// <param name="success">服务器回复数据，检查通过</param>
        /// <param name="failed">链接中出现错误，或服务器返回执行不通过的状态码</param>
        /// <returns>执行任务</returns>
        public void PostForm(string action, string controller,
            Dictionary<string, object> data,
            Action<object> success = null,
            Action<HttpResponseMessage> failed = null)
        {
            ControllerAction controllerAction = ServerRouteConfig.GetControllerAction(controller, action, "HttpPost");
            if (data is null)
                throw new ArgumentNullException(nameof(data));

            Task.Run(() =>
            {
                bool needCallback = false;
                Task<HttpResponseMessage> responseMessage = null;
                try
                {
                    MultipartFormDataContent form = new MultipartFormDataContent();
                    for (int index = 0; index < controllerAction.FormKeys.Length; index++)
                    {
                        string itemName = controllerAction.FormKeys[index];
                        if (!data.TryGetValue(itemName, out object obj)
                            || obj is null)
                            throw new ArgumentNullException($"{itemName} is null.");
                        string itemType = controllerAction.FormValues[index];
                        if (HttpContentConverter.ObjectToHttpContent.TryGetValue(itemType, out Func<object, IEnumerable<HttpContent>> converter))
                        {
                            foreach (HttpContent content in converter(obj))
                                form.Add(content, itemName);
                            continue;
                        }

                        Type objType = obj.GetType();
                        if (objType.GetCustomAttribute<SerializableAttribute>(false) == null
                            || objType.GetCustomAttribute<ShareScriptAttribute>(false) == null)
                            throw new ArgumentNullException($"{itemType} is not defined converter.");
                        else
                        {
                            string content = JsonFuck.FromObjectToJson(obj);
                            form.Add(new StringContent(content), itemName);
                            continue;
                        }
                    }
                    responseMessage = Client.PostAsync($"{controller}/{action}", form);
                    responseMessage.Wait();
                    needCallback = true;
                    object resultObject = null;
                    if (!responseMessage.Result.IsSuccessStatusCode)
                        goto FailCallback;
                    else if (HttpContentConverter.HttpContentToObject.TryGetValue(controllerAction.ReturnType, out Func<HttpContent, object> converter))
                    {
                        resultObject = converter(responseMessage.Result.Content);
                        goto SuccessCallback;
                    }

                    Type returnType = controllerAction.GetSerializedReturnType();
                    if (returnType == null)
                        throw new ArgumentNullException($"{controllerAction.ReturnType} is not defined converter.");
                    else
                    {
                        string jsonResult = responseMessage.Result.Content.ReadAsStringAsync().Result;
                        resultObject = JsonFuck.FromJsonToObject(jsonResult, returnType);
                        goto SuccessCallback;
                    }

                SuccessCallback:
                    if (needCallback)
                    {
                        needCallback = false;
                        success?.Invoke(resultObject);
                    }
                FailCallback:
                    if (needCallback)
                    {
                        needCallback = false;
                        failed?.Invoke(responseMessage.Result);
                    }
                }
                catch (Exception e)
                {
                    // Log : Controller/Action
#if UNITY_EDITOR
                    Debug.LogError($"{controllerAction.Controller}/{controllerAction.Action}\n{e.Message}\n{e.StackTrace}");
#else
                    Console.WriteLine($"{e.Message}\n{e.StackTrace}");
#endif
                }
                finally
                {
                    if (needCallback)
                    {
                        needCallback = false;
                        failed?.Invoke(responseMessage?.Result);
                    }
                }
            });
        }
        public void PostFile(string controller, string action,
            byte[] binData, string name, string fileName,
            Action<object> success = null,
            Action<HttpResponseMessage> failed = null)
        {
            ControllerAction controllerAction = ServerRouteConfig.GetControllerAction(controller, action, "HttpPost");
            if (binData is null)
                throw new ArgumentNullException(nameof(binData));
            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentException($"{nameof(fileName)}Can not be Null or white space", nameof(fileName));

            Task.Run(() =>
            {
                bool needCallback = false;
                Task<HttpResponseMessage> responseMessage = null;
                try
                {
                    MultipartFormDataContent form = new MultipartFormDataContent
                    {
                        { new ByteArrayContent(binData), name, fileName }
                    };
                    responseMessage = Client.PostAsync($"{controller}/{action}", form);
                    responseMessage.Wait();
                    needCallback = true;
                    object resultObject = null;
                    if (!responseMessage.Result.IsSuccessStatusCode)
                        goto FailCallback;
                    else if (HttpContentConverter.HttpContentToObject.TryGetValue(controllerAction.ReturnType, out Func<HttpContent, object> converter))
                    {
                        resultObject = converter(responseMessage.Result.Content);
                        goto SuccessCallback;
                    }

                    Type returnType = controllerAction.GetSerializedReturnType();
                    if (returnType == null)
                        throw new ArgumentNullException($"{controllerAction.ReturnType} is not defined converter.");
                    else
                    {
                        string jsonResult = responseMessage.Result.Content.ReadAsStringAsync().Result;
                        resultObject = JsonFuck.FromJsonToObject(jsonResult, returnType);
                        goto SuccessCallback;
                    }

                SuccessCallback:
                    if (needCallback)
                    {
                        needCallback = false;
                        success?.Invoke(resultObject);
                    }
                FailCallback:
                    if (needCallback)
                    {
                        needCallback = false;
                        failed?.Invoke(responseMessage.Result);
                    }
                }
                catch (Exception e)
                {
                    // Log : Controller/Action
#if UNITY_EDITOR
                    Debug.LogError($"{controllerAction.Controller}/{controllerAction.Action}\n{e.Message}\n{e.StackTrace}");
#else
                    Console.WriteLine($"{e.Message}\n{e.StackTrace}");
#endif
                }
                finally
                {
                    if (needCallback)
                    {
                        needCallback = false;
                        failed?.Invoke(responseMessage?.Result);
                    }
                }
            });
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