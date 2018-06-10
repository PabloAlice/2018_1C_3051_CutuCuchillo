using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Geometry;
using TGC.Core.Interpolation;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Core.Shaders;
using TGC.Group.Model;

namespace TGC.Group.Lighting
{
    class LightManager
    {
        public static LightManager Instance { get; } = new LightManager();
        private Effect effect;
        private Light[] lights = new Light[] { };
        private ColorValue[] lightColors = new ColorValue[] { };
        private Vector4[] pointLightPositions = new Vector4[] { };
        private float[] pointLightIntensities = new float[] { };
        private float[] pointLightAttenuations = new float[] { };
        private ColorValue EmissiveModifier = new ColorValue(0,0,0);
        private ColorValue DiffuseModifier = new ColorValue(255,255,255);
            
        public LightManager() { }

        public void DoLightMe(TgcMesh mesh)
        {
            if (this.lights.Length == 0) return;
            this.effect = TgcShaders.loadEffect(GlobalConcepts.GetInstance().GetShadersDir() + "MultiDiffuseLights.fx");
            string currentTechnique;
            currentTechnique = "MultiDiffuseLightsTechnique";
            mesh.Effect = this.effect;
            mesh.Technique = currentTechnique;

            mesh.Effect.SetValue("numLights", lightColors.Length);
            mesh.Effect.SetValue("lightColor", lightColors);
            mesh.Effect.SetValue("lightPosition", pointLightPositions);
            mesh.Effect.SetValue("lightIntensity", pointLightIntensities);
            mesh.Effect.SetValue("lightAttenuation", pointLightAttenuations);
            mesh.Effect.SetValue("materialEmissiveColor", EmissiveModifier);
            mesh.Effect.SetValue("materialDiffuseColor", DiffuseModifier);
        }

        public void SuscribeLight(Light light)
        {
            if (this.lights.Contains(light)) return;
            this.lights.Append(light);
            this.pointLightPositions.Append(TGCVector3.Vector3ToVector4(light.position));
            this.pointLightIntensities.Append(light.intensity);
            this.pointLightAttenuations.Append(light.attenuation);
        }

        public void Dispose()
        {
            this.effect.Dispose();
        }
    }
}
