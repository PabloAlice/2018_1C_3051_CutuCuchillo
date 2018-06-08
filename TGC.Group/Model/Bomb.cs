using TGC.Core.Collision;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Core.BoundingVolumes;

namespace TGC.Group.Model
{
    class Bomb : Weapon
    {
        private int numberOfCollisions = 3;

        public Bomb(TransformationMatrix matrix, TgcMesh mesh) : base(matrix, mesh)
        {
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
            if (this.numberOfCollisions == 0)
            {
                this.weaponState = new InExhibition(this);
                this.numberOfCollisions = 3;
                return;
            }
            this.Bounce(element);
        }

        private void Bounce(Collidable element)
        {
            TGCVector3 vector = this.direction;
            this.direction = new TGCVector3(-vector.X, vector.Y, -vector.Z);
        }
    }
}
