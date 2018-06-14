using TGC.Core.Mathematica;
using TGC.Core.Collision;

namespace TGC.Group.Model
{
    class InExhibition : WeaponState
    {

        public InExhibition(Weapon weapon):base(weapon)
        {
            this.weapon.matrix = this.weapon.ReturnSame(this.weapon.initialTransformation);
            this.weapon.Transform();

        }

        public override void HandleCollision(Vehicle car)
        {
            if (TgcCollisionUtils.testSphereOBB(this.weapon.sphere, car.GetTGCBoundingOrientedBox()))
            {
                car.AddWeapon(this.weapon);
                this.weapon.weaponState = new ReadyToShoot(this.weapon);
            }
        }

        override public void Move()
        {
            return;
        }

        public override void Render()
        {
            this.Update();
            base.Render();
        }

        override public void Update()
        {
            System.Console.WriteLine("Antes de modificar");
            System.Console.WriteLine(this.weapon.initialTransformation.GetTransformation());
            System.Console.WriteLine(this.weapon.matrix.GetTransformation());
            this.weapon.matrix.Rotate(TGCMatrix.RotationYawPitchRoll(0.005f, 0.003f, 0));
            System.Console.WriteLine("Despues de modificar");
            System.Console.WriteLine(this.weapon.initialTransformation.GetTransformation());
            System.Console.WriteLine(this.weapon.matrix.GetTransformation());
        }

        public override void Shoot(Vehicle car)
        {
            return;
        }

    }
}
