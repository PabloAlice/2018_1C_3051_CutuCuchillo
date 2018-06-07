

using System;
using TGC.Core.Mathematica;

namespace TGC.Group.Model
{
    public class FloatModifier
    {
        private float minValue, maxValue;
        public float Modifier { get; set; }
        Func<float, float, float> Increase = (x, y) => x + y;
        Func<float, float, float> Decrease = (x, y) => x - y;
        Func<float, float, float> ModifierLambda;

        public FloatModifier(float initialValue, float minValue, float maxValue)
        {
            Modifier = initialValue;
            this.minValue = minValue;
            this.maxValue = maxValue;
            ModifierLambda = Increase;
        }

        public void Increment(float value)
        {
            Modifier = FastMath.Min(Modifier + value, maxValue);
        }

        public void Decrement(float value)
        {
            Modifier = FastMath.Max(Modifier - value, minValue);
        }

        public void Modify(float value)
        {
            Modifier = ModifierLambda.Invoke(Modifier,value);
            if (Modifier > maxValue) ModifierLambda = Decrease;
            if (Modifier < minValue) ModifierLambda = Increase;
        }
    }
}
