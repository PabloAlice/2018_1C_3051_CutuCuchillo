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
            this.velocity = 90f;
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
            this.weapon.matrix.Translate(TGCMatrix.Translation(this.weapon.direction * this.velocity * GlobalConcepts.GetInstance().GetElapsedTime() * 2));
            this.weapon.Transform();
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
            Collidable c = null;
            foreach (Collidable element in Scene.GetInstance().GetPosiblesCollidables(weapon))
            {
                //ésto está hecho a lo negro por que sino no anda, vaya a saber uno por qué
                if (element.IsColliding(this.weapon))
                {
                    c = element;
                    break;
                }
            }
            if (Scene.GetInstance().auto.IsColliding(this.weapon))
            {
                c = Scene.GetInstance().auto;
            }
            else if (Scene.GetInstance().AI.IsColliding(this.weapon))
            {
                c = Scene.GetInstance().AI;
            }
            if(c != null)
            {
                this.Detach(c);
                this.car.Remove(this.weapon);
                this.Explode();
                this.weapon.Collide(c);
            }
        }

        private void Detach(Collidable collided)
        {
            while (collided.IsColliding(this.weapon))
            {
                this.MoveBackward();
                this.weapon.Transform();
            }
        }

        protected void Explode()
        {
            this.weapon.particle.Playing = true;
            this.weapon.particle.Position = this.weapon.GetPosition();
            this.weapon.soundManager.GetSound("Explosion").play();
        }

    }
}
