using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Mathematica;

namespace TGC.Group.Model.Vehiculos.Estados
{
    class Crashing : EstadoVehiculo
    {
        private float distance;
        private TGCVector3 direction;

        public Crashing(Vehicle car, float distance, TGCVector3 direction) : base(car)
        {
            this.distance = distance;
            this.direction = direction;
            car.UnFreeze();
        }

        override public TGCVector3 GetCarDirection()
        {
            return this.auto.GetVectorAdelante();
        }

        override public void Advance()
        {
            return;
        }

        override public void Back()
        {
            return;
        }

        override public void Jump()
        {
            return;
        }

        override public void SpeedUpdate()
        {
            return;
        }

        public override void JumpUpdate()
        {
            base.JumpUpdate();
            this.Move(this.direction);
            this.distance -= this.auto.GetElapsedTime();
            if(distance < 0)
            {
                this.auto.SetEstado(new Stopped(this.auto));
            }
        }

    }
}
