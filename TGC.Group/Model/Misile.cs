using TGC.Core.SceneLoader;
using TGC.Core.Mathematica;
using TGC.Core.Particle;

namespace TGC.Group.Model
{
    class Misile : Weapon
    {
        public Misile(TransformationMatrix matrix, TgcMesh mesh) : base(matrix, mesh)
        {
            this.soundManager.AddSound(this.GetPosition(), 10f, 0, "Weapons\\Misil.wav", "Misil", false);
            this.soundManager.AddSound(this.GetPosition(), 10f, 0, "Explosion\\Misil.wav", "Explosion", false);
        }

        public override void Shoot()
        {
            this.soundManager.GetSound("Misil").play();
        }

        override protected void CreateParticle()
        {
            this.particle = new ParticleEmitter(GlobalConcepts.GetInstance().GetMediaDir() + "Texturas\\Explosion\\Misil.png", 10);
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
            float angle = GlobalConcepts.GetInstance().AngleBetweenVectors(new TGCVector3(0,0,1), this.direction);
            return new TGCVector3(FastMath.PI_HALF, angle, 0);
        }
    }
}

