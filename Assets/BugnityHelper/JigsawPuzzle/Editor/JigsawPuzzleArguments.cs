using System;
using System.Collections.Generic;
using System.IO;
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
        public void LoadTextureCopy()
        {
            Effect.LoadPixel(this);
            foreach (SpriteCopy spriteCopy in AllImages)
                spriteCopy.LoadPixel(this);
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
        }

        public void Execute()
        {
            try
            {
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
                Debug.LogError($"Throw Exception in {nameof(JigsawPuzzleArguments)}.{nameof(Execute)} method.\n {e.Message}\n{e.StackTrace}");
            }
        }

        /* operator */
        public override string ToString()
        {
            return base.ToString();
        }
    }
}