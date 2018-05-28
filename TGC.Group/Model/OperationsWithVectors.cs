using TGC.Core.Mathematica;

namespace TGC.Group.Model
{
    class OperationsWithVectors
    {

        public static float anguloEntreVectores(TGCVector3 vector1, TGCVector3 vector2)
        {
            if((TGCVector3.Length(vector1) * TGCVector3.Length(vector2)) == 0)
            {
                return 0;
            }

            float cos = TGCVector3.Dot(vector1, vector2) / (TGCVector3.Length(vector1) * TGCVector3.Length(vector2));
            
            return FastMath.Acos(cos);
        }

        public static TGCMatrix rotacionEntreVectores(TGCVector3 v1, TGCVector3 v2)
        {
            float rotationY = OperationsWithVectors.anguloEntreVectores(new TGCVector3(v1.X, 0, v1.Z), new TGCVector3(v2.X, 0, v2.Z));
            float rotationX = OperationsWithVectors.anguloEntreVectores(new TGCVector3(0, v1.Y, v1.Z), new TGCVector3(0, v2.Y, v2.Z));
            float rotationZ = OperationsWithVectors.anguloEntreVectores(new TGCVector3(v1.X, v1.Y, 0), new TGCVector3(v2.X, v2.Y, 0));

            return TGCMatrix.RotationYawPitchRoll(rotationY, rotationX, rotationZ);
        }



    }
}
