using TGC.Core.Mathematica;
using TGC.Core.Collision;
using System.Collections.Generic;

namespace TGC.Group.Model
{
    class Shooted : WeaponState
    {
        private float velocity;
        private Vehicle car;

        public Shooted(Weapon weapon, Vehicle car) : base(weapon)
        {
            this.car = car;
            this.velocity = car.GetMaximunForwardVelocity() * 1.2f;
            this.weapon.direction = car.GetVectorAdelante();
            this.weapon.matrix.SetTranslation(TGCMatrix.Translation(car.GetPosition()));
            this.weapon.matrix.Translate(TGCMatrix.Translation(new TGCVector3(0,0.35f,0)));
            TGCVector3 rotation = this.weapon.GetShootRotation();
            this.weapon.matrix.SetRotation(TGCMatrix.RotationYawPitchRoll(rotation.Y, rotation.X, rotation.Z));
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

        public override void HandleCollision(Vehicle car)
        {
            return;
        }

        public override void Move()
        {
            this.weapon.matrix.Translate(TGCMatrix.Translation(this.weapon.direction * this.velocity * GlobalConcepts.GetInstance().GetElapsedTime()));
        }

        public void MoveBackward()
        {
            this.weapon.matrix.Translate(TGCMatrix.Translation(this.weapon.direction * -this.velocity * GlobalConcepts.GetInstance().GetElapsedTime()));
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
            Collidable collided;
            if (this.IsColliding(out collided))
            {
                System.Console.WriteLine("Cantidad de Armas Antes {0}", this.car.NumberOfWeapons());
                this.car.Remove(this.weapon);
                System.Console.WriteLine("Cantidad de Armas Despues {0}", this.car.NumberOfWeapons());
                this.weapon.Collide(collided);
            }
        }

        private bool IsColliding(out Collidable collided)
        {
            List<Collidable> elements = Scene.GetInstance().GetPosiblesCollidables(this.weapon);
            foreach (Collidable element in elements)
            {
                if (element.IsColliding(this.weapon, out collided))
                {
                    this.Detach(collided);
                    return true;
                }
            }
            collided = null;
            return false;
        }

        private void Detach(Collidable collided)
        {
            Collidable c;
            while(collided.IsColliding(this.weapon, out c))
            {
                this.MoveBackward();
                this.weapon.Transform();
            }
        }

    }
}
