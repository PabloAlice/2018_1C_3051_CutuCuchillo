

using TGC.Core.Mathematica;

namespace TGC.Group.Model
{
    public class FloatModifier
    {
        private float minValue, maxValue;
        public float Modifier { get; set; }

        public FloatModifier(float initialValue, float minValue, float maxValue)
        {
            Modifier = initialValue;
            this.minValue = minValue;
            this.maxValue = maxValue;
        }

        public void Increment(float value)
        {
            Modifier = FastMath.Min(Modifier + value, maxValue);
        }

        public void Decrement(float value)
        {
            Modifier = FastMath.Max(Modifier - value, minValue);
        }
    }
}
