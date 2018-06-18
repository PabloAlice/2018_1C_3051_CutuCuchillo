using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Group.Model.Vehiculos;
using Microsoft.DirectX.DirectInput;

namespace TGC.Group.Model.GameModelStates
{
    class CharacterSelect : GameModelState
    {
        private GameModel gameModel;
        private TgcMesh piso;
        private List<Vehicle> autos;
        private Vehicle selectedCar;
        private SoundsManager soundManager = new SoundsManager();
        private int CarsCount { get; set; }
        private int SelectedCarIndex { get; set; }



        public CharacterSelect(GameModel gameModel)
        {
            this.gameModel = gameModel;

            this.piso = new TgcSceneLoader().loadSceneFromFile(GlobalConcepts.GetInstance().GetMediaDir() + "MeshCreator\\Scenes\\CiudadBerreta\\CiudadBerreta-TgcScene.xml").Meshes[2];
            this.gameModel.Camara = new ThirdPersonCamera(new TGCVector3(0, 0, 0), 100f, 150f);

            var auto1 = new Van(new TGCVector3(0f, 0f, 0f), soundManager);
            soundManager.GetSound("Motor").stop();
            auto1.matrixs.SetScalation(TGCMatrix.Scaling(0.2f, 0.2f, 0.2f));
            auto1.Transform();

            var auto2 = new Van(new TGCVector3(0f, 0f, 0f), soundManager);
            soundManager.GetSound("Motor").stop();
            auto2.matrixs.SetScalation(TGCMatrix.Scaling(0.3f, 0.3f, 0.3f));
            auto2.Transform();

            var auto3 = new Van(new TGCVector3(0f, 0f, 0f), soundManager);
            soundManager.GetSound("Motor").stop();
            auto3.matrixs.SetScalation(TGCMatrix.Scaling(0.4f, 0.4f, 0.4f));
            auto3.Transform();

            autos = new List<Vehicle>
            {
                auto1,
                auto2,
                auto3
            };

            selectedCar = auto1;
            CarsCount = autos.Count - 1;

        }

        public override void Render()
        {
            piso.Render();
            selectedCar.Render();
        }

        public override void Update()
        {
            if(gameModel.Input.keyDown(Key.RightArrow))
            {
                NextCar();
            }

            if(gameModel.Input.keyDown(Key.LeftArrow))
            {

                PreviousCar();
            }

            if(gameModel.Input.keyDown(Key.Return))
            {
                gameModel.SetState(new Playing(gameModel, selectedCar));
            }
        }

        public override void Dispose()
        {
            piso.Dispose();
            selectedCar.Dispose();
        }

        private void NextCar()
        {
            SelectedCarIndex++;
            if (SelectedCarIndex > CarsCount) SelectedCarIndex = 0;
            selectedCar = autos[SelectedCarIndex];
        }

        private void PreviousCar()
        {
            SelectedCarIndex--;
            if (SelectedCarIndex < 0) SelectedCarIndex = CarsCount;
            selectedCar = autos[SelectedCarIndex];
        }
    }
}
