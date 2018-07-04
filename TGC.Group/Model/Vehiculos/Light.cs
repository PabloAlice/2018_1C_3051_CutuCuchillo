using System.Collections.Generic;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Core.Shaders;

namespace TGC.Group.Model.Vehiculos
{
    class Light
    {

        private TGCMatrix matrix;
        private List<TgcMesh> meshes = new List<TgcMesh>();

        public Light(string path)
        {
            TgcSceneLoader loader = new TgcSceneLoader();
            TgcScene scene = loader.loadSceneFromFile(path);
            meshes = scene.Meshes;
            foreach (TgcMesh mesh in meshes)
            {
                mesh.AutoTransform = false;
                mesh.Effect = TgcShaders.loadEffect(GlobalConcepts.GetInstance().GetShadersDir() + "Faros.fx");
                mesh.Technique = "Normal";
            }
            this.matrix = TGCMatrix.Identity;
        }

        public void Render()
        {
            meshes.ForEach(m => m.Render());
        }

        public void Dispose()
        {
            meshes.ForEach(m => m.Dispose());
        }

        public void SetTransformation(TGCMatrix transformation)
        {
            this.matrix = transformation;
        }

        public void ActivateLight()
        {
            this.meshes.ForEach(m => m.Technique = "Iluminate");
        }

        public void DesactivateLight()
        {
            this.meshes.ForEach(m => m.Technique = "Normal");
        }

        public void Transform()
        {
            foreach (TgcMesh mesh in meshes)
            {
                mesh.Transform = this.matrix;
            }
        }
    }
}
