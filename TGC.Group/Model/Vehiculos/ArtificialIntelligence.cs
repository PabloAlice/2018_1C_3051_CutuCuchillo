using TGC.Core.Input;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using Microsoft.DirectX.DirectInput;
using TGC.Core.BoundingVolumes;
using TGC.Group.Model.Vehiculos.AIStates;

namespace TGC.Group.Model.Vehiculos
{
    class ArtificialIntelligence : Vehicle
    {
        TgcBoundingSphere radarSphere = new TgcBoundingSphere();
        public AIState aiState;

        public ArtificialIntelligence(TGCVector3 posicionInicial, SoundsManager soundsManager) : base(posicionInicial, soundsManager)
        {
            this.CrearMesh(GlobalConcepts.GetInstance().GetMediaDir() + "meshCreator\\meshes\\Vehiculos\\Camioneta\\Camioneta-TgcScene.xml", posicionInicial);
            this.CreateWheels();
            radarSphere.setValues(this.GetPosition(), 75f);
            this.aiState = new StandBy(this);

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
            var playerCar = Scene.GetInstance().auto;
            if (TGC.Core.Collision.TgcCollisionUtils.testSphereOBB(this.radarSphere, playerCar.GetTGCBoundingOrientedBox()))
            {
                //this.estado.Jump();
            }
            /*
            if (input.keyDown(Key.L))
            {
                this.estado.Advance();
            }

            if (!input.keyDown(Key.L) && !input.keyDown(Key.S))
            {
                this.estado.SpeedUpdate();
            }*/
            return;
        }

        override public void Render()
        {
            base.Render();
            radarSphere.Render();
        }

        override protected void UpdateValues()
        {
            base.UpdateValues();

        }
        
    }
}
