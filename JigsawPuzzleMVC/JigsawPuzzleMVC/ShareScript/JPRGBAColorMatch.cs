using System;

namespace JigsawPuzzle
{
    [ShareScript]
    public class JPRGBAColorMatch : JPColorMatch<float, float>
    {
        /* const */
        public const float DefaultMaxSqrMagnitude = 0.5f;

        /* ctor */
        public JPRGBAColorMatch(
            JPColor[,] effectSpriteColor,
            JPColor[,] spriteColor,
            ShiftPositionPropensity propensity,
            WeightedPoint[] preferredPositions = null) : base()
        {
            EffectSpriteColor = effectSpriteColor ?? throw new NullReferenceException(nameof(effectSpriteColor));
            SpriteColor = spriteColor ?? throw new NullReferenceException(nameof(spriteColor));
            Propensity = propensity;
            if (preferredPositions != null)
                PreferredPosition.AddRange(preferredPositions);
        }

        /* func */
        protected override float GetDeltaValue(JPColor effectColor, JPColor spriteColor) =>
            JPColor.RGBADelta(effectColor, spriteColor).SqrMagnitude;
        protected override bool ValueMapIsBetter(float[,] valueMap, out float averageDeltaValue)
        {
            float TotalMaxDelta = EffectiveArea.Length * DefaultMaxSqrMagnitude;
            float value = 0f;
            for (int indedx = 0; indedx < EffectiveArea.Length; indedx++)
            {
                Point selectPoint = EffectiveArea[indedx];
                value += valueMap[selectPoint.X, selectPoint.Y];
            }
            averageDeltaValue = value / EffectiveArea.Length;
            return value < TotalMaxDelta;
        }
    }
}