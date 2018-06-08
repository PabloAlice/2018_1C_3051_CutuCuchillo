using TGC.Core.Mathematica;
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
            car.Remove(this.weapon);
        }

        public override void Render()
        {
            return;
        }

        override public TgcMesh GetCollidable(Vehicle car)
        {
            return null;
        }

        override public TGCVector3 GetDirection()
        {
            throw new System.Exception("Nunca le puedo pedir la direction al estado exhibido");
        }

        override public void SetDirection(TGCVector3 vector)
        {
            throw new System.Exception("Este estado no tiene direction");
        }
    }
}
