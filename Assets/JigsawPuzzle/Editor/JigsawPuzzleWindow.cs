using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace JigsawPuzzle
{
    public class JigsawPuzzleWindow : EditorWindow
    {
        /* field */
        

        /* ctor */
        [MenuItem("Custom Tool/Jigsaw Puzzle")]
        public static void OpenJigsawPuzzleWindow()
        {
            JigsawPuzzleWindow window = GetWindow<JigsawPuzzleWindow>(nameof(JigsawPuzzleWindow));
        }



        /* func */


        private void OnGUI()
        {
            if (GUILayout.Button("1"))
            {
                JigsawPuzzleAsset.GetNew();
            }
            if (GUILayout.Button("2"))
            {
            }
        }
    }
}