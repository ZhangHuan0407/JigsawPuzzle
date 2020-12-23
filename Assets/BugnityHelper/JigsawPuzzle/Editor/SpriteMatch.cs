using System;
using System.Collections.Generic;
using UnityEngine;

namespace BugnityHelper.JigsawPuzzle
{
    public class SpriteMatch
    {
        /* field */
        internal readonly Vector4[,] Background;
        internal readonly Vector4[,] Fontground;

        public SpriteMatch(Vector4[,] background, Vector4[,] fontground)
        {
            Background = background ?? throw new ArgumentNullException(nameof(background));
            Fontground = fontground ?? throw new ArgumentNullException(nameof(fontground));
        }
    }
}