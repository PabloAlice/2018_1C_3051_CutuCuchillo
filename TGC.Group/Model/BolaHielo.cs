using TGC.Core.Mathematica;
using TGC.Core.Particle;
using TGC.Core.SceneLoader;

namespace TGC.Group.Model
{
    class BolaHielo : Weapon
    {
        public BolaHielo(TransformationMatrix matrix, TgcMesh mesh) : base(matrix, mesh)
        {
            this.soundManager.AddSound(this.GetPosition(), 10f, 0, "Weapons\\BolaHielo.wav", "BolaHielo", false);
            this.soundManager.AddSound(this.GetPosition(), 10f, 0, "Explosion\\BolaHielo.wav", "Explosion", false);
            this.powerOfDamage = 15f;
        }

        public override void Shoot()
        {
            this.soundManager.GetSound("BolaHielo").play();
        }

        override protected void CreateParticle()
        {
            this.particle = new ParticleEmitter(GlobalConcepts.GetInstance().GetMediaDir() + "Texturas\\Explosion\\Bomba.png", 10);
            this.particle.Position = this.GetPosition();
            this.particle.MinSizeParticle = 1f;
            this.particle.MaxSizeParticle = 2f;
            this.particle.ParticleTimeToLive = 0.5f;
            this.particle.CreationFrecuency = 0.1f;
            this.particle.Dispersion = 30;
            this.particle.Playing = false;
            this.particle.Speed = new TGCVector3(1, 1, 1);
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
