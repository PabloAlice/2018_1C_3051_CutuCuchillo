using System.Text;
using System.Threading.Tasks;
using TGC.Core.Sound;
using Microsoft.DirectX.DirectSound;
using TGC.Core.Mathematica;

namespace TGC.Group.Model
{
    class GlobalConcepts
    {

        private static GlobalConcepts instance;
        private string mediaDir;
        private Device dispositivoDeAudio;
        private float elapsedTime;

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
    }
}