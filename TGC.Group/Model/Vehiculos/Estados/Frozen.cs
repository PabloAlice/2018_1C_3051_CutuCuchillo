using TGC.Core.Mathematica;

namespace TGC.Group.Model.Vehiculos.Estados
{
    class Frozen : EstadoVehiculo
    {
        Timer timer;

        public Frozen(Vehicle auto) : base(auto)
        {
            timer = new Timer();
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
            if(timer.tiempoTranscurrido() > 10f)
            {
                this.auto.SetEstado(new Stopped(this.auto));
            }
        }
    }
}
