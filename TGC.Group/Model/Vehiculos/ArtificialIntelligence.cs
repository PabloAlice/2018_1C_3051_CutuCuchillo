using TGC.Core.Input;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Core.Collision;
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

        protected override void CreateSounds(SoundsManager soundsManager)
        {
            TGCVector3 position = this.GetPosition();
            soundsManager.AddSound(position, 2f, -1500, "AI\\Motor.wav", "Motor", true);
            soundsManager.AddSound(position, 2f, 0, "AI\\Salto.wav", "Salto", false);
            soundsManager.AddSound(position, 7f, 0, "AI\\Bocina3.wav", "Bocina", false);
            soundsManager.AddSound(position, 5f, 0, "AI\\Alarma.wav", "Alarma", false);
            soundsManager.AddSound(position, 2f, 0, "AI\\Caida.wav", "Caida", false);
            soundsManager.AddSound(position, 4f, 0, "AI\\Choque2.wav", "Choque", false);
            soundsManager.GetSound("Motor").play(true);
            return;
        }

        protected override void ManageEntry(TgcD3dInput input)
        {
            this.DeterminateState();

                
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

        private void DeterminateState()
        {
            Vehicle car = Scene.GetInstance().auto;
            if (TgcCollisionUtils.testSphereOBB(this.radarSphere, car.GetTGCBoundingOrientedBox()))
            {
                this.aiState = new FollowingCar(this);
            }
            else
            {
                this.aiState = new SearchOfWeapons(this);
            }
        }

        override public void Render()
        {
            base.Render();
            radarSphere.Render();
        }

        override protected void UpdateValues()
        {
            base.UpdateValues();
            radarSphere.setValues(this.GetPosition(), radarSphere.Radius);
        }
        
    }
}
