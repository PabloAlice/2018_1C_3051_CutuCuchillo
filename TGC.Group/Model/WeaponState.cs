using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public void HandleCollision(Vehicle car)
        {
            if (TgcCollisionUtils.testSphereOBB(this.weapon.sphere, car.GetTGCBoundingOrientedBox()))
            {
                car.AddWeapon(this.weapon);
                this.weapon.weaponState = new ReadyToShoot(this.weapon);
                //Scene.GetInstance().remove(this);
            }
        }

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
