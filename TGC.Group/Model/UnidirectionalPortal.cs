using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Group.Model.Vehiculos;

namespace TGC.Group.Model
{
    class UnidirectionalPortal : TypeOfPortal
    {
        private TGCVector3 targetPosition;

        public UnidirectionalPortal(Portal originPortal, TGCVector3 targetPosition, TGCVector3 outDirection, TgcMesh mesh) : base(originPortal, outDirection)
        {
            this.targetPosition = targetPosition;
            this.originPortal.CreateMesh(mesh);
        }

        public override TGCVector3 GetTargetPosition()
        {
            return this.targetPosition;
        }

        public override void Collide(Vehicle car)
        {
            car.ChangePosition(TGCMatrix.Translation(this.targetPosition.X, this.targetPosition.Y, this.targetPosition.Z));
            base.Collide(car);
        }

        /*
        public TGCVector3 getTargetPosition()
        {
            return targetPosition;
        }

        public float getTargetRotation()
        {
            return targetRotation;
        }

        public TGCMatrix GetTransformation()
        {
            return this.scalation * this.rotation * this.translation;
        }

        public void Rotate(TGCMatrix rotation)
        {
            this.rotation = rotation * this.rotation;
        }
        */

    }
}
