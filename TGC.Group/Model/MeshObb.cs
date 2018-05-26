using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.SceneLoader;
using TGC.Core.Mathematica;

namespace TGC.Group.Model
{
    class MeshObb : Collidable
    {
        private TgcMesh mesh;
        private BoundingOrientedBox obb;

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

        public void Render()
        {
            this.mesh.Render();
            this.obb.Render();
        }

        public void Dispose()
        {
            this.mesh.Dispose();
        }

        public TgcMesh getMesh()
        {
            return this.mesh;
        }

        public BoundingOrientedBox getObb()
        {
            return this.obb;
        }

        public void HandleCollisions(Vehiculo car)
        {

        }
    }
}
