using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;

namespace TGC.Group.Model
{
    class BolaHielo : Weapon
    {
        public BolaHielo(TransformationMatrix matrix, TgcMesh mesh) : base(matrix, mesh)
        {
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
