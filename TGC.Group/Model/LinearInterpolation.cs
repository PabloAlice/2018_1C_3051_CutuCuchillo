using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Mathematica;

namespace TGC.Group.Model
{
    class LinearInterpolation
    {
        private float toRotate;
        private float speed;

        public LinearInterpolation()
        {
            this.toRotate = 0f;
            this.speed = 0.9f;
        }

        public void Acumulate(float value)
        {
            this.toRotate += value;
        }

        public float Update(float time)
        {
            if (this.IsEnd()) return 0;
            float realVelocity = (toRotate >= 0) ? time * this.speed : -time * this.speed;
            toRotate -= realVelocity;
            realVelocity = this.CheckIfEnd(realVelocity);
            return realVelocity;
        }

        private float CheckIfEnd(float realVelocity)
        {
            System.Console.WriteLine("hey");
            if (realVelocity > 0 && this.toRotate < 0)
            {
                System.Console.WriteLine("me pase +");
                realVelocity -= FastMath.Abs(realVelocity - this.toRotate);
                this.toRotate = 0;
            }
            else if(realVelocity < 0 && this.toRotate > 0)
            {
                System.Console.WriteLine("me pase -");
                realVelocity -= FastMath.Abs(realVelocity - this.toRotate);
                this.toRotate = 0;
            }

            return realVelocity;
        }

        public bool IsEnd()
        {
            return this.toRotate == 0f;
        }

        
    }
}
