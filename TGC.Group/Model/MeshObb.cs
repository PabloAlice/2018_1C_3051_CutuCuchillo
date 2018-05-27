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

        public void Transform(TGCMatrix transformataion)
        {
            this.mesh.Transform = transformataion;
            this.mesh.BoundingBox.transform(transformataion);
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

        private TGCVector3 GetNormalPlane(TGCVector3 direccion)
        {
            return -direccion;
            
        }

        private void Collide(Vehicle car)
        {
            
            TgcRay ray = this.GenerateRay(car.GetLastPosition(), car.GetVectorAdelante());
            TGCVector3 intersectionPoint = this.DetectIntersection(ray);
            TGCVector3 normalPlane = this.GetNormalPlane(intersectionPoint);
            normalPlane.Normalize();
            car.SetTranslate(TGCMatrix.Translation(intersectionPoint));
            
            while (TgcCollisionUtils.testObbObb(car.GetTGCBoundingOrientedBox(), this.GetObb()))
            {
                car.Translate(TGCMatrix.Translation(-car.GetVectorAdelante()));
                car.Transform();
            }
        }
    }
}
