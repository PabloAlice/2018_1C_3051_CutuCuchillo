using TGC.Core.Mathematica;
using TGC.Core.Particle;

namespace TGC.Group.Model
{
    class ParticleTimer
    {
        public ParticleEmitter particle;
        private float timer = 0f;
        private float end = 0f;

        public ParticleTimer(ParticleEmitter particle, float endTime)
        {
            this.particle = particle;
            this.particle.Playing = false;
            this.end = endTime;
        }
        
        public void Play(TGCVector3 position)
        {
            this.particle.Playing = true;
            this.particle.Position = position;
        }

        public void Render(float elapsedTime)
        {
            if (!this.particle.Playing) return;
            timer += elapsedTime;
            this.particle.render(elapsedTime);
            if (timer >= end)
            {
                this.Stop();
                return;
            }
            
        }

        public void Reset()
        {
            timer = 0f;
        }

        private void Stop()
        {
            this.Reset();
            this.particle.Playing = false;
        }
    }
}
