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

        public UnidirectionalPortal(Portal originPortal, TGCVector3 targetPosition) : base(originPortal)
        {
            this.targetPosition = targetPosition;
        }

        public override TGCVector3 GetTargetPosition()
        {
            return this.targetPosition;
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
