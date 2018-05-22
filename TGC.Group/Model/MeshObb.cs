using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.SceneLoader;
using TGC.Core.Mathematica;

namespace TGC.Group.Model
{
    class MeshObb
    {
        public TgcMesh mesh;
        public BoundingOrientedBox obb;

        public MeshObb(TgcMesh mesh)
        {
            mesh.AutoTransform = false;
            this.mesh = mesh;
            this.obb = new BoundingOrientedBox(this.mesh.BoundingBox);
        }

        public void ActualizarBoundingOrientedBox()
        {
            this.obb.ActualizarBoundingOrientedBox(this.mesh.BoundingBox);
        }

        public void Transform(TGCMatrix transformataion)
        {
            this.mesh.Transform = transformataion;
            this.mesh.BoundingBox.transform(transformataion);
            this.ActualizarBoundingOrientedBox();
        }
    }
}
