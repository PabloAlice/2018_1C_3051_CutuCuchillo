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
        private Effect effect;
        private List<Light> lights;
        private List<ColorValue> lightColors;
        private List<Vector4> pointLightPositions;
        private List<float> pointLightIntensities;
        private List<float> pointLightAttenuations;
        private ColorValue EmissiveModifier = new ColorValue(0, 0, 0);
        private ColorValue AmbientModifier = new ColorValue(255, 255, 255);
        private ColorValue DiffuseModifier = new ColorValue(255, 255, 255);
        private ColorValue SpecularModifier = new ColorValue(255, 255, 255);
        private float SpecularEx = 8;
        private static LightManager instance;

        public LightManager()
        {
            this.lights = new List<Light>();
            this.lightColors = new List<ColorValue>();
            this.pointLightPositions = new List<Vector4>();
            this.pointLightIntensities = new List<float>();
            this.pointLightAttenuations = new List<float>();
            this.effect = TgcShaders.Instance.TgcMeshPointLightShader;
        }

        public void ChangeLights(ColorValue color)
        {
            DiffuseModifier = color;
        }

        public void DoLightMe(TgcMesh mesh)
        {
            if (this.lights.Count == 0) return;
            string currentTechnique = "DIFFUSE_MAP";
            mesh.Effect = this.effect;
            mesh.Technique = currentTechnique;
            TGCVector3 p = Scene.GetInstance().camera.Position;

            mesh.Effect.SetValue("lightColor", lightColors.First());
            mesh.Effect.SetValue("lightPosition", pointLightPositions.First());
            mesh.Effect.SetValue("eyePosition", TGCVector3.Vector3ToFloat4Array(p));
            mesh.Effect.SetValue("lightIntensity", pointLightIntensities.First());
            mesh.Effect.SetValue("lightAttenuation", pointLightAttenuations.First());
            mesh.Effect.SetValue("materialEmissiveColor", EmissiveModifier);
            mesh.Effect.SetValue("materialAmbientColor", AmbientModifier);
            mesh.Effect.SetValue("materialDiffuseColor", DiffuseModifier);
            mesh.Effect.SetValue("materialSpecularColor", SpecularModifier);
            mesh.Effect.SetValue("materialSpecularExp", SpecularEx);
        }

        public void DoLightMe(Effect effect)
        {
            TGCVector3 p = Scene.GetInstance().camera.Position;
            effect.SetValue("lightColor", lightColors.First());
            effect.SetValue("lightPosition", pointLightPositions.First());
            effect.SetValue("eyePosition", TGCVector3.Vector3ToFloat4Array(p));
            effect.SetValue("lightIntensity", pointLightIntensities.First());
            effect.SetValue("lightAttenuation", pointLightAttenuations.First());
            effect.SetValue("materialEmissiveColor", EmissiveModifier);
            effect.SetValue("materialAmbientColor", AmbientModifier);
            effect.SetValue("materialDiffuseColor", DiffuseModifier);
            effect.SetValue("materialSpecularColor", SpecularModifier);
            effect.SetValue("materialSpecularExp", SpecularEx);
        }

        public void SuscribeLight(Light light)
        {
            if (this.lights.Contains(light)) return;
            this.lights.Add(light);
            this.pointLightPositions.Add(TGCVector3.Vector3ToVector4(light.position));
            this.lightColors.Add(light.lightColor);
            this.pointLightIntensities.Add(light.intensity);
            this.pointLightAttenuations.Add(light.attenuation);
        }

        public void ResetLights()
        {
            this.lights = new List<Light>();
            this.lightColors = new List<ColorValue>();
            this.pointLightPositions = new List<Vector4>();
            this.pointLightIntensities = new List<float>();
            this.pointLightAttenuations = new List<float>();
        }

        public static LightManager GetInstance()
        {
            if (instance == null)
            {
                instance = new LightManager();
            }
            return instance;
        }

        public void Dispose()
        {
            this.effect.Dispose();
        }
    }
}
