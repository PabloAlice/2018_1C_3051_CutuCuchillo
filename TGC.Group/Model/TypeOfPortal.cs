using TGC.Core.Mathematica;
using TGC.Core.BoundingVolumes;
using TGC.Core.Collision;
using System.Drawing;

namespace TGC.Group.Model
{
    abstract class TypeOfPortal : Collidable
    {
        protected Portal originPortal;
        protected TGCVector3 outDirection;

        public TypeOfPortal(Portal originPortal, TGCVector3 outDirection)
        {
            this.outDirection = outDirection;
            this.originPortal = originPortal;
        }

        public TgcBoundingAxisAlignBox GetBoundingAlignBox()
        {
            return this.originPortal.mesh.BoundingBox;
        }

        abstract public TGCVector3 GetTargetPosition();

        virtual public void Dispose()
        {
            this.originPortal.Dispose();
        }

        public void Render()
        {
            this.originPortal.Render();
        }

        public TgcBoundingAxisAlignBox GetBoundingBox()
        {
            return this.originPortal.GetBoundingBox();
        }

        public void HandleCollisions(Vehicle car)
        {
            if (TgcCollisionUtils.testObbAABB(car.GetTGCBoundingOrientedBox(), this.GetBoundingBox())){
                this.GetBoundingBox().setRenderColor(Color.Red);
                this.Collide(car);
            }
        }

        abstract public void Collide(Vehicle car);

        public TGCVector3 GetPosition()
        {
            return this.originPortal.GetPosition();
        }
    }
}
