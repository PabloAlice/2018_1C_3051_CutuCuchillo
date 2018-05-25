using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;

namespace TGC.Group.Model
{
    class SceneRepeatedElement : SceneElement
    {

        private List<TGCMatrix> transformations;

        public SceneRepeatedElement(List<TgcMesh> meshList, List<TGCMatrix> transformations) : base(meshList)
        {
            this.transformations = transformations;
            foreach (TgcMesh mesh in meshList)
            {
                this.elementos.Add(new MeshObb(mesh));
            }
        }

        public override void Render()
        {
            foreach (TGCMatrix transformation in transformations)
            {
               transform(transformation);
               base.Render();
            }
        }       
    }
}
