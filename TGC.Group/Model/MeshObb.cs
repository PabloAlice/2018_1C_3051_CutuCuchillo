using TGC.Core.SceneLoader;
using TGC.Core.Mathematica;
using TGC.Core.Collision;
using TGC.Core.BoundingVolumes;

namespace TGC.Group.Model
{
    class MeshObb
    {
        private TgcMesh mesh;
        private BoundingOrientedBox obb;

        public MeshObb(TgcMesh mesh)
        {
            mesh.AutoTransform = false;
            this.mesh = mesh;
            this.obb = new BoundingOrientedBox(this.mesh.BoundingBox);
        }

        public void ActualizarBoundingOrientedBox()
        {
            this.obb.ActualizarBoundingOrientedBox(this.mesh.BoundingBox);
        }

        public void Transform(TGCMatrix transformation)
        {
            this.mesh.Transform = transformation;
            this.mesh.BoundingBox.transform(transformation);
            this.ActualizarBoundingOrientedBox();
        }

        public void Render()
        {
            this.mesh.Render();
            this.obb.Render();
        }

        public void Dispose()
        {
            this.mesh.Dispose();
        }

        public TgcBoundingOrientedBox GetObb()
        {
            return this.obb.GetBoundingOrientedBox();
        }

        private void Collide(Vehicle car)
        {
            TGCVector3 frontVector = car.GetVectorAdelante();
            //TgcRay ray = this.GenerateRay(car.GetLastPosition(), frontVector);
            //TGCVector3 intersectionPoint = this.DetectIntersection(ray);
            //car.SetTranslate(TGCMatrix.Translation(intersectionPoint));
            //TGCVector3 output = this.GenerateOutput(frontVector);
            //car.SetDirection(output);
            
            while (TgcCollisionUtils.testObbObb(car.GetTGCBoundingOrientedBox(), this.GetObb()))
            {
                car.Translate(TGCMatrix.Translation(-frontVector));
                car.Transform();
            }

        }
    }
}
