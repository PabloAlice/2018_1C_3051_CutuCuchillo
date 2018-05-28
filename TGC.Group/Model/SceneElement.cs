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

        private bool IsInPlane(TGCVector3 point, TGCPlane plane)
        {
            float result = plane.A * point.X + plane.B * point.Y + plane.C * point.Z + plane.D;
            float epsilon = 0f;
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

        private void Collide(TgcMesh elemento, Vehicle car)
        {
            //direccion a la que estoy yendo antes de chocar
            TGCVector3 directionOfCollision = car.GetDirectionOfCollision();
            //creo un rayo en direccion al choque
            TgcRay ray = new TgcRay();
            ray.Origin = car.GetLastPosition();
            ray.Direction = directionOfCollision;
            //interseco el rayo con el aabb, para conocer un punto del plano con el que colisione
            TGCVector3 intersectionPoint = new TGCVector3();
            TgcCollisionUtils.intersectRayAABB(ray, elemento.BoundingBox, out intersectionPoint);
            //obtengo las caras del bounding y me fijo cual es el plano que tiene incluido al 
            //punto que averigue previamente
            TgcBoundingAxisAlignBox.Face[] faces;
            faces = elemento.BoundingBox.computeFaces();
            TGCPlane plane = this.CreatePlane(intersectionPoint, faces);
            //averiguo el vector de salida
            TGCVector3 output = GlobalConcepts.GetInstance().GetNormalPlane(plane) + directionOfCollision;
            //giro el auto para que quede mirando hacia la direccion de salidaw
            car.SetDirection(output);

            while (TgcCollisionUtils.testObbAABB(car.GetTGCBoundingOrientedBox(), elemento.BoundingBox))
            {
                car.Translate(TGCMatrix.Translation(-directionOfCollision * 0.001f));
                car.Transform();
            }
        }

        private bool IsColliding(TgcMesh elemento, Vehicle car)
        {
            return TgcCollisionUtils.testObbAABB(car.GetTGCBoundingOrientedBox(), elemento.BoundingBox);
            
        }

        public void HandleCollisions(Vehicle car)
        {
            foreach(TgcMesh elemento in this.elementos)
            {
                if (this.IsColliding(elemento, car)) {
                    this.Collide(elemento, car);
                    return;
                }
            }
        }
    }
}
