using TGC.Core.BoundingVolumes;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;

namespace TGC.Group.Model
{
    class Portal
    {
        private TGCVector3 position;
        public TgcMesh mesh;

        public Portal(TGCVector3 position, TGCMatrix transformationMatrix)
        {
            this.position = position;
            this.CreateMesh(transformationMatrix);
            
        }

        public void CreateMesh(TGCMatrix transformationMatrix)
        {
            TgcSceneLoader loader = new TgcSceneLoader();
            TgcScene scene = loader.loadSceneFromFile(GlobalConcepts.GetInstance().GetMediaDir() + "meshCreator\\meshes\\Otros\\Portal\\Portal-TgcScene.xml");
            this.mesh = scene.Meshes[0];
            this.mesh.AutoTransform = false;
            this.mesh.Transform = transformationMatrix;
            this.mesh.BoundingBox.transform(transformationMatrix);
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
            this.mesh.BoundingBox.Render();
        }

        public void Rotate(TGCMatrix Rotation)
        {
            this.mesh.Transform = Rotation * this.mesh.Transform;
        }

        public TgcBoundingAxisAlignBox GetBoundingBox()
        {
            return this.mesh.BoundingBox;
        }
    }
}
