using Microsoft.DirectX.DirectInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Direct3D;
using TGC.Core.Input;
using TGC.Core.Mathematica;
using TGC.Core.Text;

namespace TGC.Group.Model.GameModelStates
{
    class InitialMenu : GameModelState
    {
        private GameModel gameModel;

        private string MediaDir = GlobalConcepts.GetInstance().GetMediaDir();
        private CustomSprite pressStart;
        private Drawer2D drawer = new Drawer2D();

        public InitialMenu(GameModel gameModel)
        {
            this.gameModel = gameModel;
            var deviceWidth = D3DDevice.Instance.Width;
            var deviceHeight = D3DDevice.Instance.Height;
            var camaraInterna = new ThirdPersonCamera(new TGCVector3(0f, 0f, 0f), 12.6f, -81.5f);
            Scene.GetInstance().SetCamera(camaraInterna);
            this.gameModel.Camara = camaraInterna;
            pressStart = new CustomSprite();
            pressStart.Bitmap = new CustomBitmap(MediaDir + "GUI\\Menu\\press-start.png", D3DDevice.Instance.Device);
            pressStart.Position = new TGCVector2((deviceWidth / 2f) - pressStart.Bitmap.Width / 2, deviceHeight / 8f);

        }

        public override void Render()
        {
            drawer.BeginDrawSprite();
            drawer.DrawSprite(pressStart);
            drawer.EndDrawSprite();

            Scene.GetInstance().RenderRoom();
        }

        public override void Update()
        {
            if (gameModel.Input.keyDown(Microsoft.DirectX.DirectInput.Key.Return))
            {
                this.gameModel.SetState(new CharacterSelect(gameModel));
            }


            Scene.GetInstance().camera.rotateY(0.0005f);
        }

        public override void Dispose()
        {
            pressStart.Dispose();
            Scene.GetInstance().Dispose();
        }
    }
}
