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

        public static readonly List<(string, string)> AssetFilesPath = new List<(string, string)>()
        {
            ("JigsawPuzzleMVC/JigsawPuzzleMVC/App_Data/ServerRouteConfig.json", 
            "Assets/Editor Default Resources/JigsawPuzzle/ServerRouteConfig.json"),
        };

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

        [MenuItem("Unit Test/" + nameof(ShareScriptAttribute) + "/" + nameof(AssetShouldEqual))]
        public static void AssetShouldEqual()
        {
            foreach ((string, string) filePath in AssetFilesPath)
            {
                bool mvcScriptFileExists = File.Exists(filePath.Item1);
                bool unityScriptFileExists = File.Exists(filePath.Item2);
                if (!mvcScriptFileExists
                    || !unityScriptFileExists)
                {
                    Debug.LogError($"File not found\n MVC : {mvcScriptFileExists}, {filePath.Item1}\n Unity : {unityScriptFileExists}, {filePath.Item2}");
                    continue;
                }
                else if (!File.ReadAllText(filePath.Item1).Equals(File.ReadAllText(filePath.Item2)))
                {
                    string mvcLastWriteTime = new FileInfo(filePath.Item1).LastWriteTime.ToString();
                    string unityLastWriteTime = new FileInfo(filePath.Item2).LastWriteTime.ToString();
                    Debug.LogError($"File is not equal\n MVC : {mvcScriptFileExists} LastWriteTime : {mvcLastWriteTime}, {filePath.Item1}\nUnity : {unityScriptFileExists} LastWriteTime : {unityLastWriteTime}, {filePath.Item2}");
                    continue;
                }
            }
            Debug.Log("Finish");
        }
    }
}