using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine;

namespace BugnityHelper.JigsawPuzzle
{
    public class JigsawPuzzleWindow : EditorWindow
    {
        /* field */
        

        /* ctor */
        [MenuItem("BugnityHelper/JigsawPuzzle")]
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