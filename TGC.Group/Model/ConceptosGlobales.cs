using System.Text;
using System.Threading.Tasks;
using TGC.Core.Sound;
using Microsoft.DirectX.DirectSound;
using TGC.Core.Mathematica;

namespace TGC.Group.Model
{
    class ConceptosGlobales
    {

        private static ConceptosGlobales instance;
        private string mediaDir;
        private Device dispositivoDeAudio;

        private ConceptosGlobales()
        {
        }

        public static ConceptosGlobales getInstance()
        {
            if (instance == null)
            {
                instance = new ConceptosGlobales();
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

        public TGCMatrix GenerateTransformation(TGCVector3 escala, TGCVector3 rotacion, TGCVector3 traslado)
        {
            TGCMatrix matrixEscalado = TGCMatrix.Scaling(escala);
            TGCMatrix matrixRotacionX = TGCMatrix.RotationX(rotacion.X);
            TGCMatrix matrixRotacionY = TGCMatrix.RotationY(rotacion.Y);
            TGCMatrix matrixRotacionZ = TGCMatrix.RotationZ(rotacion.Z);
            TGCMatrix matrixRotacion = matrixRotacionX * matrixRotacionY * matrixRotacionZ;
            TGCMatrix matrixTraslacion = TGCMatrix.Translation(traslado);
            TGCMatrix transformacion = matrixEscalado * matrixRotacion * matrixTraslacion;
            return transformacion;
        }
    }
}