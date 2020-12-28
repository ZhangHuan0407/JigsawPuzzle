using System.Net.Http;
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
                "Dashboard", "GetDetailInfo",
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
                "Explorer", "GetFileMapJson",
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
    }
}