using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.SceneLoader;
using TGC.Core.Mathematica;
using TGC.Core.Collision;
using System.Drawing;
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
            this.transform();
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

        private TgcRay GenerateRay(TGCVector3 origin, TGCVector3 direction)
        {
            TgcRay ray = new TgcRay();
            ray.Origin = origin;
            ray.Direction = direction;
            return ray;
        }

        private TGCVector3 DetectIntersection(TgcRay ray, TgcMesh elemento)
        {
            TGCVector3 intersection = new TGCVector3();
            TgcCollisionUtils.intersectRayAABB(ray, elemento.BoundingBox, out intersection);
            return intersection;

        }

        private bool IsInPlane(TGCVector3 point, TGCPlane plane)
        {
            float result = plane.A * point.X + plane.B * point.Y + plane.C * point.Z + plane.D;
            float epsilon = 0.1f;
            return (result < epsilon && result > -epsilon) ? true : false;
        }

        private TGCPlane CreatePlane(TGCVector3 punto, TgcBoundingAxisAlignBox.Face[] faces)
        {
            foreach (TgcBoundingAxisAlignBox.Face face in faces)
            {
                if(this.IsInPlane(punto, face.Plane))
                {
                    return face.Plane;
                }
            }
            return faces[0].Plane;
        }

        private TGCVector3 normal(TGCPlane plane)
        {
            return new TGCVector3(plane.A, plane.B, plane.C);
        }

        private void Collide(TgcMesh elemento, Vehicle car)
        {
            TGCVector3 frontVector = car.GetVectorAdelante();
            TgcRay ray = this.GenerateRay(car.GetLastPosition(), frontVector);
            TGCVector3 intersectionPoint = this.DetectIntersection(ray, elemento);
            TgcBoundingAxisAlignBox.Face[] faces;
            faces = elemento.BoundingBox.computeFaces();
            TGCPlane plane = this.CreatePlane(intersectionPoint, faces);
            TGCVector3 output = this.normal(plane);
            car.SetDirection(output);

            while (TgcCollisionUtils.testObbAABB(car.GetTGCBoundingOrientedBox(), elemento.BoundingBox))
            {
                car.Translate(TGCMatrix.Translation(-frontVector * 0.01f));
                car.Transform();
            }
        }

        private void DetectCollision(TgcMesh elemento, Vehicle car)
        {
            if (TgcCollisionUtils.testObbAABB(car.GetTGCBoundingOrientedBox(), elemento.BoundingBox))
            {
                this.Collide(elemento, car);
            }
        }

        public void HandleCollisions(Vehicle car)
        {
            foreach(TgcMesh elemento in this.elementos)
            {
                //faltaria terminar el foreach cuando se encontro el collisionado
                this.DetectCollision(elemento, car);
            }
            return;
        }
    }
}
