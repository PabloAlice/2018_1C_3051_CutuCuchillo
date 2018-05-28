using System.Drawing;
using TGC.Core.BoundingVolumes;
using TGC.Core.Mathematica;

namespace TGC.Group.Model
{
    class BoundingOrientedBox
    {
        private TgcBoundingOrientedBox obb;
        private float rotacionObb = 0f;
        private float y;

        public BoundingOrientedBox(TgcBoundingAxisAlignBox AABB)
        {
            this.obb = TgcBoundingOrientedBox.computeFromAABB(AABB);
            this.y = this.obb.Position.Y;
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
        public void ActualizarBoundingOrientedBox(TGCMatrix translation)
        {
            this.obb.Center = new TGCVector3(0, this.y, 0) + TGCVector3.transform(new TGCVector3(0, 0, 0), translation);
            //this.obb.move(TGCVector3.transform(new TGCVector3(0, 0, 0), translation));
            this.obb.setRotation(new TGCVector3(0, this.rotacionObb, 0));
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
