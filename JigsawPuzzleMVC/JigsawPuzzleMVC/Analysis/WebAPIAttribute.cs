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
        /// <summary>
        /// 获取目标 <see cref="Controller"/> 子继承类型中，所有具有 <see cref="WebAPIAttribute"/> + <see cref="ActionMethodSelectorAttribute"/> 特性的公开实例方法
        /// <para>如果不满足 <see cref="Controller"/> 子继承类型，返回空迭代器</para>
        /// <para>如果不具有满足条件的方法，返回空迭代器</para>
        /// </summary>
        /// <param name="type">目标类型</param>
        /// <returns>满足条件的方法</returns>
        public static IEnumerable<MethodInfo> GetWebAPI(Type type)
        {
            if (!typeof(Controller).IsAssignableFrom(type)
                || type.IsAbstract)
                yield break;
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