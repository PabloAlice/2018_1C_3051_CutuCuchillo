using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Direct3D;
using TGC.Core.Mathematica;

namespace TGC.Group.Model.GameModelStates
{
    class InitialMenu : GameModelState
    {
        private GameModel gameModel;

        private string MediaDir = GlobalConcepts.GetInstance().GetMediaDir();
        private CustomSprite menuBackground, pressStart;
        private Drawer2D drawer = new Drawer2D();

        public InitialMenu(GameModel gameModel)
        {
            this.gameModel = gameModel;

            menuBackground = new CustomSprite
            {
                Bitmap = new CustomBitmap(MediaDir + "GUI\\Menu\\background.jpg", D3DDevice.Instance.Device),

                Position = new TGCVector2(0f, 0f)
            };

            var menuBackgroundHeight = menuBackground.Bitmap.Height;
            var menuBackgroundWidth = menuBackground.Bitmap.Width;
            //Se debe poner la logica para el caso en el que el size del device sea mayor que la imagen;
            var deviceWidth = D3DDevice.Instance.Width;
            var deviceHeight = D3DDevice.Instance.Height;
            var scaleWidth = deviceWidth / (float)menuBackgroundWidth;
            var scaleHeight = deviceHeight / (float)menuBackgroundHeight;
            menuBackground.Scaling = new TGCVector2(scaleWidth, scaleHeight);

            pressStart = new CustomSprite();
            pressStart.Bitmap = new CustomBitmap(MediaDir + "GUI\\Menu\\press-start.png", D3DDevice.Instance.Device);
            pressStart.Position = new TGCVector2((deviceWidth / 2f) - pressStart.Bitmap.Width / 2, deviceHeight / 8f);

        }

        public override void Render()
        {
            drawer.BeginDrawSprite();
            drawer.DrawSprite(menuBackground);
            drawer.DrawSprite(pressStart);
            drawer.EndDrawSprite();
        }

        public override void Update()
        {
            if (gameModel.Input.keyDown(Microsoft.DirectX.DirectInput.Key.Return))
            {
                this.gameModel.SetState(new CharacterSelect(gameModel));
            }
        }

        public override void Dispose()
        {
            pressStart.Dispose();
            menuBackground.Dispose();

        }
    }
}
