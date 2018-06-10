using Microsoft.DirectX.Direct3D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Mathematica;

namespace TGC.Group.Lighting
{
    class Light
    {
        public ColorValue lightColor { get; set; }
        public TGCVector3 position { get; set; }
        public float intensity { get; set; }
        public float attenuation { get; set; }

        public Light(ColorValue lightColor, TGCVector3 position, float intensity, float attenuation)
        {
            this.lightColor = lightColor;
            this.position= position;
            this.intensity= intensity;
            this.attenuation= attenuation;
        }
    }
}
