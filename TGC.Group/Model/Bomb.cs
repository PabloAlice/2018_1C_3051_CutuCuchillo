using TGC.Core.Collision;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;

namespace TGC.Group.Model
{
    class Bomb : Weapon, Collidable
    {

        public Bomb(TGCMatrix initialPosition, TGCMatrix scaling, TgcMesh mesh) : base(initialPosition, scaling, mesh)
        {
        }

        public void HandleCollisions(Vehicle car)
        {
            if(TgcCollisionUtils.testSphereOBB(this.sphere, car.GetTGCBoundingOrientedBox()))
            {
                car.AddWeapon(this);
                //Scene.GetInstance().remove(this);
            }
        }
        public void Render()
        {
            this.mesh.Render();
            this.sphere.Render();
        }
        public void Dispose()
        {
            this.sphere.Render();
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
