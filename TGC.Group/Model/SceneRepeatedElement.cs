using System.Collections.Generic;
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
        }

        public void transform(TGCMatrix transformation)
        {
            foreach(TgcMesh mesh in this.elementos)
            {
                mesh.Transform = transformation;
                mesh.BoundingBox.transform(transformation);
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
