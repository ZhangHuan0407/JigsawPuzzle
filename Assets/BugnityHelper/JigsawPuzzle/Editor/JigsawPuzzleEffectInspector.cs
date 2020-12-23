using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace BugnityHelper.JigsawPuzzle
{
    [CustomEditor(typeof(JigsawPuzzleEffect))]
    public class JigsawPuzzleEffectInspector : Editor
    {
        /* field */
        private JigsawPuzzleEffect m_target;

        private object LockFactor;
        private CancellationToken m_CancellationToken;

        private JigsawPuzzleArguments JPArguments;

        /* ctor */
        private void OnEnable()
        {
            m_target = target as JigsawPuzzleEffect;
            LockFactor = new object();
            JPArguments = null;
        }
        private void OnDisable()
        {
            m_target = null;
            JPArguments = null;
            lock (LockFactor)
            {
                if (m_CancellationToken != default)
                {
                    m_CancellationToken.ThrowIfCancellationRequested();
                    Debug.LogWarning($"{nameof(JigsawPuzzleEffect)} task invoke Cancellation Requested.");
                }
            }
        }

        /* func */
        public override void OnInspectorGUI()
        {
            GUILayout.Label("会被储存的数据");
            base.OnInspectorGUI();
            GUILayout.Space(15f);

            GUILayout.Label("不会被储存的数据");
            if (GUILayout.Button("Recreate Cache"))
                OnClickRecreateCache();

            GUILayout.Label($"LeftTaskCount : {JPArguments?.LeftTaskCount}");
        }

        private void OnClickRecreateCache()
        {
            lock (LockFactor)
            {
                if (m_CancellationToken != default)
                {
                    Debug.LogError($"Have one task in background.");
                    return;
                }
                RecreateCache();
            }
        }
        private void RecreateCache()
        {
            // 我懒得拦截 null 了，反正拦截完也要 LogError，直接报错也一样
            SpriteCopy effectCopy = new SpriteCopy(m_target.Sprite);
            IEnumerable<SpriteCopy> imageTexture2DCopy = from image in m_target.AllImages
                                     where image.sprite != null
                                     select new SpriteCopy(image.sprite);

            JPArguments = new JigsawPuzzleArguments(
                m_target.TempFileName,
                effectCopy,
                imageTexture2DCopy.ToArray());

            JPArguments.Add((JigsawPuzzleArguments argument) => 
            {
                argument.Effect.CreateMinimap();
            }, true);
            int imageCount = JPArguments.AllImages.Length;
            for (int index = 0; index < imageCount; index++)
            {
                SpriteCopy texture2DCopy = JPArguments.AllImages[index];
                JPArguments.Add((_) => { texture2DCopy.CreateMinimap(); }, true);
            }

            for (int index = 0; index < imageCount; index++)
            {
                SpriteCopy texture2DCopy = JPArguments.AllImages[index];
                JPArguments.Add(texture2DCopy.TryGetPreferredLocalPosition, true);
            }

            JPArguments
                .Add(JigsawPuzzleArguments.WriteOut, true)
                .Add((_) => { m_CancellationToken = default; }, true);

            lock (LockFactor)
            {
                if (m_CancellationToken != default)
                {
                    Debug.LogError($"Have one task in background.");
                    return;
                }

                m_CancellationToken = new CancellationToken();
                Task.Run(JPArguments.Execute, m_CancellationToken);
            }
        }
    }
}