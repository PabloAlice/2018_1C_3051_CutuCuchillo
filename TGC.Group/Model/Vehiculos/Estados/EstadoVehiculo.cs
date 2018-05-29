using TGC.Core.Mathematica;
using TGC.Core.Sound;

namespace TGC.Group.Model.Vehiculos.Estados
{
    abstract class EstadoVehiculo
    {
        protected Vehicle auto;

        abstract public TGCVector3 GetCarDirection();

        public EstadoVehiculo(Vehicle auto)
        {
            this.auto = auto;
        }

        virtual public void Advance()
        {
            auto.GetDeltaTiempoAvance().acumularTiempo(auto.GetElapsedTime());
            auto.SetVelocidadActual(auto.VelocidadFisica());
            return;
        }

        virtual public void Back()
        {
            auto.GetDeltaTiempoAvance().acumularTiempo(auto.GetElapsedTime());
            auto.SetVelocidadActual(auto.VelocidadFisicaRetroceso());
            return;
        }

        virtual public void Jump()
        {
            this.auto.GetDeltaTiempoSalto().acumularTiempo(auto.GetElapsedTime());
            this.auto.vectorAdelanteSalto = auto.vectorAdelante;
            this.auto.SetEstado(new Jumping(this.auto));
            return;
        }

        virtual public void SpeedUpdate()
        {
            return;
        }

        virtual public void JumpUpdate()
        {
            return;
        }

        protected void Move(TGCVector3 desplazamiento)
        {
            this.auto.Move(desplazamiento);
        }

        protected float VelocidadFisicaDeSalto()
        {
            return auto.GetVelocidadActualDeSalto() + (-auto.GetAceleracionGravedad()) * auto.GetDeltaTiempoSalto().tiempoTranscurrido();
        }

        virtual public void Right()
        {
            float rotacionReal = auto.GetVelocidadDeRotacion() * auto.GetElapsedTime();
            rotacionReal = (auto.GetVelocidadActual() > 0) ? rotacionReal : -rotacionReal;
            this.auto.Girar(rotacionReal);

        }

        //lo mismo que arriba
        virtual public void Left()
        {
            float rotacionReal = auto.GetVelocidadDeRotacion() * auto.GetElapsedTime();
            rotacionReal = (auto.GetVelocidadActual() < 0) ? rotacionReal : -rotacionReal;
            this.auto.Girar(rotacionReal);
        }

        virtual public void UpdateWheels()
        {
            var rotacionReal = this.auto.GetVelocidadDeRotacion() * this.auto.GetElapsedTime();
            this.auto.UpdateFrontWheels(rotacionReal);
        }
    }
}
