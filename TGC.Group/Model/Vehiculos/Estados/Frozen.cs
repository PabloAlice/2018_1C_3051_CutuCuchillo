using System;
using TGC.Core.Mathematica;

namespace TGC.Group.Model.Vehiculos.Estados
{
    class Frozen : EstadoVehiculo
    {
        Timer timer;

        public Frozen(Vehicle auto) : base(auto)
        {
            timer = new Timer();
            this.auto.GetDeltaTiempoAvance().resetear();
        }

        public override TGCVector3 GetCarDirection()
        {
            return this.auto.GetVectorAdelante();
        }

        public override void Advance()
        {
            return;
        }

        public override void Back()
        {
            return;
        }

        public override void Jump()
        {
            return;
        }

        public override void Move(TGCVector3 desplazamiento)
        {
            return;
        }

        public override void Right()
        {
            return;
        }

        public override void Left()
        {
            return;
        }

        public override void UpdateWheels()
        {
            base.UpdateWheels();
        }

        public override void FrozenTimeUpdate()
        {
            timer.acumularTiempo(this.auto.GetElapsedTime());
            auto.GetDeltaTiempoAvance().acumularTiempo(auto.GetElapsedTime());
            auto.SetVelocidadActual(Math.Max(0f,auto.GetVelocidadActual() - auto.GetDeltaTiempoAvance().tiempoTranscurrido()));
            auto.Move(auto.GetVectorAdelante() * auto.GetVelocidadActual() * auto.GetElapsedTime());
            if (timer.tiempoTranscurrido() > 10f)
            {
                this.auto.SetEstado(new Stopped(this.auto));
            }
        }
    }
}
