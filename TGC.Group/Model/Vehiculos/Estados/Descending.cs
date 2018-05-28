using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Mathematica;
using TGC.Core.Sound;

namespace TGC.Group.Model.Vehiculos.Estados
{
    class Descending : EstadoVehiculo
    {
        private float initialSpeed;

        public Descending(Vehicle auto, float initialSpeed) : base(auto)
        {
            this.initialSpeed = initialSpeed;
            this.audio = new Tgc3dSound(GlobalConcepts.GetInstance().GetMediaDir() + "Sound\\Caida.wav", this.auto.GetPosicion(), GlobalConcepts.GetInstance().GetDispositivoDeAudio());
            this.audio.MinDistance = 50f;
        }

        public override TGCVector3 GetCarDirection()
        {
            return new TGCVector3(0,-1,0);
        }

        public override void Advance()
        {
            if (auto.GetVelocidadActual() < 0)
            {
                auto.SetVelocidadActual(auto.GetVelocidadActual() + auto.GetConstanteFrenado() * 2);
                if (auto.GetVelocidadActual() > 0)
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
            if (auto.GetVelocidadActual() > 0)
            {
                auto.SetVelocidadActual(auto.GetVelocidadActual() - auto.GetConstanteFrenado() * 2);
                if (auto.GetVelocidadActual() < 0)
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
            desplazamientoEnY = (this.auto.GetPosicion().Y + desplazamientoEnY < 0) ? -this.auto.GetPosicion().Y : desplazamientoEnY;
            TGCVector3 nuevoDesplazamiento = new TGCVector3(0, desplazamientoEnY, 0);
            this.Move(nuevoDesplazamiento + auto.vectorAdelanteSalto * this.initialSpeed * auto.GetElapsedTime());
            if(this.auto.GetPosicion().Y == 0)
            {
                auto.GetDeltaTiempoSalto().resetear();
                auto.SetVelocidadActualDeSalto(auto.GetVelocidadMaximaDeSalto());
                this.audio.play();
                if (auto.GetVelocidadActual() > 0)
                {
                    this.cambiarEstado(new Forward(this.auto));
                }
                else if(auto.GetVelocidadActual() < 0)
                {
                    this.cambiarEstado(new Backward(this.auto));
                }
                else
                {
                    this.cambiarEstado(new Stopped(this.auto));
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
