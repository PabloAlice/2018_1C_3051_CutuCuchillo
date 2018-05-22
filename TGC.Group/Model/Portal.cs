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
        private TGCMatrix rotation, scalation, translation;

        public Portal(TGCVector3 scalation, TGCVector3 rotation, TGCVector3 translation, TGCVector3 targetPos, float targetRot)
        {
            this.scalation = TGCMatrix.Scaling(scalation);
            this.rotation = TGCMatrix.RotationYawPitchRoll(rotation.Y, rotation.X, rotation.Z);
            this.translation = TGCMatrix.Translation(translation);
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

        public TGCMatrix GetTransformation()
        {
            return this.scalation * this.rotation * this.translation;
        }

        public void Rotate(TGCMatrix rotation)
        {
            this.rotation = rotation * this.rotation;
        }

    }
}
