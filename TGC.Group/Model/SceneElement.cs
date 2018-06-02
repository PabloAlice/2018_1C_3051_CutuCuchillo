using System.Collections.Generic;
using TGC.Core.SceneLoader;
using TGC.Core.Mathematica;
using TGC.Core.Collision;
using TGC.Core.BoundingVolumes;

namespace TGC.Group.Model
{
    class SceneElement : Collidable
    {
        protected List<TgcMesh> elementos = new List<TgcMesh>();
        protected TGCMatrix transformacion;

        public SceneElement(List<TgcMesh> elementos, TGCMatrix transformacion)
        {
            foreach (TgcMesh mesh in elementos)
            {
                mesh.AutoTransform = false;
                this.elementos.Add(mesh);
            }
            this.transformacion = transformacion;
        }

        public TgcMesh GetCollidable(Vehicle car)
        {
            foreach (TgcMesh elemento in this.elementos)
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
                this.elementos.Add(mesh);
            }
        }

        public TgcBoundingAxisAlignBox GetBoundingAlignBox()
        {
            return this.elementos[0].BoundingBox;
        }

        public TGCVector3 GetPosition()
        {
            return TGCVector3.transform(new TGCVector3(0,0,0), this.transformacion);
        }

        public virtual void transform()
        {
            foreach(TgcMesh elemento in this.elementos)
            {
                elemento.Transform = this.transformacion;
                elemento.BoundingBox.transform(this.transformacion);
            }
        }
       

        public virtual void Render()
        {
            this.transform();
            foreach (TgcMesh elemento in this.elementos)
            {
                if (Scene.GetInstance().getCamera().IsInView(elemento.BoundingBox))
                {
                    elemento.Render();
                    elemento.BoundingBox.Render();
                }
            }
        }

        public void Dispose()
        {
            foreach (TgcMesh elemento in this.elementos)
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
                System.Console.WriteLine("Cara{0}: ({1},{2},{3})", loop, face.Plane.A, face.Plane.B, face.Plane.C);
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
            car.SetDirection(output, normal);
            System.Console.WriteLine("Direccion de Colision: ({0},{1},{2})", directionOfCollision.X, directionOfCollision.Y, directionOfCollision.Z);
            System.Console.WriteLine("Normal: ({0},{1},{2})", normal.X, normal.Y, normal.Z);
            System.Console.WriteLine("OutPut: ({0},{1},{2})", output.X, output.Y, output.Z);

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
            foreach (TgcMesh elemento in this.elementos)
            {
                
                if (this.IsColliding(elemento, car)) {
                    this.Collide(elemento, car);
                    return;
                }
            }
        }
    }
}
