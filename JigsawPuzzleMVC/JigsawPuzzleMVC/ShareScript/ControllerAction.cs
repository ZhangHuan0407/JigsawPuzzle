using System;
using System.Text;

#if DEBUG && MVC
using System.Web.Mvc;
using System.Reflection;
#endif

namespace JigsawPuzzle
{
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
        public ControllerAction(MethodInfo methodInfo)
        {
            Controller = methodInfo.DeclaringType.Name;
            Controller = Controller.Substring(0, Controller.IndexOf("Controller"));
            Action = methodInfo.Name;
            if (methodInfo.GetCustomAttribute<HttpGetAttribute>() != null)
                Type = "HttpGet";
            else if (methodInfo.GetCustomAttribute<HttpPostAttribute>() != null)
                Type = "HttpPost";
            else
                Type = "Error";
        }
#endif

        /* operator */
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder()
                .AppendLine($"{nameof(Action)} : {Action}")
                .AppendLine($"{nameof(Controller)} : {Controller}")
                .AppendLine($"{nameof(FormKeys)} : {FormKeys} {FormKeys.Length}")
                .AppendLine($"{nameof(FormValues)} : {FormValues} {FormValues.Length}")
                .AppendLine($"{nameof(ReturnType)} : {ReturnType}")
                .AppendLine($"{nameof(Type)} : {Type}");
            return builder.ToString();
        }
    }
}