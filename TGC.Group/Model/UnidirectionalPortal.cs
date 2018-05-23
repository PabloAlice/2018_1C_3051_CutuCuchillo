using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Mathematica;

namespace TGC.Group.Model
{
    class UnidirectionalPortal : TypeOfPortal
    {
        private TGCVector3 targetPosition;

        public UnidirectionalPortal(Portal originPortal, TGCVector3 targetPosition, TGCVector3 outDirection) : base(originPortal, outDirection)
        {
            this.targetPosition = targetPosition;
        }

        public override TGCVector3 GetTargetPosition()
        {
            return this.targetPosition;
        }

        public override void Collide(Vehiculo car)
        {
            car.ChangePosition(TGCMatrix.Translation(this.targetPosition.X, this.targetPosition.Y, this.targetPosition.Z));
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
