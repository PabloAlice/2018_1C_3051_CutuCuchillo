using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.SceneLoader;
using TGC.Core.Mathematica;
using TGC.Core.Collision;
using System.Drawing;

namespace TGC.Group.Model
{
    class SceneElement
    {
        protected List<MeshObb> elementos = new List<MeshObb>();

        public SceneElement(List<TgcMesh> elementos, TGCMatrix transformacion)
        {
            foreach (TgcMesh mesh in elementos)
            {
                this.elementos.Add(new MeshObb(mesh));
            }
            this.transform(transformacion);
        }

        public SceneElement(List<TgcMesh> elementos)
        {
            foreach (TgcMesh mesh in elementos)
            {
                this.elementos.Add(new MeshObb(mesh));
            }
        }

        public void transform(TGCMatrix transformacion)
        {
            foreach(MeshObb elemento in this.elementos)
            {
                elemento.Transform(transformacion);
            }
        }
       

        public virtual void Render()
        {
            foreach (MeshObb elemento in this.elementos)
            {
                elemento.mesh.Render();
                elemento.obb.Render();
            }
        }

        public void RenderBoundingBox()
        {
            foreach (MeshObb elemento in this.elementos)
            {
                elemento.obb.Render();
            }
        }

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
                if (TgcCollisionUtils.testAABBAABB(meshObb.mesh.BoundingBox, elemento.mesh.BoundingBox))
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

        public void SetColorBoundingBox(Color color)
        {
            foreach (MeshObb elemento in this.elementos)
            {
                elemento.obb.SetRenderColor(color);
            }
        }
    }
}
