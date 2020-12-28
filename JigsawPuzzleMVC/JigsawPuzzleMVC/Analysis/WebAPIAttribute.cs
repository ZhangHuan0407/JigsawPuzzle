using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web.Mvc;

namespace JigsawPuzzle.Analysis
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class WebAPIAttribute : Attribute
    {
        /* func */
        public static IEnumerable<MethodInfo> GetWebAPI(Type type)
        {
            //if (!type.IsAssignableFrom(typeof(Controller)))
            //    yield break;
            foreach (MethodInfo methodInfo in type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
            {
                if (methodInfo.GetCustomAttribute<WebAPIAttribute>() == null)
                    continue;
                ActionMethodSelectorAttribute actionMethodSelector = methodInfo.GetCustomAttribute<ActionMethodSelectorAttribute>(true);
                if (actionMethodSelector != null)
                    yield return methodInfo;
            }
        }
    }
}