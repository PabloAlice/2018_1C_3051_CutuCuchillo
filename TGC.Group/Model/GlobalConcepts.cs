using System;
using Microsoft.DirectX.DirectSound;
using TGC.Core.Mathematica;

namespace TGC.Group.Model
{
    class GlobalConcepts
    {

        private static GlobalConcepts instance;
        private string mediaDir;
        private string shaderDir;
        private Device dispositivoDeAudio;
        private float elapsedTime;
        private Microsoft.DirectX.Direct3D.Device screen;

        private GlobalConcepts()
        {
        }

        public static GlobalConcepts GetInstance()
        {
            if (instance == null)
            {
                instance = new GlobalConcepts();
            }

            return instance;
        }

        public void SetScreen(Microsoft.DirectX.Direct3D.Device screen)
        {
            this.screen = screen;
        }

        public Microsoft.DirectX.Direct3D.Device GetScreen()
        {
            return this.screen;
        }

        public void SetMediaDir(string mediaDir)
        {
            this.mediaDir = mediaDir;
        }

        public string GetMediaDir()
        {
            return this.mediaDir;
        }

        public void SetDispositivoDeAudio(Device dispositivo)
        {
            this.dispositivoDeAudio = dispositivo;
        }

        public Device GetDispositivoDeAudio()
        {
            return this.dispositivoDeAudio;
        }

        public float GetElapsedTime()
        {
            return this.elapsedTime;
        }

        public void SetElapsedTime(float value)
        {
            this.elapsedTime = value;
        }

        public TGCMatrix GenerateTransformation(TGCVector3 escala, TGCVector3 rotacion, TGCVector3 traslado)
        {
            TGCMatrix matrixEscalado = TGCMatrix.Scaling(escala);
            TGCMatrix matrixRotacion = TGCMatrix.RotationYawPitchRoll(rotacion.Y, rotacion.X, rotacion.Z);
            TGCMatrix matrixTraslacion = TGCMatrix.Translation(traslado);
            TGCMatrix transformacion = matrixEscalado * matrixRotacion * matrixTraslacion;
            return transformacion;
        }

        public float AngleBetweenVectors(TGCVector3 v1, TGCVector3 v2)
        {
            var dot = TGCVector3.Dot(v1, v2);
            var modulusProduct = v1.Length() * v2.Length();
            return FastMath.Acos(dot / modulusProduct);
        }

        public TGCVector3 GetNormalPlane(TGCPlane plane)
        {
            return new TGCVector3(plane.A, plane.B, plane.C);
        }

        public void SetShaderDir(string shadersDir)
        {
            this.shaderDir = shadersDir;
        }

        public string GetShadersDir()
        {
            return this.shaderDir;
        }
    }
}