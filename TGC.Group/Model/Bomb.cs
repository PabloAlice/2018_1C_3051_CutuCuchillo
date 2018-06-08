using TGC.Core.Collision;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;

namespace TGC.Group.Model
{
    class Bomb : Weapon, Collidable
    {

        public Bomb(TransformationMatrix matrix, TgcMesh mesh) : base(matrix, mesh)
        {
        }

        public void HandleCollisions(Vehicle car)
        {
            if(TgcCollisionUtils.testSphereOBB(this.sphere, car.GetTGCBoundingOrientedBox()))
            {
                car.AddWeapon(this);
                this.weaponState = new InExhibition(this);
                //Scene.GetInstance().remove(this);
            }
        }

        public TgcMesh GetCollidable(Vehicle car)
        {
            if (TgcCollisionUtils.testSphereOBB(this.sphere, car.GetTGCBoundingOrientedBox()))
            {
                return this.mesh;
            }
            return null;
        }

        override public void Move()
        {
            //proximamente
        }
    }
}
