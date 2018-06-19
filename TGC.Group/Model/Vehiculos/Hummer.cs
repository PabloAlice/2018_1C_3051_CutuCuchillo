using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;

namespace TGC.Group.Model.Vehiculos
{
    class Hummer : Vehicle
    {
        public Hummer(TGCVector3 posicionInicial, SoundsManager soundsManager) : base(posicionInicial, soundsManager)
        {

            this.escaladoInicial = new TGCVector3(0.01f, 0.01f, 0.01f);
            this.CrearMesh(GlobalConcepts.GetInstance().GetMediaDir() + "meshCreator\\meshes\\Vehiculos\\Hummer\\Hummer-TgcScene.xml", posicionInicial);
            this.CreateWheels();
            this.CreateSounds(soundsManager);

        }

        private void CreateWheels()
        {
            TgcSceneLoader loader = new TgcSceneLoader();
            TgcMesh ruedaIzquierda = loader.loadSceneFromFile(GlobalConcepts.GetInstance().GetMediaDir() + "meshCreator\\meshes\\Vehiculos\\Camioneta\\Rueda\\Rueda-TgcScene.xml").Meshes[0];
            TgcMesh ruedaDerecha = ruedaIzquierda.clone("ruedaDerecha");
            TgcMesh ruedaTraseraIzquierda = ruedaIzquierda.clone("ruedaTraseraIzquierda");
            TgcMesh ruedaTraseraDerecha = ruedaIzquierda.clone("ruedaTraseraDerecha");
            var wheelScale = TGCVector3.One * 0.48f;
            delanteraIzquierda = new Wheel(ruedaIzquierda, new TGCVector3(23.5f, 7.6f, 30f), wheelScale);
            delanteraDerecha = new Wheel(ruedaDerecha, new TGCVector3(-23.5f, 7.6f, 30f), wheelScale);
            ruedas.Add(new Wheel(ruedaTraseraIzquierda, new TGCVector3(23f, 7.2f, -33f), wheelScale));
            ruedas.Add(new Wheel(ruedaTraseraDerecha, new TGCVector3(-23f, 7.2f, -33f), wheelScale));
        }

        override protected void CreateSounds(SoundsManager soundsManager)
        {
            TGCVector3 position = this.GetPosition();
            soundsManager.AddSound(position, 50f, -2500, "Van\\Motor.wav", "Motor", true);
            soundsManager.AddSound(position, 50f, 0, "Van\\Salto.wav", "Salto", false);
            soundsManager.AddSound(position, 50f, 0, "Van\\Bocina3.wav", "Bocina", false);
            soundsManager.AddSound(position, 50f, 0, "Van\\Alarma.wav", "Alarma", false);
            soundsManager.AddSound(position, 50f, 0, "Van\\Caida.wav", "Caida", false);
            soundsManager.AddSound(position, 50f, 0, "Van\\Choque2.wav", "Choque", false);
            soundsManager.GetSound("Motor").play(true);

        }

        public override void Crash(float angle)
        {
            base.Crash(angle);
            Scene.GetInstance().camera.rotateY(angle);
        }
    }
}
