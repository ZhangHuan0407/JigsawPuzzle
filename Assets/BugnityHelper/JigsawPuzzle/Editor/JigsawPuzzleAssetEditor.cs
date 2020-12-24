﻿using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace BugnityHelper.JigsawPuzzle
{
    [CustomEditor(typeof(JigsawPuzzleAsset))]
    public class JigsawPuzzleAssetEditor : Editor
    {
        /* inter */
        private JigsawPuzzleAsset m_Target => target as JigsawPuzzleAsset;

        /* func */
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.Space();
            if (GUILayout.Button(nameof(JigsawPuzzleAsset.RemoveInvalidData)))
                m_Target.RemoveInvalidData();
            EditorGUILayout.Space();
            if (GUILayout.Button(nameof(JigsawPuzzleAsset.CreateDataFiles)))
                m_Target.CreateDataFiles();
            EditorGUILayout.Space();
            if (GUILayout.Button(nameof(JigsawPuzzleAsset.RemoveDataFiles)))
                m_Target.RemoveDataFiles();
        }
    }
}