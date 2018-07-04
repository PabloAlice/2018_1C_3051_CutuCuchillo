using TGC.Core.Collision;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Core.Particle;

namespace TGC.Group.Model
{
    class Bomb : Weapon
    {
        private int numberOfCollisions = 3;

        public Bomb(TransformationMatrix matrix, TgcMesh mesh) : base(matrix, mesh)
        {
            this.soundManager.AddSound(this.GetPosition(), 10f, 0, "Weapons\\Bomba.wav", "Bomba", false);
            this.soundManager.AddSound(this.GetPosition(), 10f, 0, "Explosion\\Bomba.wav", "Explosion", false);
        }

        override protected void CreateParticle()
        {
            this.particle = new ParticleEmitter(GlobalConcepts.GetInstance().GetMediaDir() + "Texturas\\Chispas\\Chispas.png", 10);
            this.particle.Position = this.GetPosition();
            this.particle.MinSizeParticle = 1f;
            this.particle.MaxSizeParticle = 2f;
            this.particle.ParticleTimeToLive = 0.5f;
            this.particle.CreationFrecuency = 0.1f;
            this.particle.Dispersion = 30;
            this.particle.Playing = false;
            this.particle.Speed = new TGCVector3(1, 1, 1);
        }

        public override void Shoot()
        {
            this.soundManager.GetSound("Bomba").play(true);
        }

        override public void Move()
        {
            //proximamente
        }

        public override TGCVector3 GetShootRotation()
        {
            return new TGCVector3(0,0,0);
        }

        public override void Collide(Collidable element)
        {
            numberOfCollisions--;
            element.HandleCollision(this);
            if (this.numberOfCollisions == 0)
            {
                this.weaponState = new InExhibition(this);
                this.numberOfCollisions = 3;
                return;
            }
            this.Bounce(element);
            this.soundManager.GetSound("Bomba").stop();
        }

        public override void IAmTheCar()
        {
            numberOfCollisions = 0;
        }

        private void Bounce(Collidable element)
        {
            TGCVector3 vector = this.direction;
            this.direction = new TGCVector3(-vector.X, vector.Y, -vector.Z);
        }
    }
}
