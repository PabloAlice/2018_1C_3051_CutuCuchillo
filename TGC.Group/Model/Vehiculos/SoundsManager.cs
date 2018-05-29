using Microsoft.DirectX.DirectSound;
using System;
using TGC.Core.Mathematica;
using TGC.Group.MyAbstractions;

namespace TGC.Group.Model.Vehiculos
{
    class SoundsManager
    {
        Tgc3dSound jump;
        Tgc3dSound drop;
        Tgc3dSound velSound;
        Tgc3dSound crash;
        Tgc3dSound backgroundMusic;
        Tgc3dSound horn;
        Tgc3dSound alarm;
        Tgc3dSound defaultWeapon;
        private int initialFreq;
        public SoundsManager(TGCVector3 position)
        {
            BufferDescription bufferDescription = new BufferDescription();
            bufferDescription.ControlFrequency = true;
            bufferDescription.ControlVolume = true;
            this.velSound = new Tgc3dSound(GlobalConcepts.GetInstance().GetMediaDir() + "Sound\\Motor.wav", position, GlobalConcepts.GetInstance().GetDispositivoDeAudio(), bufferDescription);

            bufferDescription = new BufferDescription();
            bufferDescription.ControlVolume = true;
            this.jump = new Tgc3dSound(GlobalConcepts.GetInstance().GetMediaDir() + "Sound\\Salto.wav", position, GlobalConcepts.GetInstance().GetDispositivoDeAudio(), bufferDescription);

            bufferDescription = new BufferDescription();
            bufferDescription.ControlVolume = true;
            this.defaultWeapon = new Tgc3dSound(GlobalConcepts.GetInstance().GetMediaDir() + "Sound\\Metralleta.wav", position, GlobalConcepts.GetInstance().GetDispositivoDeAudio(), bufferDescription);

            bufferDescription = new BufferDescription();
            bufferDescription.ControlVolume = true;
            this.horn = new Tgc3dSound(GlobalConcepts.GetInstance().GetMediaDir() + "Sound\\Bocina3.wav", position, GlobalConcepts.GetInstance().GetDispositivoDeAudio(), bufferDescription);

            bufferDescription = new BufferDescription();
            bufferDescription.ControlVolume = true;
            this.alarm = new Tgc3dSound(GlobalConcepts.GetInstance().GetMediaDir() + "Sound\\Alarma.wav", position, GlobalConcepts.GetInstance().GetDispositivoDeAudio(), bufferDescription);

            bufferDescription = new BufferDescription();
            bufferDescription.ControlVolume = true;
            this.drop = new Tgc3dSound(GlobalConcepts.GetInstance().GetMediaDir() + "Sound\\Caida.wav", position, GlobalConcepts.GetInstance().GetDispositivoDeAudio(), bufferDescription);

            bufferDescription = new BufferDescription();
            bufferDescription.ControlVolume = true;
            this.crash = new Tgc3dSound(GlobalConcepts.GetInstance().GetMediaDir() + "Sound\\Choque2.wav", position, GlobalConcepts.GetInstance().GetDispositivoDeAudio(), bufferDescription);

            bufferDescription = new BufferDescription();
            bufferDescription.ControlVolume = true;
            this.backgroundMusic = new Tgc3dSound(GlobalConcepts.GetInstance().GetMediaDir() + "Sound\\YouCouldBeMine.wav", position, GlobalConcepts.GetInstance().GetDispositivoDeAudio(), bufferDescription);

            this.initialFreq = this.velSound.SoundBuffer.Frequency;
            //el sonido va de 0 (maximo volumen) a -10000 (total silencio)
            this.velSound.SoundBuffer.Volume = -1000;
            this.velSound.play(true);

            this.backgroundMusic.SoundBuffer.Volume = -2000;
            this.backgroundMusic.play(true);
        }

        public void Crash()
        {
            this.crash.play();
        }

        public void Shoot()
        {
            this.defaultWeapon.play(true);
        }

        public void StopShoot()
        {
            this.defaultWeapon.stop();
        }

        public void Jump()
        {
            this.jump.play();
        }

        public void Horn()
        {
            this.horn.play();
        }

        public void Alarm()
        {
            if (!this.alarm.SoundBuffer.Status.Playing)
            {
                this.alarm.play();
            }
            else
            {
                this.alarm.stop();
            }
            
        }

        public void Drop()
        {
            this.drop.play();
        }

        private void SetFrequency(int freq)
        {
            this.velSound.SoundBuffer.Frequency = freq > this.initialFreq ? freq : this.initialFreq;
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
