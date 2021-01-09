using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace JigsawPuzzle
{
    public class JigsawPuzzleWindow : EditorWindow
    {
        /* field */
        internal Lazy<JPTaskConnector> Connector = new Lazy<JPTaskConnector>(() => CreateJPTaskConnector());
        internal JPFileMap FileMap;
        internal bool Hotkey;
        internal KeyCode NextKeyCode;
        private Queue<Action> Coroutine;

        internal Image EffectImage;
        internal Dictionary<Sprite, JigsawPuzzleAsset> JPAssetPool;

        internal static readonly object LockFactor = new object();
        internal static Action Callback;

        /* inter */
        internal RectTransform EffectSlider
        {
            get
            {
                if (EffectImage
                    && EffectImage.transform.Find(nameof(EffectSlider)) is RectTransform sliderTrans)
                    return sliderTrans;
                else
                    return null;
            }
        }

        /* ctor */
        [MenuItem("Custom Tool/Jigsaw Puzzle")]
        public static void OpenJigsawPuzzleWindow()
        {
            JigsawPuzzleWindow window = GetWindow<JigsawPuzzleWindow>(nameof(JigsawPuzzleWindow));
        }
        private void OnEnable()
        {
            Hotkey = false;
            Coroutine = new Queue<Action>();
            EffectImage = null;
            JPAssetPool = new Dictionary<Sprite, JigsawPuzzleAsset>();
        }
        private void OnDisable()
        {
            if (Hotkey)
                SceneView.duringSceneGui -= SceneView_TryUseHotkey;
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
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("GetNew"))
                Coroutine.Enqueue(() => JigsawPuzzleAsset.GetNew());
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(15f);
            GUILayout.BeginHorizontal();
            GUILayout.Label($"{nameof(EffectImage)} : {(EffectImage ? EffectImage.name : "null")}");
            if (GUILayout.Button(nameof(SetSelectedAsEffectImage)))
                Coroutine.Enqueue(SetSelectedAsEffectImage);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label(nameof(NextKeyCode));
            NextKeyCode = (KeyCode)EditorGUILayout.EnumPopup(NextKeyCode);
            GUILayout.Space(15f);
            if (GUILayout.Button($"{nameof(Hotkey)} {(Hotkey ? "Unregister" : "Register")}"))
            {
                if (Hotkey)
                    SceneView.duringSceneGui -= SceneView_TryUseHotkey;
                else
                    SceneView.duringSceneGui += SceneView_TryUseHotkey;
                Hotkey = !Hotkey;
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(15f);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button(nameof(GetTaskMap))
                && Coroutine.Count == 0)
                Coroutine.Enqueue(GetTaskMap);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            if (FileMap != null)
            {
                GUILayout.Label($"{nameof(FileMap)} Length : {FileMap.Task.Length}");
                for (int index = 0; index < FileMap.Task.Length; index++)
                {
                    string serverTask = FileMap.Task[index];
                    GUILayout.BeginHorizontal();
                    GUILayout.Label($"Client : {FileMap.ClientTask?[index]}");
                    GUILayout.Label($"Server : {serverTask}");
                    if (GUILayout.Button(nameof(DownloadInfoData)))
                        Coroutine.Enqueue(() => DownloadInfoData(serverTask));
                    if (GUILayout.Button(nameof(JPTask.StartRemoteTask)))
                        Coroutine.Enqueue(() => JPTask.StartRemoteTask(Connector.Value, new string[] { $"Task/{serverTask}" }));
                    GUILayout.EndHorizontal();
                }
            }
        }

        private void Update()
        {
            while (Coroutine.Count > 0)
                Coroutine.Dequeue()();
            lock (LockFactor)
            {
                if (Callback != null)
                {
                    Action copy = Callback;
                    Callback = null;
                    copy();
                }
            }
        }

        private void GetTaskMap()
        {
            Connector.Value.Get("GetFileMapJson", "Explorer",
                (object obj) =>
                {
                    FileMap = obj as JPFileMap;
                    string[] serverTask = FileMap.Task;
                    FileMap.ClientTask = new string[serverTask.Length];
                    string[] clientTask = FileMap.ClientTask;
                    HashSet<string> clientAssetFile = new HashSet<string>(JigsawPuzzleAsset.GetAssetList());
                    for (int index = 0; index < serverTask.Length; index++)
                    {
                        string fileName = JPFileMap.GetFileName(serverTask[index]);
                        if (clientAssetFile.Contains(fileName))
                            clientTask[index] = "Exists";
                        else
                            clientTask[index] = "Not Found";
                    }
                });
        }

        private void DownloadInfoData(string task)
        {
            Connector.Value.PostForm("SelectFiles", "Explorer",
                new Dictionary<string, object>() { { "File", new string[] { $"Task/{task}" } }, })
            ?.Result.Get("DownloadFile", "Explorer",
                (object contents) =>
                {
                    lock (LockFactor)
                    {
                        Callback = () => JigsawPuzzleAsset.OverrideInfoData(JPFileMap.GetFileName(task), contents as byte[]);
                    }
                },
                (HttpResponseMessage message) =>
                {
                    Debug.LogError(message.Content.ReadAsStringAsync().Result);
                });
        }
        private void SetSelectedAsEffectImage()
        {
            if (!Selection.activeGameObject)
            {
                Debug.LogWarning("Where is effect image?");
                return;
            }
            EffectImage = Selection.activeGameObject.GetComponent<Image>();
            if (!EffectImage)
            {
                Debug.LogWarning("Where is effect image?");
                return;
            }
            else
            {
                Transform effectTrans = EffectImage.transform;
                if (effectTrans.Find(nameof(EffectSlider)) == null)
                {
                    GameObject sliderGo = new GameObject(nameof(EffectSlider), typeof(RectTransform));
                    RectTransform sliderTrans = sliderGo.transform as RectTransform;
                    sliderTrans.SetParent(effectTrans, false);
                    sliderTrans.anchorMin = Vector2.zero;
                    sliderTrans.anchorMax = Vector2.zero;
                    sliderTrans.sizeDelta = Vector2.zero;
                }
            }
        }

        private void SceneView_TryUseHotkey(SceneView view)
        {
            Event currentEvent = Event.current;
            if (currentEvent is null
                || currentEvent.type != EventType.KeyUp
                || currentEvent.keyCode == KeyCode.None)
                return;
            if (currentEvent.keyCode == NextKeyCode)
                Coroutine.Enqueue(SetImagePosition);
        }

        internal void SetImagePosition()
        {
            if (!EffectImage)
            {
                Debug.LogWarning($"Not found {nameof(EffectImage)}");
                return;
            }
            RectTransform sliderTrans = EffectSlider;
            if (!sliderTrans)
            {
                Debug.LogWarning($"Not found {nameof(EffectSlider)}");
                return;
            }
            if (!Selection.activeGameObject)
            {
                Debug.LogWarning($"Not found {nameof(Selection.activeGameObject)}");
                return;
            }
            Sprite imageSprite = Selection.activeGameObject.GetComponent<Image>()?.sprite;
            if (!imageSprite)
            {
                Debug.LogWarning($"Not found {nameof(imageSprite)}");
                return;
            }

            if (!JPAssetPool.TryGetValue(EffectImage.sprite, out JigsawPuzzleAsset asset))
            {
                Debug.LogWarning($"Not found {nameof(JigsawPuzzleAsset)}");
                return;
            }
            else if (asset[$"{imageSprite.texture.name}/{imageSprite.name}"] is SpriteInfo spriteInfo
                && spriteInfo.BestOne is Vector2Int bestPosition)
            {
                sliderTrans.anchoredPosition = bestPosition;
                Selection.activeGameObject.transform.position = sliderTrans.position;
            }
        }
    }
}