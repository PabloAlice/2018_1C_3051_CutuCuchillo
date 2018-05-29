using TGC.Core.BoundingVolumes;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;

namespace TGC.Group.Model
{
    class Portal
    {
        private TGCVector3 position;
        public TgcMesh mesh;
        TGCMatrix transformation;

        public Portal(TGCVector3 position, TGCMatrix transformationMatrix)
        {
            this.position = position;
            this.transformation = transformationMatrix;
            
        }

        public void CreateMesh(TgcMesh mesh)
        {
            this.mesh = mesh;
            this.mesh.AutoTransform = false;
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
            this.Transform();
            this.mesh.Render();
            this.mesh.BoundingBox.Render();
        }

        private void Transform()
        {
            this.mesh.Transform = this.transformation;
            this.mesh.BoundingBox.transform(this.transformation);
        }

        public void Rotate(TGCMatrix Rotation)
        {
            this.transformation = Rotation * this.transformation;
        }

        public TgcBoundingAxisAlignBox GetBoundingBox()
        {
            this.Transform();
            return this.mesh.BoundingBox;
        }
    }
}
