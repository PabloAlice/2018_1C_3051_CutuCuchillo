using TGC.Core.Mathematica;
using TGC.Core.Particle;
using TGC.Core.SceneLoader;

namespace TGC.Group.Model
{
    class BolaHielo : Weapon
    {
        public BolaHielo(TransformationMatrix matrix, TgcMesh mesh) : base(matrix, mesh)
        {
            this.soundManager.AddSound(this.GetPosition(), 50, 0, "Weapons\\BolaHielo.wav", "BolaHielo", false);
            this.soundManager.AddSound(this.GetPosition(), 50, 0, "Explosion\\BolaHielo.wav", "Explosion", false);
            this.powerOfDamage = 15f;
        }

        public override void Shoot()
        {
            this.soundManager.GetSound("BolaHielo").play();
        }

        override protected void CreateParticle()
        {
            ParticleEmitter particle = new ParticleEmitter(GlobalConcepts.GetInstance().GetMediaDir() + "Texturas\\Explosion\\BolaHielo.png", 10);
            particle.Position = this.GetPosition();
            particle.MinSizeParticle = 1f;
            particle.MaxSizeParticle = 2f;
            particle.ParticleTimeToLive = 0.5f;
            particle.CreationFrecuency = 0.1f;
            particle.Dispersion = 30;
            particle.Playing = false;
            particle.Speed = new TGCVector3(1, 1, 1);
            this.particle = new ParticleTimer(particle, 2f);
        }

        public override void Collide(Collidable element)
        {
            element.HandleCollision(this);
            this.weaponState = new InExhibition(this);
            return;
        }

        override public void Move()
        {
            //proximamente
        }

        public override TGCVector3 GetShootRotation()
        {
            return new TGCVector3(0,0,0);
        }

        public override void IAmTheCar(Vehicle car)
        {
            base.IAmTheCar(car);
            car.ChangeToFreeze();
        }
    }
}
