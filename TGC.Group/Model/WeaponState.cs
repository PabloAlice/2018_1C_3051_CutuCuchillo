using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGC.Group.Model
{
    abstract class WeaponState
    {
        public Weapon weapon;

        public WeaponState(Weapon weapon)
        {
            this.weapon = weapon;
        }

        public void Render()
        {
            this.weapon.Transform();
            this.weapon.mesh.Render();
            this.weapon.sphere.Render();
        }

        public void Dispose()
        {
            this.weapon.mesh.Dispose();
        }

        abstract public void Move();

        abstract public void Update();
    }
}
