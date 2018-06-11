using TGC.Core.Input;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using Microsoft.DirectX.DirectInput;

namespace TGC.Group.Model.Vehiculos
{
    class ArtificialIntelligence : Vehicle
    {
        public ArtificialIntelligence(TGCVector3 posicionInicial, SoundsManager soundsManager) : base(posicionInicial, soundsManager)
        {
            this.CrearMesh(GlobalConcepts.GetInstance().GetMediaDir() + "meshCreator\\meshes\\Vehiculos\\Camioneta\\Camioneta-TgcScene.xml", posicionInicial);
            this.CreateWheels();

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

        protected override void ManageEntry(TgcD3dInput input)
        {
            return;
        }
    }
}
