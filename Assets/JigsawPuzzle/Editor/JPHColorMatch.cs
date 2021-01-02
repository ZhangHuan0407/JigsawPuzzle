using System;

namespace JigsawPuzzle
{
    [ShareScript]
    public class JPHColorMatch : ColorMatch<float, float>
    {
        /* const */
        public const float DefaultMaxDelta = 0.1f;

        /* field */
        public float MaxDelta;

        /* ctor */
        public JPHColorMatch(
            JPColor[,] effectSpriteColor,
            JPColor[,] spriteColor,
            ShiftPositionPropensity propensity,
            float maxDelta = DefaultMaxDelta) : base()
        {
            EffectSpriteColor = effectSpriteColor ?? throw new NullReferenceException(nameof(effectSpriteColor));
            SpriteColor = spriteColor ?? throw new NullReferenceException(nameof(spriteColor));
            Propensity = propensity;
            MaxDelta = maxDelta;
        }

        /* func */
        protected override float GetDeltaValue(JPColor effectColor, JPColor spriteColor) =>
            JPColor.HDelta(effectColor, spriteColor);
        protected override bool ValueMapIsBetter(float[,] valueMap, out float averageValue)
        {
            float TotalMaxDelta = EffectiveArea.Length * MaxDelta;
            float value = 0f;
            for (int indedx = 0; indedx < EffectiveArea.Length / 3; indedx++)
            {
                Point selectPoint = EffectiveArea[indedx];
                value += valueMap[selectPoint.X, selectPoint.Y];
            }
            if (value > TotalMaxDelta)
            {
                averageValue = MaxDelta;
                return false;
            }
            for (int indedx = EffectiveArea.Length / 3; indedx < EffectiveArea.Length; indedx++)
            {
                Point selectPoint = EffectiveArea[indedx];
                value += valueMap[selectPoint.X, selectPoint.Y];
            }
            averageValue = value / EffectiveArea.Length;
            return value < TotalMaxDelta;
        }

        public override (Point, float) BestOne()
        {
            Point bestPoint = Point.Zero;
            float minValue = 1f;
            foreach ((Point, float) position in PreferredPosition)
                if (position.Item2 < minValue)
                {
                    bestPoint = position.Item1;
                    minValue = position.Item2;
                }
            return (bestPoint, minValue);
        }

    }
}