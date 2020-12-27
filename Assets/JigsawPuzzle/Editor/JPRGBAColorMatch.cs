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
        public override float GetDeltaValue(JPColor effectColor, JPColor spriteColor) =>
            JPColor.RGBADelta(effectColor, spriteColor).SqrMagnitude;
        public override bool ValueMapIsBetter(float[,] valueMap, out float averageValue)
        {
            float value = 0f;
            foreach (float add in valueMap)
                value += add;
            averageValue = value / valueMap.Length;
            return averageValue < MaxSqrMagnitude;
        }
    }
}