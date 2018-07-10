using TGC.Core.Mathematica;
using TGC.Core.Geometry;
using TGC.Core.SceneLoader;
using TGC.Core.Collision;
using TGC.Core.Textures;
using System;

namespace TGC.Group.Model
{
    class Plane : Collidable
    {
        TgcPlane plane;
        TgcMesh mesh;
        TGCPlane realPlane;

        public Plane(TGCVector3 minPoint, TGCVector3 maxPoint, TGCVector3 orientation, string fileName, float UCoordinate, float VCoordinate)
        {
            orientation.Normalize();
            this.plane = new TgcPlane(new TGCVector3(0,0,0), new TGCVector3(0,0,0), this.GetPlaneOrientation(orientation), TgcTexture.createTexture(GlobalConcepts.GetInstance().GetMediaDir() + "MeshCreator\\Meshes\\" + fileName), UCoordinate, VCoordinate);
            this.plane.setExtremes(minPoint, maxPoint);
            this.plane.updateValues();
            this.mesh = this.plane.toMesh("plane");
            this.realPlane = TGCPlane.FromPointNormal(minPoint, orientation);
        }
        public void Init()
        {

        }
        public void SetTexture(float u, float v)
        {
            this.plane.UTile = u;
            this.plane.VTile = v;
            this.plane.updateValues();
        }

        public void HandleCollision(ThirdPersonCamera camera)
        {
            return;
            while (IsColliding(camera))
            {
                System.Console.WriteLine("me trabe pues");
                System.Console.WriteLine(camera.Position);
                camera.ZoomIn();
            }
        }

        public TGCPlane GetPlaneOfCollision(TgcRay ray, Vehicle car)
        {
            return this.realPlane;
        }

        public void HandleCollision(Weapon weapon)
        {
            return;
        }

        private TgcPlane.Orientations GetPlaneOrientation(TGCVector3 vector)
        {
            if(vector.X != 0)
            {
                return TgcPlane.Orientations.YZplane;
            }
            else if(vector.Y != 0)
            {
                return TgcPlane.Orientations.XZplane;
            }
            else if(vector.Z != 0)
            {
                return TgcPlane.Orientations.XYplane;
            }
            throw new Exception("Error al crear la pared");
        }

        public bool IsColliding(ThirdPersonCamera camera)
        {
            return (int)TgcCollisionUtils.classifyPlaneAABB(realPlane, camera.aabb) != 1;
        }

        public bool IsColliding(Vehicle car)
        {
            foreach (TGCVector3 point in car.mesh.BoundingBox.computeCorners())
            {
                if (!GlobalConcepts.GetInstance().IsInFrontOf(point, this.realPlane))
                {
                    return true;
                }
            }
            return (int)TgcCollisionUtils.classifyPlaneAABB(this.realPlane, car.mesh.BoundingBox) != 1;
        }

        public void HandleCollision(Vehicle car)
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
                car.Translate(TGCMatrix.Translation(normal * 0.1f));
                car.Transform();
            }
        }

        public bool IsColliding(Weapon weapon)
        {
            //return TgcCollisionUtils.classifyPointPlane(weapon.GetPosition(), this.realPlane) == 0;
            return !GlobalConcepts.GetInstance().IsInFrontOf(weapon.GetPosition(), realPlane);
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
                //plane.BoundingBox.Render();
            }
        }

        private bool IsInView()
        {
            return TgcCollisionUtils.classifyFrustumAABB(GlobalConcepts.GetInstance().GetFrustum(), this.plane.toMesh("plane").BoundingBox) != 0;
        }
    }
}
