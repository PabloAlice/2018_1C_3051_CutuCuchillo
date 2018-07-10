using TGC.Core.SceneLoader;
using TGC.Core.Mathematica;
using TGC.Core.Particle;

namespace TGC.Group.Model
{
    class Misile : Weapon
    {
        public Misile(TransformationMatrix matrix, TgcMesh mesh) : base(matrix, mesh)
        {
            this.soundManager.AddSound(this.GetPosition(), 50f, 0, "Weapons\\Misil.wav", "Misil", false);
            this.soundManager.AddSound(this.GetPosition(), 50f, 0, "Explosion\\Misil.wav", "Explosion", false);
            this.powerOfDamage = 60f;
        }

        public override void Shoot()
        {
            this.soundManager.GetSound("Misil").play();
        }

        override protected void CreateParticle()
        {
            ParticleEmitter particle = new ParticleEmitter(GlobalConcepts.GetInstance().GetMediaDir() + "Texturas\\Explosion\\Misil.png", 10);
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
            float angle = GlobalConcepts.GetInstance().AngleBetweenVectors(new TGCVector3(0,0,1), direction);
            TGCVector3 result = TGCVector3.Cross(direction, new TGCVector3(0, 0, 1));
            angle = (result.Y > 0) ? -angle : angle;
            return new TGCVector3(FastMath.PI_HALF, angle, 0);
        }
    }
}

