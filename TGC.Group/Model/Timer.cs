
namespace TGC.Group.Model
{
    class Timer
    {
        float elapsedTime;
        public Timer()
        {
            this.elapsedTime = 0f;
        }

        public void acumularTiempo(float elapsedTime)
        {
            this.elapsedTime += elapsedTime;
        }

        public float tiempoTranscurrido()
        {
            return this.elapsedTime;
        }

        public void resetear()
        {
            this.elapsedTime = 0;
        }

    }
}
