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
        protected List<MeshObb> elementos = new List<MeshObb>();
        protected TGCMatrix transformacion;

        public SceneElement(List<TgcMesh> elementos, TGCMatrix transformacion)
        {
            foreach (TgcMesh mesh in elementos)
            {
                this.elementos.Add(new MeshObb(mesh));
            }
            this.transformacion = transformacion;
            this.transform();
        }

        public SceneElement(List<TgcMesh> elementos)
        {
            foreach (TgcMesh mesh in elementos)
            {
                this.elementos.Add(new MeshObb(mesh));
            }
        }

        public TgcBoundingAxisAlignBox GetBoundingAlignBox()
        {
            return this.elementos[0].getMesh().BoundingBox;
        }

        public TGCVector3 GetPosition()
        {
            return TGCVector3.transform(new TGCVector3(0,0,0), this.transformacion);
        }

        public void transform()
        {
            foreach(MeshObb elemento in this.elementos)
            {
                elemento.Transform(this.transformacion);
            }
        }
       

        public virtual void Render()
        {
            foreach (MeshObb elemento in this.elementos)
            {
                //if (Scene.GetInstance().getCamera().IsInView(this.GetBoundingAlignBox()))
                //{
                    elemento.Render();
                //}
            }
        }

        /*public void RenderBoundingBox()
        {
            foreach (MeshObb elemento in this.elementos)
            {
                elemento.obb.Render();
            }
        }*/

        public MeshObb TestColision(MeshObb meshObb)
        {
            /*
             * 
             * 
             * 
             * Revisar esto, ahora deberia chequearse con MeshObb
             * 
             * 
             * 
             *
             */
            foreach (MeshObb elemento in this.elementos)
            {
                if (TgcCollisionUtils.testAABBAABB(meshObb.getMesh().BoundingBox, elemento.getMesh().BoundingBox))
                {
                    return elemento;
                }
            }

            return null;
            
        }

        public void Dispose()
        {
            foreach (MeshObb elemento in this.elementos)
            {
                //no se por que rompe esta garcha
                //elemento.mesh.Dispose();
            }
        }

        /*public void SetColorBoundingBox(Color color)
        {
            foreach (MeshObb elemento in this.elementos)
            {
                elemento.obb.SetRenderColor(color);
            }
        }*/

        public void HandleCollisions(Vehiculo car)
        {
            return;
        }
    }
}
