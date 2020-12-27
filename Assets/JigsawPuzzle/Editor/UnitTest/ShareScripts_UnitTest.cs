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
                bool mvcScriptsExists = File.Exists($"{MVCShareScriptDirectory}/{scriptFilePath}");
                bool unityScriptsExists = File.Exists($"{UnityShareScriptDirectory}/{scriptFilePath}");
                if (!mvcScriptsExists
                    || !unityScriptsExists)
                {
                    Debug.LogError($"File not found, {scriptFilePath}\n MVC : {mvcScriptsExists}, Unity : {unityScriptsExists}");
                    continue;
                }

            }
        }

        [MenuItem("Unit Test/" + nameof(ShareScriptAttribute) + "/1")]
        public static void ShareScriptsShoul11111111l()
        {

        }
    }
}