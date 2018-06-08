using TGC.Core.Mathematica;
using TGC.Core.Collision;
using System.Collections.Generic;

namespace TGC.Group.Model
{
    class Shooted : WeaponState
    {
        private TGCVector3 direction;

        public Shooted(Weapon weapon, Vehicle car) : base(weapon)
        {
            this.direction = car.GetVectorAdelante();
            Scene.GetInstance().Remove(this.weapon);
            //hay que ponerle el nuevo rotado y trasladado del arma 
            this.weapon.matrix.SetTranslation(TGCMatrix.Translation(this.weapon.GetPosition()));
            TGCVector3 rotation = this.weapon.GetShootRotation();
            this.weapon.matrix.SetRotation(TGCMatrix.RotationYawPitchRoll(rotation.X, rotation.Y, rotation.Z));
        }

        public override void Move()
        {
            return;
        }

        public override void Update()
        {
            this.CheckCollision();
        }

        public override void Shoot(Vehicle car)
        {
            return;
        }

        private void CheckCollision()
        {
            if (this.IsColliding())
            {
                this.weapon.weaponState = new InExhibition(this.weapon);
                //TODO agregarlo NUEVAMENTE A LA LISTA DEL ESCENARIO
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
