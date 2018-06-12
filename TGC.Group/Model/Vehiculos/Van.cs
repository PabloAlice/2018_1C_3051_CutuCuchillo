using TGC.Core.SceneLoader;
using TGC.Core.Mathematica;

namespace TGC.Group.Model.Vehiculos
{
    class Van : Vehicle
    {
        public Van(TGCVector3 posicionInicial, SoundsManager soundsManager) : base(posicionInicial, soundsManager)
        {
            this.CrearMesh(GlobalConcepts.GetInstance().GetMediaDir() + "meshCreator\\meshes\\Vehiculos\\Camioneta\\Camioneta-TgcScene.xml", posicionInicial);
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
            delanteraIzquierda = new Wheel(ruedaIzquierda, new TGCVector3(35f, 15f, 63f));
            delanteraDerecha = new Wheel(ruedaDerecha, new TGCVector3(-35f, 15f, 63f));
            ruedas.Add(new Wheel(ruedaTraseraIzquierda, new TGCVector3(38f, 18f, -61f)));
            ruedas.Add(new Wheel(ruedaTraseraDerecha, new TGCVector3(-34f, 18f, -61f)));
        }

        override protected void CreateSounds(SoundsManager soundsManager)
        {
            TGCVector3 position = this.GetPosition();
            soundsManager.AddSound(position, 50f, -2500,"Van\\Motor.wav", "Motor", true);
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
