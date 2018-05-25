using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using TGC.Core.BoundingVolumes;
using TGC.Core.Mathematica;

namespace TGC.Group.Model
{
    class BoundingOrientedBox
    {
        private TgcBoundingOrientedBox obb;
        private float rotacionObb = 0f;

        public BoundingOrientedBox(TgcBoundingAxisAlignBox AABB)
        {
            this.ActualizarBoundingOrientedBox(AABB);
        }

        public void Render()
        {
            this.obb.Render();
        }

        public void ActualizarBoundingOrientedBox(TgcBoundingAxisAlignBox AABB)
        {
            this.obb = TgcBoundingOrientedBox.computeFromAABB(AABB);
            this.obb.rotate(new TGCVector3(0, this.rotacionObb, 0));
            this.obb.updateValues();
        }

        public void Rotate(float rotacion)
        {
            this.rotacionObb += rotacion;
        }

        public void SetRenderColor(Color color)
        {
            this.obb.setRenderColor(color);
        }

        public TgcBoundingOrientedBox GetBoundingOrientedBox()
        {
            return this.obb;
        }
    }
}
