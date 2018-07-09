using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Direct3D;
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
        private Effect lightEffect;
        private Effect shadowEffect;
        private List<Light> lights;
        private List<ColorValue> lightColors;
        private List<Vector4> pointLightPositions;
        private List<float> pointLightIntensities;
        private List<float> pointLightAttenuations;
        private ColorValue EmissiveModifier = new ColorValue(0,0,0);
        private ColorValue AmbientModifier = new ColorValue(255, 255, 255);
        private ColorValue DiffuseModifier = new ColorValue(255,255,255);
        private static LightManager instance;
        // FOR SHADOW MAP
        public int SHADOWMAP_SIZE { get; set; } = 1024;
        private TGCVector3 g_LightDir;  // direccion de la luz actual
        private TGCVector3 g_LightPos; // posicion de la luz actual (la que estoy analizando)
        private TGCMatrix g_LightView; // matriz de view del light
        private TGCMatrix g_mShadowProj; // Projection matrix for shadow map
        public Texture g_pShadowMap { get; set; } // Texture to which the shadow map is rendered
        // FOR SHADOW MAP

        public LightManager() {
            this.lights = new List<Light>();
            this.lightColors = new List<ColorValue> ();
            this.pointLightPositions = new List<Vector4>();
            this.pointLightIntensities = new List<float>();
            this.pointLightAttenuations = new List<float>();
            this.shadowEffect = TgcShaders.loadEffect(GlobalConcepts.GetInstance().GetShadersDir() + "TgcMeshSpotLightShader.fx");
            this.lightEffect = TgcShaders.loadEffect(GlobalConcepts.GetInstance().GetShadersDir() + "TgcMeshPointLightShader.fx");
            g_pShadowMap = new Texture(D3DDevice.Instance.Device, SHADOWMAP_SIZE, SHADOWMAP_SIZE, 1, Usage.RenderTarget, Format.R32F, Pool.Default);
            var aspectRatio = D3DDevice.Instance.AspectRatio;
            g_mShadowProj = TGCMatrix.PerspectiveFovLH(Geometry.DegreeToRadian(80), aspectRatio, 50, 5000);
            g_LightPos = new TGCVector3(0, 200, 0);
            g_LightDir = TGCVector3.Empty - g_LightPos;

        }

        public void DoLightMe(TgcMesh mesh)
        {
            mesh.Effect = this.shadowEffect;
            mesh.Technique = "RenderScene";

            this.shadowEffect.SetValue("g_vLightPos", new Vector4(g_LightPos.X, g_LightPos.Y, g_LightPos.Z, 1));
            this.shadowEffect.SetValue("g_vLightDir", new Vector4(g_LightDir.X, g_LightDir.Y, g_LightDir.Z, 1));
            g_LightView = TGCMatrix.LookAtLH(g_LightPos, g_LightPos + g_LightDir, new TGCVector3(0, 0, 1));

            // inicializacion standard:
            this.shadowEffect.SetValue("g_mProjLight", g_mShadowProj.ToMatrix());
            this.shadowEffect.SetValue("g_mViewLightProj", (g_LightView * g_mShadowProj).ToMatrix());
            this.shadowEffect.SetValue("g_txShadow", g_pShadowMap);
            //   mesh.Effect.SetValue("materialSpecularColor", DiffuseModifier);
        }
        public void RenderMyShadow(TgcMesh mesh)
        {
            mesh.Effect = this.shadowEffect;
            mesh.Technique = "RenderShadow";
            g_LightPos = new TGCVector3(pointLightPositions.First().X, pointLightPositions.First().Y, pointLightPositions.First().Z);
            g_LightDir = TGCVector3.Empty - g_LightPos;
            this.shadowEffect.SetValue("g_vLightPos", new Vector4(g_LightPos.X, g_LightPos.Y, g_LightPos.Z, 1));
            this.shadowEffect.SetValue("g_vLightDir", new Vector4(g_LightDir.X, g_LightDir.Y, g_LightDir.Z, 1));
            g_LightView = TGCMatrix.LookAtLH(g_LightPos, g_LightPos + g_LightDir, new TGCVector3(0, 0, 1));

            // inicializacion standard:
            this.shadowEffect.SetValue("g_mProjLight", g_mShadowProj.ToMatrix());
            this.shadowEffect.SetValue("g_mViewLightProj", (g_LightView * g_mShadowProj).ToMatrix());
            this.shadowEffect.SetValue("g_txShadow", g_pShadowMap);

            mesh.Render();
        }

        public void DoLightMe(Effect effect)
        {
            effect.SetValue("lightColor", lightColors.First());
            effect.SetValue("lightPosition", pointLightPositions.First());
            effect.SetValue("lightIntensity", pointLightIntensities.First());
            effect.SetValue("lightAttenuation", pointLightAttenuations.First());
            effect.SetValue("materialEmissiveColor", EmissiveModifier);
            effect.SetValue("materialAmbientColor", AmbientModifier);
            effect.SetValue("materialDiffuseColor", DiffuseModifier);
            //   mesh.Effect.SetValue("materialSpecularColor", DiffuseModifier);
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
            this.lightEffect.Dispose();
            this.shadowEffect.Dispose();
        }
    }
}
