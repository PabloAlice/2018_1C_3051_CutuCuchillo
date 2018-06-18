using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Group.Model.Vehiculos;
using Microsoft.DirectX.DirectInput;
using TGC.Core.Direct3D;

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
        private float keyDownTime;
        private string MediaDir = GlobalConcepts.GetInstance().GetMediaDir();
        private Drawer2D drawer = new Drawer2D();
        private CustomSprite choose;


        public CharacterSelect(GameModel gameModel)
        {
            this.gameModel = gameModel;

            this.piso = new TgcSceneLoader().loadSceneFromFile(GlobalConcepts.GetInstance().GetMediaDir() + "MeshCreator\\Scenes\\CiudadBerreta\\CiudadBerreta-TgcScene.xml").Meshes[2];
            this.gameModel.Camara = new ThirdPersonCamera(new TGCVector3(0, 0, 0), 100f, 150f);

            var auto1 = new Van(new TGCVector3(0f, 0f, 0f), soundManager);
            soundManager.GetSound("Motor").stop();
            auto1.matrixs.SetScalation(TGCMatrix.Scaling(0.2f, 0.2f, 0.2f));
            auto1.Transform();

            var auto2 = new Car(new TGCVector3(0f, 0f, 0f), soundManager);
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


            var deviceWidth = D3DDevice.Instance.Width;
            var deviceHeight = D3DDevice.Instance.Height;

            choose = new CustomSprite();
            choose.Bitmap = new CustomBitmap(MediaDir + "GUI\\Menu\\choose.png", D3DDevice.Instance.Device);
            choose.Position = new TGCVector2((deviceWidth / 2f) - choose.Bitmap.Width / 2, deviceHeight * 0.6f);

        }

        public override void Render()
        {
            drawer.BeginDrawSprite();
            drawer.DrawSprite(choose);
            drawer.EndDrawSprite();

            piso.Render();
            selectedCar.Render();
        }

        public override void Update()
        {
            if(gameModel.Input.keyDown(Key.RightArrow) && keyDownTime == 0f)
            {
                NextCar();
                keyDownTime = 0.5f;
            }

            if(gameModel.Input.keyDown(Key.LeftArrow) && keyDownTime == 0f)
            {
                PreviousCar();
                keyDownTime = 0.5f;
            }

            if(gameModel.Input.keyDown(Key.Return))
            {
                gameModel.SetState(new Playing(gameModel, selectedCar));
            }

            UpdateKeyDownTime();
            selectedCar.Rotate(0.5f * GlobalConcepts.GetInstance().GetElapsedTime());
        }

        public override void Dispose()
        {
            piso.Dispose();
            selectedCar.Dispose();
            choose.Dispose();
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

        private void UpdateKeyDownTime()
        {
            var nextTime = keyDownTime - GlobalConcepts.GetInstance().GetElapsedTime();
            keyDownTime = FastMath.Max(0f, nextTime);
        }
    }
}
