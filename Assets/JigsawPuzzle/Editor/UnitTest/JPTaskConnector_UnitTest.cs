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
                (HttpResponseMessage message) => 
                {
                    Debug.Log("Finish Dashboard/GetDetailInfo");
                    Debug.Log(message.ToString());
                },
                (HttpResponseMessage message) => 
                {
                    Debug.LogError("Error!");
                    Debug.Log(message.ToString());
                });
        }
    }
}