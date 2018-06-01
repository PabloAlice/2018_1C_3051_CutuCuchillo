using TGC.Core.Mathematica;
using TGC.Core.Sound;

namespace TGC.Group.Model.Vehiculos.Estados
{
    class Jumping : EstadoVehiculo
    {
        private float initialSpeed;

        public Jumping(Vehicle auto) : base(auto)
        {
            this.initialSpeed = auto.GetVelocidadActual();
            this.auto.SoundsManager.Jump();
        }

        public override TGCVector3 GetCarDirection()
        {
            return new TGCVector3(0, 1, 0);
        }

        public override void Advance()
        {
            if(auto.GetVelocidadActual() < 0)
            {
                auto.SetVelocidadActual(auto.GetVelocidadActual() + auto.GetConstanteFrenado() * 2);
                if(auto.GetVelocidadActual() > 0)
                {
                    auto.SetVelocidadActual(0);
                    auto.GetDeltaTiempoAvance().resetear();
                }
                return;
            }
            base.Advance();

        }

        public override void Back()
        {
            if(auto.GetVelocidadActual() > 0)
            {
                auto.SetVelocidadActual(auto.GetVelocidadActual() - auto.GetConstanteFrenado() *2);
                if(auto.GetVelocidadActual() < 0)
                {
                    auto.SetVelocidadActual(0);
                    auto.GetDeltaTiempoAvance().resetear();
                }
                return;
            }
            base.Back();
        }

        public override void Jump()
        {
            return;
        }

        public override void JumpUpdate()
        {
            auto.SetVelocidadActualDeSalto(this.VelocidadFisicaDeSalto());
            float desplazamientoEnY = auto.GetVelocidadActualDeSalto() * auto.GetElapsedTime();
            TGCVector3 nuevoDesplazamiento = new TGCVector3(0, desplazamientoEnY, 0);
            TGCVector3 cuenta = nuevoDesplazamiento + auto.VectorAdelanteSalto * this.initialSpeed * auto.GetElapsedTime();
            this.Move(nuevoDesplazamiento + auto.VectorAdelanteSalto * this.initialSpeed * auto.GetElapsedTime());
            if(this.IsCollidingWithFloor())
            {
                this.Move(-cuenta);
                auto.GetDeltaTiempoSalto().resetear();
                auto.SetVelocidadActualDeSalto(auto.GetVelocidadMaximaDeSalto());
                this.auto.SoundsManager.Drop();
                if (auto.GetVelocidadActual() > 0)
                {
                    this.auto.SetEstado(new Forward(this.auto));
                }
                else if (auto.GetVelocidadActual() < 0)
                {
                    this.auto.SetEstado(new Backward(this.auto));
                }
                else
                {
                    this.auto.SetEstado(new Stopped(this.auto));
                }
            }
        }

        public override void Left()
        {
            this.auto.RotarDelanteras(-this.auto.GetVelocidadDeRotacion() * this.auto.GetElapsedTime());
            float rotacionReal = auto.GetVelocidadDeRotacion() * auto.GetElapsedTime();
            rotacionReal = (auto.GetVelocidadActual() < 0) ? rotacionReal : -rotacionReal;
            this.auto.Girar(rotacionReal);
        }

        public override void Right()
        {
            this.auto.RotarDelanteras(this.auto.GetVelocidadDeRotacion() * this.auto.GetElapsedTime());
            float rotacionReal = auto.GetVelocidadDeRotacion() * auto.GetElapsedTime();
            rotacionReal = (auto.GetVelocidadActual() > 0) ? rotacionReal : -rotacionReal;
            this.auto.Girar(rotacionReal);
        }

        public override void UpdateWheels()
        {
            return;
        }
    }
}
