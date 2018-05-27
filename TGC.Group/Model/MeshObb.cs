using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public void ActualizarBoundingOrientedBox()
        {
            this.obb.ActualizarBoundingOrientedBox(this.mesh.BoundingBox);
        }

        public TgcBoundingAxisAlignBox GetBoundingAlignBox()
        {
            return this.mesh.BoundingBox;
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

        public TgcMesh GetMesh()
        {
            return this.mesh;
        }

        public TgcBoundingOrientedBox GetObb()
        {
            return this.obb.GetBoundingOrientedBox();
        }

        public void HandleCollisions(Vehicle car)
        {
            if(TgcCollisionUtils.testObbObb(car.GetTGCBoundingOrientedBox(), this.GetObb()))
            {
                this.Collide(car);
            }
        }

        private TGCVector3 DetectIntersection(TgcRay ray)
        {
            TGCVector3 intersection = new TGCVector3();
            TgcCollisionUtils.intersectRayObb(ray, this.GetObb(), out intersection);
            return intersection;

        }

        private TgcRay GenerateRay(TGCVector3 origin, TGCVector3 direction)
        {
            TgcRay ray = new TgcRay();
            ray.Origin = origin;
            ray.Direction = direction;
            return ray;
        }

        private TGCVector3 GenerateOutput(TGCVector3 vector)
        {
            if((vector.X >= 0 && vector.Z >=0) || (vector.X < 0 && vector.Z > 0))
            {
                return new TGCVector3(-vector.X, vector.Y, vector.Z);
            }
            else
            {
                return new TGCVector3(vector.X, vector.Y, vector.Z);
            }
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
