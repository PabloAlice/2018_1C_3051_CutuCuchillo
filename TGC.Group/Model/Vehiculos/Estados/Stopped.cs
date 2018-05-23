using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Group.Model.Vehiculos.Estados;
using TGC.Core.Mathematica;
using TGC.Core.Sound;

namespace TGC.Group.Model.Vehiculos.Estados
{
    class Stopped : EstadoVehiculo
    {

        public Stopped(Vehiculo auto) : base(auto)
        {
            this.audio = new Tgc3dSound(ConceptosGlobales.GetInstance().GetMediaDir() + "Sound\\Motor.wav", this.auto.GetPosicion(), ConceptosGlobales.GetInstance().GetDispositivoDeAudio());
            this.audio.MinDistance = 50f;
            // this.audio.play(true);
        }

        override public void Advance()
        {
            base.Advance();
            this.Move(auto.GetVectorAdelante() * auto.GetVelocidadActual() * auto.GetElapsedTime());
            this.cambiarEstado(new Forward(this.auto));
        }

        override public void Back()
        {
            base.Back();
            this.Move(auto.GetVectorAdelante() * auto.GetVelocidadActual() * auto.GetElapsedTime());
            this.cambiarEstado(new Backward(this.auto));
        }

        override public void Left()
        {
            float rotacionReal = -1f * auto.GetElapsedTime();
            auto.RotarDelanteras(rotacionReal);
        }

        override public void Right()
        {
            float rotacionReal = 1f * auto.GetElapsedTime();
            auto.RotarDelanteras(rotacionReal);
        }

        public override void UpdateWheels()
        {
            return;
        }
    }
}
