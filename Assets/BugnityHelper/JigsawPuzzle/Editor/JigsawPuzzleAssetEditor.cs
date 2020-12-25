using UnityEditor;
using UnityEngine;

namespace JigsawPuzzle
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
            {
                m_Target.CreateDataFiles();
                AssetDatabase.Refresh();
            }
            EditorGUILayout.Space();
            if (GUILayout.Button(nameof(JigsawPuzzleAsset.RemoveDataFiles)))
            {
                m_Target.RemoveDataFiles();
                AssetDatabase.Refresh();
            }
        }
    }
}