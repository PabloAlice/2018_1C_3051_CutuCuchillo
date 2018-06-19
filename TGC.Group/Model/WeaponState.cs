using TGC.Core.Mathematica;
using TGC.Core.Collision;
using TGC.Core.SceneLoader;

namespace TGC.Group.Model
{
    abstract class WeaponState
    {
        public Weapon weapon;

        public WeaponState(Weapon weapon)
        {
            this.weapon = weapon;
        }

        

        virtual public void Render()
        {
            this.weapon.Transform();
            this.weapon.mesh.Render();
            this.weapon.sphere.Render();
        }

        public void Dispose()
        {
            this.weapon.mesh.Dispose();
        }

        abstract public void HandleCollision(Vehicle car);

        virtual public TgcMesh GetCollidable(Vehicle car)
        {
            if (TgcCollisionUtils.testSphereOBB(this.weapon.sphere, car.GetTGCBoundingOrientedBox()))
            {
                return this.weapon.mesh;
            }
            return null;
        }

        abstract public void Move();

        abstract public void Update();

        abstract public void Shoot(Vehicle car);

    }
}
