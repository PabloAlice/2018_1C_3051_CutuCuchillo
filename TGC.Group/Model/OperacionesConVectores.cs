using TGC.Core.Mathematica;

namespace TGC.Group.Model
{
    class OperacionesConVectores
    {
        public static TGCVector3 minimaAlturaEntreVectores(TGCVector3 vector1, TGCVector3 vector2)
        {
            return vector1.Y < vector2.Y ? vector1 : vector2;
        }

        public static TGCVector3 maximaAlturaEntreVectores(TGCVector3 vector1, TGCVector3 vector2)
        {
            return vector1.Y > vector2.Y ? vector1 : vector2;
        }

        public static TGCVector3 sumaDeVectores(TGCVector3 vector1, TGCVector3 vector2)
        {
            return new TGCVector3(vector1.X + vector2.X, vector1.Y + vector2.Y, vector1.Z + vector2.Z);
        }

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
            float rotationY = OperacionesConVectores.anguloEntreVectores(new TGCVector3(v1.X, 0, v1.Z), new TGCVector3(v2.X, 0, v2.Z));
            float rotationX = OperacionesConVectores.anguloEntreVectores(new TGCVector3(0, v1.Y, v1.Z), new TGCVector3(0, v2.Y, v2.Z));
            float rotationZ = OperacionesConVectores.anguloEntreVectores(new TGCVector3(v1.X, v1.Y, 0), new TGCVector3(v2.X, v2.Y, 0));

            return TGCMatrix.RotationYawPitchRoll(rotationY, rotationX, rotationZ);
        }

        //Por las dudas si algun dia hace falta
        private TGCMatrix MatrizPorEscalar(TGCMatrix matrix, float escalar)
        {
            var nuevaMatriz = new TGCMatrix();
            nuevaMatriz.M11 = matrix.M11 * escalar;
            nuevaMatriz.M12 = matrix.M12 * escalar;
            nuevaMatriz.M13 = matrix.M13 * escalar;
            nuevaMatriz.M14 = matrix.M14 * escalar;
            nuevaMatriz.M21 = matrix.M21 * escalar;
            nuevaMatriz.M22 = matrix.M22 * escalar;
            nuevaMatriz.M23 = matrix.M23 * escalar;
            nuevaMatriz.M24 = matrix.M24 * escalar;
            nuevaMatriz.M31 = matrix.M31 * escalar;
            nuevaMatriz.M32 = matrix.M32 * escalar;
            nuevaMatriz.M33 = matrix.M33 * escalar;
            nuevaMatriz.M34 = matrix.M34 * escalar;
            nuevaMatriz.M41 = matrix.M41 * escalar;
            nuevaMatriz.M42 = matrix.M42 * escalar;
            nuevaMatriz.M43 = matrix.M43 * escalar;
            nuevaMatriz.M44 = matrix.M44 * escalar;

            return nuevaMatriz;
        }
    }
}
