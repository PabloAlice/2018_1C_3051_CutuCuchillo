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
            this.audio = new Tgc3dSound(GlobalConcepts.GetInstance().GetMediaDir() + "Sound\\Salto.wav", this.auto.GetPosicion(), GlobalConcepts.GetInstance().GetDispositivoDeAudio());
            this.audio.MinDistance = 50f;
           // this.audio.play();
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
            if(auto.GetVelocidadActualDeSalto() < 0)
            {
                this.cambiarEstado(new Descending(this.auto, this.initialSpeed));
                return;
            }
            float desplazamientoEnY = auto.GetVelocidadActualDeSalto() * auto.GetElapsedTime();
            TGCVector3 nuevoDesplazamiento = new TGCVector3(0, desplazamientoEnY, 0);
            //auto.mesh.Move(nuevoDesplazamiento);
            //auto.mesh.Move(auto.getVectorAdelante() * this.initialSpeed * auto.getElapsedTime());
            this.Move(nuevoDesplazamiento + auto.vectorAdelanteSalto * this.initialSpeed * auto.GetElapsedTime());
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
