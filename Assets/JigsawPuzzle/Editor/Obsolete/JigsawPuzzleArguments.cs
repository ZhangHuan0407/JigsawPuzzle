using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using UnityEngine;

namespace BugnityHelper.JigsawPuzzle
{
    public class JigsawPuzzleArguments
    {
        /* const */
        /// <summary>
        /// <see cref="JigsawPuzzle"/> 缓存文件夹位置
        /// </summary>
        public const string DirectoryPath = "Temp/BugnityHelper/JigsawPuzzle/";

        /* field */
        internal readonly SpriteCopy[] AllImages;
        internal Dictionary<Texture2D, Texture2D> CopyTexture;
        internal readonly SpriteCopy Effect;

        internal readonly object LockFactor;
        internal readonly Queue<Action<JigsawPuzzleArguments>> TaskQueue;

        internal readonly string TempFileName;

        internal readonly Stopwatch Stopwatch;

        /* inter */
        internal int LeftTaskCount => TaskQueue.Count;

        /* ctor */
        public JigsawPuzzleArguments(string tempFileName, SpriteCopy effect, SpriteCopy[] allImages)
        {
            AllImages = allImages ?? throw new ArgumentNullException(nameof(allImages));
            CopyTexture = new Dictionary<Texture2D, Texture2D>();
            Effect = effect ?? throw new ArgumentNullException(nameof(effect));

            LockFactor = new object();
            TaskQueue = new Queue<Action<JigsawPuzzleArguments>>();

            TempFileName = tempFileName ?? throw new ArgumentNullException(nameof(tempFileName));

            Stopwatch = new Stopwatch();
        }

        /* func */
        public JigsawPuzzleArguments Add(Action<JigsawPuzzleArguments> task, bool threadSafe = false)
        {
            if (threadSafe)
                TaskQueue.Enqueue(task);
            else
                lock (LockFactor)
                {
                    TaskQueue.Enqueue(task);
                }
            return this;
        }
        public static void LoadTextureCopy(JigsawPuzzleArguments arguments)
        {
            arguments.Effect.LoadPixel(arguments);
            foreach (SpriteCopy spriteCopy in arguments.AllImages)
                spriteCopy.LoadPixel(arguments);
        }
        internal Texture2D GetTexture2DCopy(Sprite sprite)
        {
            Texture2D texture = sprite.texture;
            if (!CopyTexture.TryGetValue(texture, out Texture2D result))
            {
                // 绕过 isReadable 检测
                result = new Texture2D(texture.width, texture.height, texture.format, false);
                byte[] buffer = texture.GetRawTextureData();
                result.LoadRawTextureData(buffer);
                CopyTexture.Add(texture, result);
            }
            return result;
        }

        public static void WriteOut(JigsawPuzzleArguments arguments)
        {
            if (arguments is null)
                throw new ArgumentNullException(nameof(arguments));

            Directory.CreateDirectory(DirectoryPath);
            string content = arguments.ToString();
            File.WriteAllText($"{DirectoryPath}/{arguments.TempFileName}", content);
            UnityEngine.Debug.Log($"{arguments.TempFileName} have write out.\nIn {arguments.Stopwatch.ElapsedMilliseconds / 1000f:0.00}(s)");
            arguments.Stopwatch.Stop();
        }

        public void Execute()
        {
            try
            {
                UnityEngine.Debug.Log($"{TempFileName} start to exucute.");
                Action<JigsawPuzzleArguments> topTask;
                while (true)
                {
                    lock (LockFactor)
                    {
                        if (TaskQueue.Count == 0)
                            break;
                        else
                            topTask = TaskQueue.Dequeue();
                    }
                    topTask(this);
                }
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError($"Throw Exception in {nameof(JigsawPuzzleArguments)}.{nameof(Execute)} method.\n {e.Message}\n{e.StackTrace}");
            }
            finally
            {
                Stopwatch.Stop();
            }
        }

        /* operator */
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            foreach (SpriteCopy spriteCopy in AllImages)
            {
                builder.AppendLine($"{spriteCopy.ImageIdentified}")
                    .AppendLine($"{spriteCopy.HavePreferredPositiosn}&{spriteCopy.PreferredPosition}");
            }
            return builder.ToString();
        }
    }
}