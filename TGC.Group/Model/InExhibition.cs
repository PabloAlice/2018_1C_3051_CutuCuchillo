using TGC.Core.Mathematica;
using TGC.Core.Collision;
using Microsoft.DirectX.Direct3D;
using TGC.Core.Shaders;

namespace TGC.Group.Model
{
    class InExhibition : WeaponState
    {

        public InExhibition(Weapon weapon):base(weapon)
        {
            this.weapon.matrix = this.weapon.ReturnSame(this.weapon.initialTransformation);
            this.weapon.Transform();
            weapon.ExhibitionEffect();
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
            this.weapon.matrix.Rotate(TGCMatrix.RotationYawPitchRoll(0.005f, 0.003f, 0));
        }

        public override void Shoot(Vehicle car)
        {
            return;
        }

    }
}
