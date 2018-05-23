using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;

namespace TGC.Group.Model
{
    class Portal
    {
        TGCVector3 position;
        TgcMesh mesh;

        public Portal(TGCVector3 position, TGCMatrix transformationMatrix)
        {
            this.position = position;
            this.CreateMesh(transformationMatrix);
            
        }

        public void CreateMesh(TGCMatrix transformationMatrix)
        {
            TgcSceneLoader loader = new TgcSceneLoader();
            TgcScene scene = loader.loadSceneFromFile(ConceptosGlobales.GetInstance().GetMediaDir() + "meshCreator\\meshes\\Otros\\Portal\\Portal-TgcScene.xml");
            this.mesh = scene.Meshes[0];
            this.mesh.AutoTransform = false;
            this.mesh.Transform = transformationMatrix;
        }

        public TGCVector3 GetPosition()
        {
            return this.position;
        }

        public void Dispose()
        {
            //no se por que rompe esta mierda
            //this.mesh.Dispose();
        }

        public void Render()
        {
            this.Rotate(TGCMatrix.RotationZ(0.05f));
            this.mesh.Render();
        }

        public void Rotate(TGCMatrix Rotation)
        {
            this.mesh.Transform = Rotation * this.mesh.Transform;
        }
    }
}
