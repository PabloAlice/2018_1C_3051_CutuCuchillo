using System.Collections.Generic;
using TGC.Core.SceneLoader;
using TGC.Core.Mathematica;
using TGC.Core.Collision;
using TGC.Core.BoundingVolumes;

namespace TGC.Group.Model
{
    class SceneElement : Collidable
    {
        protected List<TgcMesh> elements = new List<TgcMesh>();
        protected TGCMatrix transformacion;

        public SceneElement(List<TgcMesh> elementos, TGCMatrix transformacion)
        {
            foreach (TgcMesh mesh in elementos)
            {
                mesh.AutoTransform = false;
                this.elements.Add(mesh);
            }
            this.transformacion = transformacion;
        }

        public bool IsColliding(Weapon weapon, out Collidable elementOut)
        {
            foreach (TgcMesh element in this.elements)
            {
                if(TgcCollisionUtils.testSphereAABB(weapon.sphere, element.BoundingBox))
                {
                    elementOut = this;
                    return true;
                }
            }
            elementOut = null;
            return false;
        }

        public TgcMesh GetCollidable(Vehicle car)
        {
            foreach (TgcMesh elemento in this.elements)
            {
                if(TgcCollisionUtils.testObbAABB(car.GetTGCBoundingOrientedBox(), elemento.BoundingBox))
                {
                    return elemento;
                }
            }

            return null;
        }

        public SceneElement(List<TgcMesh> elementos)
        {
            foreach (TgcMesh mesh in elementos)
            {
                mesh.AutoTransform = false;
                this.elements.Add(mesh);
            }
        }

        public TgcBoundingAxisAlignBox GetBoundingAlignBox()
        {
            return this.elements[0].BoundingBox;
        }

        public TGCVector3 GetPosition()
        {
            return TGCVector3.transform(new TGCVector3(0,0,0), this.transformacion);
        }

        public virtual void transform()
        {
            foreach(TgcMesh elemento in this.elements)
            {
                elemento.Transform = this.transformacion;
                elemento.BoundingBox.transform(this.transformacion);
            }
        }
       

        public virtual void Render()
        {
            this.transform();
            foreach (TgcMesh elemento in this.elements)
            {
                if (Scene.GetInstance().camera.IsInView(elemento.BoundingBox))
                {
                    Lighting.LightManager.GetInstance().DoLightMe(elemento);
                    elemento.Render();
                    elemento.BoundingBox.Render();
                }
            }
        }

        public void Dispose()
        {
            foreach (TgcMesh elemento in this.elements)
            {
                //elemento.Dispose();
            }
        }

        private TGCVector3 GenerateOutput(TGCVector3 vector)
        {
            if ((vector.X >= 0 && vector.Z >= 0) || (vector.X < 0 && vector.Z > 0))
            {
                return new TGCVector3(-vector.X, vector.Y, vector.Z);
            }
            else
            {
                return new TGCVector3(vector.X, vector.Y, vector.Z);
            }
        }

        private bool IsInFrontOf(TGCVector3 testpoint, TGCPlane plane)
        {
            return plane.A * testpoint.X + plane.B * testpoint.Y + plane.C * testpoint.Z + plane.D > 0;
        }

        private TGCPlane SelectPlane(List<TGCPlane> planes, TGCVector3 testPoint)
        {
            planes.Sort((x,y) => IsInFrontOf(testPoint, x).CompareTo(IsInFrontOf(testPoint, y)));
            planes.Reverse();
            return planes[0];
        }

        private TGCPlane CreatePlane(TgcRay ray, TgcBoundingAxisAlignBox.Face[] faces, TGCVector3 testPoint)
        {
            float instante;
            TGCVector3 intersection;
            List<TGCPlane> candidatesPlanes = new List<TGCPlane>();
            int loop = 0;
            foreach (TgcBoundingAxisAlignBox.Face face in faces)
            {
                if (TgcCollisionUtils.intersectRayPlane(ray, face.Plane, out instante, out intersection))
                {
                    candidatesPlanes.Add(face.Plane);
                }
                loop++;
            }

            return this.SelectPlane(candidatesPlanes, testPoint);

        }

        private void Collide(TgcMesh elemento, Vehicle car)
        {
            car.SoundsManager.Crash();
            car.Crash();
            //direccion a la que estoy yendo antes de chocar
            TGCVector3 directionOfCollision = car.GetDirectionOfCollision();
            TgcRay ray = new TgcRay();
            ray.Origin = car.GetLastPosition();
            ray.Direction = directionOfCollision;
            //interseco el rayo con el aabb, para conocer un punto del plano con el que colisione
            TgcBoundingAxisAlignBox.Face[] faces;
            faces = elemento.BoundingBox.computeFaces();
            TGCPlane plane = this.CreatePlane(ray, faces, car.GetLastPosition());
            TGCVector3 normal = GlobalConcepts.GetInstance().GetNormalPlane(plane);
            TGCVector3 output = new TGCVector3(normal.X + directionOfCollision.X, normal.Y + directionOfCollision.Y, normal.Z + directionOfCollision.Z);
            float angle = car.SetDirection(output, normal);
            Scene.GetInstance().camera.rotateY(angle);

            while (TgcCollisionUtils.testObbAABB(car.GetTGCBoundingOrientedBox(), elemento.BoundingBox))
            {
                car.Translate(TGCMatrix.Translation(-directionOfCollision * 0.1f));
                car.Transform();
            }
        }

        private bool IsColliding(TgcMesh elemento, Vehicle car)
        {
            return TgcCollisionUtils.testObbAABB(car.GetTGCBoundingOrientedBox(), elemento.BoundingBox);
            
        }

        public void HandleCollisions(Vehicle car)
        {
            this.transform();
            foreach (TgcMesh elemento in this.elements)
            {
                
                if (this.IsColliding(elemento, car)) {
                    this.Collide(elemento, car);
                    return;
                }
            }
        }
    }
}
