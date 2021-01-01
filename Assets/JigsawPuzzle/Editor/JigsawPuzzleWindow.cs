using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace JigsawPuzzle
{
    public class JigsawPuzzleWindow : EditorWindow
    {
        /* field */
        internal Lazy<JPTaskConnector> Connector = new Lazy<JPTaskConnector>(() => CreateJPTaskConnector());
        internal JPFileMap FileMap;

        /* ctor */
        [MenuItem("Custom Tool/Jigsaw Puzzle")]
        public static void OpenJigsawPuzzleWindow()
        {
            JigsawPuzzleWindow window = GetWindow<JigsawPuzzleWindow>(nameof(JigsawPuzzleWindow));
        }

        /* func */
        private static JPTaskConnector CreateJPTaskConnector()
        {
            string content = File.ReadAllText(JPTaskConnector.Path);
            JPTaskConnector connector = new JPTaskConnector(content);
            return connector;
        }

        private void OnGUI()
        {
            if (GUILayout.Button("GetNew"))
            {
                JigsawPuzzleAsset.GetNew();
            }
            // 线程不安全!
            if (GUILayout.Button("GetFileMap"))
                Connector.Value.Get("GetFileMapJson", "Explorer", (object obj) => { FileMap = obj as JPFileMap; });
            if (FileMap != null)
            {
                foreach (string task in FileMap.Task)
                    if (GUILayout.Button(task))
                        JPTask.StartRemoteTask(Connector.Value, new string[] { $"Task/{task}" });
            }
        }
    }
}