using System;

namespace JigsawPuzzle
{
    [ShareScript]
    public class JPRGBAColorMatch : ColorMatch<float, float>
    {
        /* const */
        public const float DefaultMaxSqrMagnitude = 0.03f;

        /* field */
        public float MaxSqrMagnitude;
        
        /* ctor */
        public JPRGBAColorMatch(
            JPColor[,] effectSpriteColor, 
            JPColor[,] spriteColor, 
            ShiftPositionPropensity propensity,
            float maxSqrMagnitude) : base()
        {
            EffectSpriteColor = effectSpriteColor ?? throw new NullReferenceException(nameof(effectSpriteColor));
            SpriteColor = spriteColor ?? throw new NullReferenceException(nameof(spriteColor));
            Propensity = propensity;
            MaxSqrMagnitude = maxSqrMagnitude;
        }

        /* func */
        protected override float GetDeltaValue(JPColor effectColor, JPColor spriteColor) =>
            JPColor.RGBADelta(effectColor, spriteColor).SqrMagnitude;
        protected override bool ValueMapIsBetter(float[,] valueMap, out float averageValue)
        {
            float TotalMaxDelta = EffectiveArea.Length * MaxSqrMagnitude;
            float value = 0f;
            for (int indedx = 0; indedx < EffectiveArea.Length / 3; indedx++)
            {
                Point selectPoint = EffectiveArea[indedx];
                value += valueMap[selectPoint.X, selectPoint.Y];
            }
            if (value > TotalMaxDelta)
            {
                averageValue = MaxSqrMagnitude;
                return false;
            }
            for (int indedx = EffectiveArea.Length / 3; indedx < EffectiveArea.Length; indedx++)
            {
                Point selectPoint = EffectiveArea[indedx];
                value += valueMap[selectPoint.X, selectPoint.Y];
            }
            averageValue = value / valueMap.Length;
            return value < TotalMaxDelta;
        }

        public override WeightedPoint BestOne()
        {
            Point bestPoint = Point.Zero;
            float minValue = 1f;
            foreach ((Point, float) position in PreferredPosition)
                if (position.Item2 < minValue)
                {
                    bestPoint = position.Item1;
                    minValue = position.Item2;
                }
            return new WeightedPoint(bestPoint, minValue);
        }
    }
}