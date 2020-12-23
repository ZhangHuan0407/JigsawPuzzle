using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BugnityHelper.JigsawPuzzle
{
    [RequireComponent(typeof(Image))]
    public sealed class JigsawPuzzleEffect : MonoBehaviour
    {
        /* field */
        [Tooltip("约定使用的缓存文件，此文件位于 Temp/BugnityHelper/JigsawPuzzle 文件夹下")]
        public string TempFileName;

        [Tooltip("零散碎图放于此处")]
        public List<Image> AllImages;

        /* inter */
        public Sprite Sprite => GetComponent<Image>().sprite;

        /* ctor */
        public void Reset()
        {
            TempFileName = Guid.NewGuid().ToString();
        }
        private void Awake()
        {
            Transform parent = transform.parent;
            string path;
            if (parent == null)
                path = gameObject.name;
            else
                path = $"{parent.gameObject.name}/{gameObject.name}";
            Debug.LogWarning($"Use {nameof(JigsawPuzzleEffect)} in Runtime, please remove this component before submit!\nGameObject.name : {path}");
            enabled = false;
        }
    }
}