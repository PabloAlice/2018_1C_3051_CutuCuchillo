using System;
using TGC.Core.Mathematica;
using TGC.Core.Collision;
using TGC.Core.SceneLoader;

namespace TGC.Group.Model
{
    class BidirectionalPortal : TypeOfPortal
    {
        private Portal targetPortal;

        public BidirectionalPortal(Portal originPortal, Portal targetPortal, TGCVector3 outDirection, TgcMesh mesh) : base(originPortal, outDirection)
        {
            this.targetPortal = targetPortal;
            this.originPortal.CreateMesh(mesh);
            this.targetPortal.CreateMesh(mesh);
        }

        override public TGCVector3 GetTargetPosition()
        {
            return targetPortal.GetPosition();
        }

        public override void Dispose()
        {
            base.Dispose();
            this.targetPortal.Dispose();
        }

        public override void Collide(Vehicle car)
        {
            TGCVector3 newPosition = this.targetPortal.GetPosition();
            TGCMatrix translation = TGCMatrix.Translation(0,0,0);
            car.ChangePosition(TGCMatrix.Translation(newPosition.X, newPosition.Y, newPosition.Z));
            while (TgcCollisionUtils.testObbAABB(car.GetTGCBoundingOrientedBox(), this.targetPortal.GetBoundingBox()))
            {
                translation *= TGCMatrix.Translation(this.outDirection * 0.1f);
                car.ChangePosition(TGCMatrix.Translation(newPosition) * translation);
            }

            var dot = TGCVector3.Dot(car.vectorAdelante, this.outDirection);
            var modulusProduct = car.vectorAdelante.Length() * this.outDirection.Length();
            var acos = (float)Math.Acos(dot / modulusProduct);
            var yCross = TGCVector3.Cross(car.vectorAdelante, this.outDirection).Y;
            car.Girar((yCross > 0) ? acos : -acos);
            if (car.GetVelocidadActual() < 0)
            {
                car.Girar(FastMath.PI);
            }
        }
    }
}
