using TGC.Core.SceneLoader;
using TGC.Core.Mathematica;
using TGC.Core.Collision;
using TGC.Core.BoundingVolumes;

namespace TGC.Group.Model
{
    class MeshObb : Collidable
    {
        private TgcMesh mesh;
        private BoundingOrientedBox obb;

        public MeshObb(TgcMesh mesh)
        {
            mesh.AutoTransform = false;
            this.mesh = mesh;
            this.obb = new BoundingOrientedBox(this.mesh.BoundingBox);
        }

        public TGCVector3 GetPosition()
        {
            //TODO esta asi por que no tengo la matrix de transformacion
            return new TGCVector3(0,0,0);
        }

        public bool IsColliding(Weapon weapon, out Collidable element)
        {
            
            if(TgcCollisionUtils.testSphereOBB(weapon.sphere, this.obb.GetBoundingOrientedBox()))
            {
                element = this;
                return true;
            }
            element = null;
            return false;
        }

        public void HandleCollisions(Vehicle car)
        {
            if (TgcCollisionUtils.testObbObb(car.GetTGCBoundingOrientedBox(), this.GetObb()))
            {
                this.Collide(car);
            }
        }

        public TgcMesh GetCollidable(Vehicle car)
        {
            //return this.mesh;
            return null;
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
