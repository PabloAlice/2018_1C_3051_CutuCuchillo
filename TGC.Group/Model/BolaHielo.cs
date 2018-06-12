using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;

namespace TGC.Group.Model
{
    class BolaHielo : Weapon
    {
        public BolaHielo(TransformationMatrix matrix, TgcMesh mesh) : base(matrix, mesh)
        {
            this.soundManager.AddSound(this.GetPosition(), 10f, 0, "Weapons\\BolaHielo.wav", "BolaHielo", false);
        }

        public override void Shoot()
        {
            this.soundManager.GetSound("BolaHielo").play();
        }

        public override void Collide(Collidable collided)
        {
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
    }
}
