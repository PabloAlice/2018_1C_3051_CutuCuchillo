using Microsoft.DirectX.DirectSound;
using System.Collections.Generic;
using TGC.Core.Mathematica;
using TGC.Group.MyAbstractions;
using System;

namespace TGC.Group.Model.Vehiculos
{
    class SoundsManager
    {
        List<Sound> sounds = new List<Sound>();
        private int initialFreq;
        private Tgc3dSound soundWithFrecuency;
        public SoundsManager()
        {
        }

        public void AddSound(TGCVector3 position, float minDistance, int volume, string sound, string apodo, bool isSoundWithFrecuency)
        {
            if(this.sounds.Exists(s => sound.Equals(apodo)))
            {
                throw new Exception("Ya existe un sonido con ese nombre");
            }
            BufferDescription bufferDescription = new BufferDescription();
            bufferDescription.ControlVolume = true;
            if (isSoundWithFrecuency)
            {
                bufferDescription.ControlFrequency = true;
            }
            Tgc3dSound newSound = new Tgc3dSound(GlobalConcepts.GetInstance().GetMediaDir() + "Sound\\" + sound, position, GlobalConcepts.GetInstance().GetDispositivoDeAudio(), bufferDescription);
            newSound.SoundBuffer.Volume = volume;
            newSound.MinDistance = minDistance;
            if (isSoundWithFrecuency)
            {
                this.initialFreq = newSound.SoundBuffer.Frequency;
                this.soundWithFrecuency = newSound;
            }
            this.sounds.Add(new Sound(newSound, apodo));
        }

        public void UpdatePositions(TGCVector3 position)
        {
            this.sounds.ForEach(s => s.sound.Position = position);
        }

        public void Dispose()
        {
            this.sounds.ForEach(s => s.sound.dispose());
        }

        public Tgc3dSound GetSound(string name)
        {
            return this.sounds.Find(sound => sound.Equals(name)).sound;
        }

        public void RemoveSound(string name)
        {
            this.sounds.RemoveAll(s => s.Equals(name));
        }

        private void SetFrequency(int freq)
        {
            this.soundWithFrecuency.SoundBuffer.Frequency = freq > this.initialFreq ? freq : this.initialFreq;
        }

        private int GetFrequency()
        {
            return this.soundWithFrecuency.SoundBuffer.Frequency;
        }
        
        public void Update (float velocidad)
        {
            var modifier = (int)velocidad;
            int freq = this.initialFreq + modifier * 1000;
            this.SetFrequency(freq);
        }

        public class Sound
        {
            public Tgc3dSound sound;
            public string name;

            public Sound(Tgc3dSound sound, string name)
            {
                this.sound = sound;
                this.name = name;
            }

            public bool Equals(string name)
            {
                return this.name == name;
            }
        }
    }
}
