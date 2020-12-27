using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace JigsawPuzzle.UnitTest
{
    public class ShareScripts_UnitTest
    {
        /* const */
        public const string MVCShareScriptDirectory = "JigsawPuzzleMVC/JigsawPuzzleMVC/ShareScript";
        public const string UnityShareScriptDirectory = "Assets/JigsawPuzzle/Editor";

        /* func */
        [MenuItem("Unit Test/" + nameof(ShareScriptAttribute) + "/" + nameof(ShareScriptShouldEqual))]
        public static void ShareScriptShouldEqual()
        {
            List<string> scriptFilesPath = new List<string>();
            foreach (Type type in typeof(ShareScriptAttribute).Assembly.GetTypes())
            {
                object[] shareScriptsAttributes = type.GetCustomAttributes(typeof(ShareScriptAttribute), false);
                if (shareScriptsAttributes.Length > 0)
                {
                    string scriptFileName = type.Name;
                    int index = scriptFileName.IndexOf('`');
                    if (index != -1)
                        scriptFileName = scriptFileName.Substring(0, index);
                    scriptFilesPath.Add($"{scriptFileName}.cs");
                }
            }
            foreach (string scriptFilePath in scriptFilesPath)
            {
                string mvcScriptFilePath = $"{MVCShareScriptDirectory}/{scriptFilePath}";
                bool mvcScriptFileExists = File.Exists(mvcScriptFilePath);
                string unityScriptFilePath = $"{UnityShareScriptDirectory}/{scriptFilePath}";
                bool unityScriptFileExists = File.Exists(unityScriptFilePath);
                if (!mvcScriptFileExists
                    || !unityScriptFileExists)
                {
                    Debug.LogError($"File not found, {scriptFilePath}\n MVC : {mvcScriptFileExists}, Unity : {unityScriptFileExists}");
                    continue;
                }
                if (!File.ReadAllText(mvcScriptFilePath).Equals(File.ReadAllText(unityScriptFilePath)))
                {
                    string mvcLastWriteTime = new FileInfo(mvcScriptFilePath).LastWriteTime.ToString();
                    string unityLastWriteTime = new FileInfo(unityScriptFilePath).LastWriteTime.ToString();
                    Debug.LogError($"File is not equal, {scriptFilePath}\n MVC : {mvcScriptFileExists} LastWriteTime : {mvcLastWriteTime}, Unity : {unityScriptFileExists} LastWriteTime : {unityLastWriteTime}");
                    continue;
                }
            }
            Debug.Log("Finish");
        }
    }
}