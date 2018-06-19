using TGC.Core.Mathematica;
using TGC.Core.Geometry;
using TGC.Core.SceneLoader;
using TGC.Core.Collision;
using TGC.Core.Textures;
using System;

namespace TGC.Group.Model
{
    class Planes : Collidable
    {
        TgcPlane plane;
        TgcMesh mesh;
        TGCPlane realPlane;

        public Planes(TGCVector3 origin, TGCVector3 size, TGCVector3 orientation, string fileName)
        {
            orientation.Normalize();
            this.plane = new TgcPlane(origin, size, this.GetPlane(orientation), TgcTexture.createTexture(GlobalConcepts.GetInstance().GetMediaDir() + fileName));
            this.mesh = this.plane.toMesh("plane");
            this.realPlane = TGCPlane.FromPointNormal(origin, orientation);
        }

        private TgcPlane.Orientations GetPlane(TGCVector3 vector)
        {
            if(vector.X > 0)
            {
                return TgcPlane.Orientations.YZplane;
            }
            else if(vector.Y > 0)
            {
                return TgcPlane.Orientations.XZplane;
            }
            else if(vector.Z > 0)
            {
                return TgcPlane.Orientations.XYplane;
            }
            throw new Exception("Error al crear la pared");
        }

        private bool IsColliding(Vehicle car)
        {
            return TgcCollisionUtils.testObbAABB(car.GetTGCBoundingOrientedBox(), this.mesh.BoundingBox) || !GlobalConcepts.GetInstance().IsInFrontOf(car.GetPosition(), this.realPlane);
        }

        public void HandleCollisions(Vehicle car)
        {
            if (this.IsColliding(car))
            {
                this.Collide(car);
                return;
            }
        }

        private void Collide(Vehicle car)
        {
            //direccion a la que estoy yendo antes de chocar
            TGCVector3 directionOfCollision = car.GetDirectionOfCollision();
            TGCVector3 normal = GlobalConcepts.GetInstance().GetNormalPlane(this.realPlane);
            TGCVector3 output = normal + directionOfCollision * 2;
            float angle = car.SetDirection(output, normal);
            car.Crash(angle);

            while (IsColliding(car))
            {
                car.Translate(TGCMatrix.Translation(-directionOfCollision * 0.1f));
                car.Transform();
            }
        }

        public bool IsColliding(Weapon weapon, out Collidable element)
        {
            if (TgcCollisionUtils.testSphereAABB(weapon.sphere, this.mesh.BoundingBox))
            {
                element = this;
                return true;
            }
            element = null;
            return false;
        }

        public bool IsInto(TGCVector3 minPoint, TGCVector3 maxPoint)
        {
            return GlobalConcepts.GetInstance().IsBetweenXZ(this.GetPosition(), minPoint, maxPoint);
        }

        public TGCVector3 GetPosition()
        {
            return plane.Position;
        }

        public TgcMesh GetCollidable(Vehicle car)
        {
            return this.mesh;
        }

        public void Dispose()
        {
            this.plane.Dispose();
        }
        
        public void Render()
        {
            if (this.IsInView())
            {
                plane.Render();
            }
        }

        private bool IsInView()
        {
            return TgcCollisionUtils.classifyFrustumAABB(GlobalConcepts.GetInstance().GetFrustum(), this.plane.toMesh("plane").BoundingBox) != 0;
        }
    }
}
