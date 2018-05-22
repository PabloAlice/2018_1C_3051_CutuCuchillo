using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Mathematica;

namespace TGC.Group.Model
{
    class Portal
    {
        private TGCVector3 targetPosition;
        private float targetRotation;
        private TGCMatrix position;

        public Portal(TGCMatrix position, TGCVector3 targetPos, float targetRot)
        {
            this.position = position;
            this.targetPosition = targetPos;
            this.targetRotation = targetRot;
        }

        public TGCVector3 getTargetPosition()
        {
            return targetPosition;
        }

        public float getTargetRotation()
        {
            return targetRotation;
        }

        public TGCMatrix getPosition()
        {
            return position;
        }

    }
}
