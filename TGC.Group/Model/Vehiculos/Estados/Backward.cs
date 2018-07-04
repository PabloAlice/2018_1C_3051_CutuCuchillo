using TGC.Core.Sound;
using TGC.Core.Mathematica;

namespace TGC.Group.Model.Vehiculos.Estados
{
    class Backward : EstadoVehiculo
    {

        public Backward(Vehicle auto) : base(auto)
        {
            auto.reverseLights.ActivateLight();
        }

        public override TGCVector3 GetCarDirection()
        {
            return -auto.GetVectorAdelante();
        }

        public override void Back()
        {
            base.Back();
            Move(auto.GetVectorAdelante() * auto.GetVelocidadActual() * auto.GetElapsedTime());
        }

        public override void Advance()
        {
            brake(auto.GetConstanteFrenado());
        }

        public override void SpeedUpdate()
        {
            brake(auto.GetConstanteRozamiento());
        }

        private void brake(float constante)
        {
            auto.GetDeltaTiempoAvance().resetear();
            auto.SetVelocidadActual(auto.GetVelocidadActual() + constante);
            if (auto.GetVelocidadActual() > 0)
            {
                auto.SetVelocidadActual(0);
                auto.GetDeltaTiempoAvance().resetear();
                auto.reverseLights.DesactivateLight();
                this.auto.SetEstado(new Stopped(this.auto));
                return;
            }
            this.Move(auto.GetVectorAdelante() * auto.GetVelocidadActual() * auto.GetElapsedTime());
        }
    }
}
