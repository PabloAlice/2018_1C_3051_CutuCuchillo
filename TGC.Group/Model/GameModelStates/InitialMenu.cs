using Microsoft.DirectX.DirectInput;
using TGC.Core.Direct3D;
using TGC.Core.Mathematica;
using TGC.Group.Model.Vehiculos;

namespace TGC.Group.Model.GameModelStates
{
    class InitialMenu : GameModelState
    {
        private GameModel gameModel;

        private string MediaDir = GlobalConcepts.GetInstance().GetMediaDir();
        private CustomSprite pressStart, title;
        private Drawer2D drawer = new Drawer2D();

        public InitialMenu(GameModel gameModel)
        {
            this.gameModel = gameModel;
            var deviceWidth = D3DDevice.Instance.Width;
            var deviceHeight = D3DDevice.Instance.Height;
            pressStart = new CustomSprite();
            pressStart.Bitmap = new CustomBitmap(MediaDir + "GUI\\Menu\\press-start.png", D3DDevice.Instance.Device);
            pressStart.Position = new TGCVector2((deviceWidth / 2f) - pressStart.Bitmap.Width * 5 / 20, deviceHeight * 0.7f);
            pressStart.Scaling = TGCVector2.One * 0.5f;

            title = new CustomSprite();
            title.Bitmap = new CustomBitmap(MediaDir + "GUI\\Menu\\title.png", D3DDevice.Instance.Device);
            title.Position = new TGCVector2((deviceWidth / 2f) - title.Bitmap.Width * 3 / 20, deviceHeight * 0.1f);
            title.Scaling = TGCVector2.One * 0.3f;
        }
        public override void Init()
        {
            
            var camaraInterna = new ThirdPersonCamera(new TGCVector3(0f, 0f, 0f), 12.6f, -81.5f);
            this.gameModel.Camara = camaraInterna;
            Scene.GetInstance().SetCamera(camaraInterna);
            Scene.GetInstance().Init();
        }

        public override void Render()
        {
            drawer.BeginDrawSprite();
            drawer.DrawSprite(pressStart);
            drawer.DrawSprite(title);
            drawer.EndDrawSprite();

            Scene.GetInstance().RenderRoom();
        }

        public override void Update()
        {
            if (gameModel.Input.keyDown(Key.Return))
            {
                SoundsManager backgroundMusic = new SoundsManager();
                backgroundMusic.AddSound(TGCVector3.Empty, 50f, 0, "Init\\song.wav", "song", false);
                this.gameModel.SetState(new CharacterSelect(gameModel, backgroundMusic.GetSound("song")));
            }


            Scene.GetInstance().camera.rotateY(0.0005f);
        }

        public override void Dispose()
        {
            pressStart.Dispose();
            title.Dispose();
            Scene.GetInstance().Dispose();
        }
    }
}
