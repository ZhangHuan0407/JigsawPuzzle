using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace JigsawPuzzle.UnitTest
{
    public class JPTaskConnector_UnitTest
    {
        [MenuItem("Unit Test/" + nameof(JPTaskConnector) + "/" + nameof(ServerRouteConfigShouldLoaded))]
        public static void ServerRouteConfigShouldLoaded()
        {
            JigsawPuzzleWindow window = EditorWindow.GetWindow<JigsawPuzzleWindow>();
            _ = window.Connector.Value;
            string content = JsonUtility.ToJson(window.Connector.Value.ServerRouteConfig, true);
            Debug.Log($"{nameof(ServerRouteConfig)} {content}");
        }

        [MenuItem("Unit Test/" + nameof(JPTaskConnector) + "/" + nameof(DashboardShouldReturnGetDetailInfo))]
        public static void DashboardShouldReturnGetDetailInfo()
        {
            JigsawPuzzleWindow window = EditorWindow.GetWindow<JigsawPuzzleWindow>();
            window.Connector.Value.Get(
                "GetDetailInfo", "Dashboard", 
                (object obj) => 
                {
                    Debug.Log($"Finish Dashboard/GetDetailInfo data :\n{obj}");
                },
                (HttpResponseMessage message) => 
                {
                    Debug.LogError("Error!");
                    Debug.Log(message.ToString());
                });
        }

        [MenuItem("Unit Test/" + nameof(JPTaskConnector) + "/" + nameof(ExplorerShouldGetFileMapJson))]
        public static void ExplorerShouldGetFileMapJson()
        {
            JigsawPuzzleWindow window = EditorWindow.GetWindow<JigsawPuzzleWindow>();
            window.Connector.Value.Get(
                "GetFileMapJson", "Explorer", 
                (object obj) =>
                {
                    Debug.Log($"Finish Explorer/GetFileMapJson data :\n{obj}");
                },
                (HttpResponseMessage message) =>
                {
                    Debug.LogError("Error!");
                    Debug.Log(message.ToString());
                });
        }

        [MenuItem("Unit Test/" + nameof(JPTaskConnector) + "/" + nameof(ExplorerShouldSelectFiles))]
        public static void ExplorerShouldSelectFiles()
        {
            FieldInfo fieldInfo = typeof(JPTaskConnector).GetField("Client", BindingFlags.NonPublic | BindingFlags.Instance);
            JPTaskConnector value = EditorWindow.GetWindow<JigsawPuzzleWindow>().Connector.Value;

            HttpClient client = fieldInfo.GetValue(value) as HttpClient;
            value.PostForm("SelectFiles", "Explorer", new Dictionary<string, object>
            {
                { "File", new string[]{ "11111", "22222", "33333" } },
            }, 
            (object result) => { Debug.Log(result); });
        }

    }
}