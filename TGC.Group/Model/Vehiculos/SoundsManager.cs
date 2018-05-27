using Microsoft.DirectX.DirectSound;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Mathematica;
using TGC.Group.MyAbstractions;

namespace TGC.Group.Model.Vehiculos
{
    class SoundsManager
    {
        Tgc3dSound velSound;
        private int initialFreq;
        public SoundsManager(String velSoundWAV, TGCVector3 position)
        {
            var bufferDescription = new BufferDescription();
            bufferDescription.ControlFrequency = true;
            this.velSound = new Tgc3dSound(velSoundWAV, position, GlobalConcepts.GetInstance().GetDispositivoDeAudio(), bufferDescription);
            this.initialFreq = this.velSound.SoundBuffer.Frequency;
            this.velSound.play(true);
        }

        private void SetFrequency(int freq)
        {
            this.velSound.SoundBuffer.Frequency = freq;
        }

        private int GetFrequency()
        {
            return this.velSound.SoundBuffer.Frequency;
        }

        public void Update (float velocidad)
        {
            var modifier = (int)velocidad;
            int freq = this.initialFreq + modifier * 1000;
            this.SetFrequency(freq);
        }
    }
}
