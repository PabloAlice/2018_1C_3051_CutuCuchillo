using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Collision;
using TGC.Core.SceneLoader;

namespace TGC.Group.Model
{
    class ReadyToShoot : WeaponState
    {
        public ReadyToShoot(Weapon weapon) : base(weapon)
        {

        }

        public override void Move()
        {
            return;
        }

        public override void Update()
        {
            return;
        }

        public override void Shoot(Vehicle car)
        {
            this.weapon.weaponState = new Shooted(this.weapon, car);
        }

        public override void Render()
        {
            return;
        }

        override public TgcMesh GetCollidable(Vehicle car)
        {
            return null;
        }
    }
}
