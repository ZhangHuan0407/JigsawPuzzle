using System;
using System.Text;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEngine;
#elif MVC
using Newtonsoft.Json;
#endif

namespace JigsawPuzzle
{
#if !UNITY_EDITOR
    public interface ISerializationCallbackReceiver
    {
        void OnAfterDeserialize();
        void OnBeforeSerialize();
    }
#endif

    /// <summary>
    /// Fuck Unity, Fuck UnityEditor
    /// </summary>
    [ShareScript]
    public class JsonFuck
    {
        public static T FromJsonToObject<T>(string jsonContent)
        {
            object obj = FromJsonToObject(jsonContent, typeof(T));
            return (T)obj;
        }
        public static object FromJsonToObject(string jsonContent, Type t) 
        {
#if UNITY_EDITOR
            // Bugnity 读 json 数据的时候，如果有注释居然会报错 JSON parse error: Invalid value??
            // Unity 这么大公司，你们的模板数据里面一行注释都没有??
            // 你们代码可读性有0分吗?
            // 目前支持单行注释
            StringBuilder builder = new StringBuilder();
            foreach (string line in jsonContent.Split('\n'))
            {
                if (line.TrimStart().StartsWith("//"))
                    continue;
                else
                    builder.AppendLine(line);
            }
            return JsonUtility.FromJson(jsonContent, t);
#elif MVC
            object obj = JsonConvert.DeserializeObject(jsonContent, t);
            // Bugnity 不能序列化字典之类的类型，而且序列化前后调用此接口
            if (obj is ISerializationCallbackReceiver receiver)
                receiver.OnAfterDeserialize();
            return obj;
#endif
        }
        public static string FromObjectToJson(object obj)
        {
#if UNITY_EDITOR
            return JsonUtility.ToJson(obj);
#elif MVC
            return JsonConvert.SerializeObject(obj);
#endif
        }
    }
}