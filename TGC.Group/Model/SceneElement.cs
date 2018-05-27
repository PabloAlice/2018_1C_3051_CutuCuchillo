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

        private void Collide(TgcMesh elemento, Vehicle car)
        {
            TGCVector3 frontVector = car.GetVectorAdelante();
            //TgcRay ray = this.GenerateRay(car.GetLastPosition(), frontVector);
            //TGCVector3 intersectionPoint = this.DetectIntersection(ray);
            //car.SetTranslate(TGCMatrix.Translation(intersectionPoint));
            //TGCVector3 output = this.GenerateOutput(frontVector);
            //car.SetDirection(output);

            while (TgcCollisionUtils.testObbAABB(car.GetTGCBoundingOrientedBox(), elemento.BoundingBox))
            {
                car.Translate(TGCMatrix.Translation(-frontVector));
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
