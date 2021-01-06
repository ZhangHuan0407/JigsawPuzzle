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
        internal bool AutoHotkey;
        internal KeyCode LastKeyCode;
        internal KeyCode NextKeyCode;
        private Queue<Action> Coroutine;

        /* ctor */
        [MenuItem("Custom Tool/Jigsaw Puzzle")]
        public static void OpenJigsawPuzzleWindow()
        {
            JigsawPuzzleWindow window = GetWindow<JigsawPuzzleWindow>(nameof(JigsawPuzzleWindow));
        }
        private void OnEnable()
        {
            AutoHotkey = false;
            Coroutine = new Queue<Action>();
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
            if (GUILayout.Button("GetFileMap")
                && Coroutine.Count == 0)
                Coroutine.Enqueue(() => 
                {
                    Connector.Value.Get(
                        "GetFileMapJson", "Explorer",
                        (object obj) => { FileMap = obj as JPFileMap; });
                });
            if (GUILayout.Button($"{nameof(AutoHotkey)} {(AutoHotkey ? "Unregister" : "Register")}"))
            {
                if (AutoHotkey)
                    SceneView.duringSceneGui -= SceneView_TryUseHotkey;
                else
                    SceneView.duringSceneGui += SceneView_TryUseHotkey;
                AutoHotkey = !AutoHotkey;
            }
            GUILayout.BeginHorizontal();
            GUILayout.Label(nameof(LastKeyCode));
            LastKeyCode = (KeyCode)EditorGUILayout.EnumPopup(LastKeyCode);
            GUILayout.Label(nameof(NextKeyCode));
            NextKeyCode = (KeyCode)EditorGUILayout.EnumPopup(NextKeyCode);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            if (FileMap != null)
            {
                foreach (string task in FileMap.Task)
                    if (GUILayout.Button(task))
                        JPTask.StartRemoteTask(Connector.Value, new string[] { $"Task/{task}" });
            }
        }

        private void Update()
        {
            while (Coroutine.Count > 1)
                Coroutine.Dequeue();
        }

        private void SceneView_TryUseHotkey(SceneView view)
        {
            Event currentEvent = Event.current;
            if (currentEvent is null
                || currentEvent.type != EventType.KeyUp
                || currentEvent.keyCode == KeyCode.None)
                return;
            if (currentEvent.keyCode == LastKeyCode)
                Coroutine.Enqueue(() => SetImagePosition(-1));
            if (currentEvent.keyCode == NextKeyCode)
                Coroutine.Enqueue(() => SetImagePosition(1));
        }
        internal void SetImagePosition(int shift)
        {

        }
    }
}