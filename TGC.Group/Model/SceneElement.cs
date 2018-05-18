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
        protected List<TgcMesh> elementos;

        public SceneElement(List<TgcMesh> elementos, TGCMatrix transformacion)
        {
            this.elementos = elementos;
            transform(transformacion);
        }

        public SceneElement(List<TgcMesh> elementos)
        {
            this.elementos = elementos;
        }

        public void transform(TGCMatrix transformacion)
        {
            foreach(TgcMesh elemento in this.elementos)
            {
                elemento.AutoTransform = false;
                elemento.Transform = transformacion;
                elemento.BoundingBox.transform(transformacion);
            }
        }
       

        public virtual void Render()
        {
            foreach (TgcMesh elemento in this.elementos)
            {
                elemento.Render();
                //por ahora, para ver los boundings
                elemento.BoundingBox.Render();
            }
        }

        public void RenderBoundingBox()
        {
            foreach (TgcMesh elemento in this.elementos)
            {
                elemento.BoundingBox.Render();
            }
        }

        public TgcMesh TestColision(TgcMesh mesh)
        {

            foreach (TgcMesh elemento in this.elementos)
            {
                if (TgcCollisionUtils.testAABBAABB(mesh.BoundingBox, elemento.BoundingBox))
                {
                    return elemento;
                }
            }

            return null;
            
        }

        public void Dispose()
        {
            foreach (TgcMesh elemento in this.elementos)
            {
                elemento.Dispose();
            }
        }

        public void SetColorBoundingBox(Color color)
        {
            foreach (TgcMesh elemento in this.elementos)
            {
                elemento.BoundingBox.setRenderColor(color);
            }
        }
    }
}
