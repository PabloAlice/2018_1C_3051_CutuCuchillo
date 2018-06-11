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
            soundsManager.AddSound(position, 50f, -2500,"Motor.wav", "Motor");
            soundsManager.AddSound(position, 50f, 0, "Salto.wav", "Salto");
            soundsManager.AddSound(position, 50f, 0, "Bocina3.wav", "Bocina");
            soundsManager.AddSound(position, 50f, 0, "Alarma.wav", "Alarma");
            soundsManager.AddSound(position, 50f, 0, "Caida.wav", "Caida");
            soundsManager.AddSound(position, 50f, 0, "Choque2.wav", "Choque");
            soundsManager.GetSound("Motor").play(true);

        }
    }
}
