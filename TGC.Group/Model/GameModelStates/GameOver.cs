using Microsoft.DirectX.DirectInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Direct3D;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Core.Text;
using TGC.Group.Model.Vehiculos;

namespace TGC.Group.Model.GameModelStates
{
    class GameOver : GameModelState
    {
        private Vehicle auto;
        private ThirdPersonCamera camaraInterna;
        private TGCVector3 camaraDesplazamiento = new TGCVector3(0, 5, 40);
        private CustomSprite gameOver;
        private Drawer2D drawer;
        private readonly string MediaDir = GlobalConcepts.GetInstance().GetMediaDir();
        private GameModel gameModel;

        public GameOver(GameModel gameModel, Vehicle car)
        {
            this.gameModel = gameModel;

            var deviceWidth = D3DDevice.Instance.Width;
            var deviceHeight = D3DDevice.Instance.Height;
            drawer = new Drawer2D();
            gameOver = new CustomSprite();
            gameOver.Bitmap = new CustomBitmap(MediaDir + "GUI\\Menu\\game-over.png", D3DDevice.Instance.Device);
            gameOver.Position = new TGCVector2((deviceWidth / 2f) - gameOver.Bitmap.Width / 4, deviceHeight * 0.3f);
            gameOver.Scaling = TGCVector2.One * 0.5f;
            //this.camaraManagement = new CamaraEnTerceraPersona(camaraDesplazamiento, 3f, -50);
            //this.auto = new Van(new TGCVector3(-60f, 0f, 0f), new SoundsManager());
            this.auto = car;
            auto.ResetScale();
            //this.auto.mesh.D3dMesh.ComputeNormals();
            Scene.GetInstance().SetVehiculo(this.auto);
            this.auto.SoundsManager.AddSound(this.auto.GetPosition(), 50f, 0, "BackgroundMusic\\YouCouldBeMine.wav", "YouCouldBeMine", false);
            this.auto.SoundsManager.GetSound("YouCouldBeMine").play(true);
            this.Update();
        }




        public override void Render()
        {
            Scene.GetInstance().Render();

            this.auto.Transform();
            this.auto.Render();

            //this.manager.Transform();
            //this.manager.Render();

            drawer.BeginDrawSprite();

            drawer.DrawSprite(gameOver);

            //Finalizar el dibujado de Sprites
            drawer.EndDrawSprite();

        }

        public override void Update()
        {
            Scene.GetInstance().camera.Update(auto);
            //this.manager.Action(this.Input);
            if (gameModel.Input.keyDown(Key.Return))
            {
                /*auto.Dispose();
                gameModel.SetState(new InitialMenu(gameModel));
                gameModel.GetState().Init();*/
                Program.Exit();
            }

        }

        public override void Dispose()
        {
            Scene.GetInstance().Dispose();
            this.auto.Dispose();
            gameOver.Dispose();
        }
    }
}
