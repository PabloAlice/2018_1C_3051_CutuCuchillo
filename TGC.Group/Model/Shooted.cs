using TGC.Core.Mathematica;
using TGC.Core.Collision;
using System.Collections.Generic;

namespace TGC.Group.Model
{
    class Shooted : WeaponState
    {
        private TGCVector3 direction;
        private Vehicle car;

        public Shooted(Weapon weapon, Vehicle car) : base(weapon)
        {
            this.direction = car.GetVectorAdelante();
            this.car = car;
            //hay que ponerle el nuevo rotado y trasladado del arma 
            this.weapon.matrix.SetTranslation(TGCMatrix.Translation(car.GetPosition()));
            this.weapon.matrix.Translate(TGCMatrix.Translation(new TGCVector3(0,0.35f,0)));
            TGCVector3 rotation = this.weapon.GetShootRotation();
            this.weapon.matrix.SetRotation(TGCMatrix.RotationYawPitchRoll(rotation.X, rotation.Y, rotation.Z));
            this.MoveAwayFromTheCar(car);
        }

        private void MoveAwayFromTheCar(Vehicle car)
        {
            while(TgcCollisionUtils.testSphereOBB(this.weapon.sphere, car.GetTGCBoundingOrientedBox()))
            {
                this.Move();
                this.weapon.Transform();
            }
        }

        public override void Move()
        {
            this.weapon.matrix.Translate(TGCMatrix.Translation(this.direction * 0.1f));
        }

        public override void Update()
        {
            this.Move();
            this.weapon.Transform();
            this.CheckCollision();
        }

        public override void Render()
        {
            this.Update();
            base.Render();
        }

        public override void Shoot(Vehicle car)
        {
            return;
        }

        private void CheckCollision()
        {
            if (this.IsColliding())
            {
                this.car.Remove(this.weapon);
                this.weapon.weaponState = new InExhibition(this.weapon);
            }
        }

        private bool IsColliding()
        {
            List<Collidable> elements = Scene.GetInstance().GetPosiblesCollidables(this.weapon);
            foreach (Collidable element in elements)
            {
                if (element.IsColliding(this.weapon))
                {
                    return true;
                }
            }
            return false;
        }

    }
}
