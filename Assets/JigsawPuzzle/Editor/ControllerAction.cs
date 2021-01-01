using System;
using System.Reflection;
using System.Text;

#if DEBUG && MVC
using System.Web.Mvc;
using System.Reflection;
using JigsawPuzzle.Analysis;
#endif

namespace JigsawPuzzle
{
    /// <summary>
    /// 控制器行为代表一个服务器可访问调用
    /// </summary>
    [ShareScript]
    [Serializable]
    public class ControllerAction
    {
        /* field */
        /// <summary>
        /// 行为名称
        /// </summary>
        public string Action;
        /// <summary>
        /// 控制器名称
        /// </summary>
        public string Controller;
        /// <summary>
        /// 仅在 Debug 模式下可用
        /// </summary>
        public bool DebugOnly;
        /// <summary>
        /// 请求表单数据项目名称
        /// </summary>
        public string[] FormKeys;
        /// <summary>
        /// 请求表单数据值类型
        /// </summary>
        public string[] FormValues;
        /// <summary>
        /// 返回值类型
        /// </summary>
        public string ReturnType;
        /// <summary>
        /// 请求类型
        /// </summary>
        public string Type;

        /* ctor */
        public ControllerAction() { }
#if DEBUG && MVC
        public ControllerAction(WebAPIAttribute webAPI)
        {
            Controller = webAPI.BindMethodInfo.DeclaringType.Name;
            Controller = Controller.Substring(0, Controller.IndexOf("Controller"));
            Action = webAPI.BindMethodInfo.Name;
            DebugOnly = webAPI.DebugOnly;
            if (webAPI.BindMethodInfo.GetCustomAttribute<HttpGetAttribute>() != null)
                Type = "HttpGet";
            else if (webAPI.BindMethodInfo.GetCustomAttribute<HttpPostAttribute>() != null)
                Type = "HttpPost";
            else
                Type = "Error";
        }
#endif
        
        /* func */
        public Type GetSerializedReturnType()
        {
            Type returnType = Assembly.GetExecutingAssembly().GetType(ReturnType, false);
            if (returnType is null
                || returnType.GetCustomAttribute<SerializableAttribute>(false) != null
                || returnType.GetCustomAttribute<ShareScriptAttribute>(false) != null)
                return null;
            else
                return returnType;
        }

        /* operator */
        /// <summary>
        /// 获取控制器路由的摘要信息
        /// </summary>
        /// <returns>摘要信息</returns>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder()
                .AppendLine($"{nameof(Action)} : {Action}")
                .AppendLine($"{nameof(Controller)} : {Controller}")
                .AppendLine($"{nameof(DebugOnly)} : {DebugOnly}")
                .AppendLine($"{nameof(FormKeys)} : {FormKeys} {FormKeys?.Length}")
                .AppendLine($"{nameof(FormValues)} : {FormValues} {FormValues?.Length}")
                .AppendLine($"{nameof(ReturnType)} : {ReturnType}")
                .AppendLine($"{nameof(Type)} : {Type}");
            return builder.ToString();
        }
    }
}