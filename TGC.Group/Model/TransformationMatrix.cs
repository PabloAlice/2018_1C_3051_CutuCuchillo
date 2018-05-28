using TGC.Core.Mathematica;

namespace TGC.Group.Model
{
    class TransformationMatrix
    {
        private TGCMatrix translation, rotation, scalation;

        public TransformationMatrix()
        {
            this.translation = TGCMatrix.Translation(new TGCVector3(0,0,0));
            this.rotation = TGCMatrix.RotationYawPitchRoll(0,0,0);
            this.scalation = TGCMatrix.Scaling(new TGCVector3(1,1,1));
        }

        public void Translate(TGCMatrix translation)
        {
            this.translation *= translation;
        }

        public void Rotate(TGCMatrix rotation)
        {
            this.rotation *= rotation;
        }

        public void Scale(TGCMatrix scalation)
        {
            this.scalation *= scalation;
        }

        public TGCMatrix GetTranslation()
        {
            return this.translation;
        }

        public TGCMatrix GetRotation()
        {
            return this.rotation;
        }

        public TGCMatrix GetScalation()
        {
            return this.scalation;
        }

        public void SetTranslation(TGCMatrix translation)
        {
            this.translation = translation;
        }

        public void SetRotation(TGCMatrix rotation)
        {
            this.rotation = rotation;
        }

        public void SetScalation(TGCMatrix scalation)
        {
            this.scalation = scalation;
        }

        public TGCMatrix GetTransformation()
        {
            return this.scalation * this.rotation * this.translation;
        }
    }
}
