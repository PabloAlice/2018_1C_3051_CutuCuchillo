using TGC.Core.Input;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Core.Collision;
using TGC.Core.BoundingVolumes;
using TGC.Group.Model.Vehiculos.AIStates;
using TGC.Core.Geometry;
using System.Drawing;

namespace TGC.Group.Model.Vehiculos
{
    class ArtificialIntelligence : Vehicle
    {
        TgcBoundingSphere radarSphere = new TgcBoundingSphere();
        public AIState aiState;
        public TGCPlane directionPlane;
        public TGCPlane planoCostado;
        public float time = 0;

        public ArtificialIntelligence(TGCVector3 posicionInicial, SoundsManager soundsManager) : base(posicionInicial, soundsManager)
        {
            this.CrearMesh(GlobalConcepts.GetInstance().GetMediaDir() + "meshCreator\\meshes\\Vehiculos\\Camioneta\\Camioneta-TgcScene.xml", posicionInicial);
            string reverseLightsPath, frontLightsPath, breakLightsPath;
            reverseLightsPath = GlobalConcepts.GetInstance().GetMediaDir() + "meshCreator\\meshes\\Vehiculos\\Camioneta\\LucesMarchaAtras\\Luces-TgcScene.xml";
            frontLightsPath = GlobalConcepts.GetInstance().GetMediaDir() + "meshCreator\\meshes\\Vehiculos\\Camioneta\\LucesDelanteras\\Luces-TgcScene.xml";
            breakLightsPath = GlobalConcepts.GetInstance().GetMediaDir() + "meshCreator\\meshes\\Vehiculos\\Camioneta\\LucesFrenado\\Luces-TgcScene.xml";
            this.CreateLights(reverseLightsPath, breakLightsPath, frontLightsPath);
            this.CreateWheels();
            radarSphere.setValues(this.GetPosition(), 75f);
            radarSphere.setRenderColor(Color.DarkViolet);
            this.aiState = new SearchWeapons(this);
            this.CreateSounds(soundsManager);
            this.velocidadMaximaDeAvance = 20f;
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
            this.aiState.Run();
            return;
        }

        public bool DoIHaveEnoughWeapons()
        {
            return this.weapons.Count > 2;
        }

        public bool IsEnemyInRadar(Vehicle car)
        {
            return TgcCollisionUtils.testSphereOBB(this.radarSphere, car.GetTGCBoundingOrientedBox());
        }

        public void ChangeState(AIState state)
        {
            this.aiState = state;
        }

        override public void Render()
        {
            base.Render();
            radarSphere.Render();
        }

        override protected void UpdateValues()
        {
            base.UpdateValues();
            directionPlane = TGCPlane.FromPointNormal(GetPosition(), -GetVectorCostadoIzquierda());
            planoCostado = TGCPlane.FromPointNormal(GetPosition(), GetVectorAdelante());
            radarSphere.setValues(this.GetPosition(), radarSphere.Radius);
        }
        
    }
}
