using TGC.Core.Direct3D;
using TGC.Core.Example;
using TGC.Core.Mathematica;
using TGC.Group.Model.Vehiculos;
using TGC.Core.Text;
using Microsoft.DirectX.Direct3D;
using System;
using System.Drawing;
using TGC.Core.Textures;

namespace TGC.Group.Model
{
    public class GameModel : TgcExample
    {

        public GameModel(string mediaDir, string shadersDir) : base(mediaDir, shadersDir)
        {
            Category = Game.Default.Category;
            Name = Game.Default.Name;
            Description = Game.Default.Description;
        }

        private Van auto;
        private ObjectManagement manager;
        private ThirdPersonCamera camaraInterna;
        private ThirdPersonCamera camaraManagement;
        private TGCVector3 camaraDesplazamiento = new TGCVector3(0, 5, 40);
        private TgcText2D textoVelocidadVehiculo, textoOffsetH, textoOffsetF, textoPosicionVehiculo, textoVectorAdelante;
        private Drawer2D drawer;
        private CustomSprite velocimeter, arrowVelocimeter, barOfLifeGreen, barOfLifeRed, menuBackground, pressStart;
        private bool enterMenu = false;

        public override void Init()
        {
            var deviceHeight = D3DDevice.Instance.Height;
            var deviceWidth = D3DDevice.Instance.Width;

            drawer = new Drawer2D();

            velocimeter = new CustomSprite();
            velocimeter.Bitmap = new CustomBitmap(MediaDir + "GUI\\HUB\\Velocimetro\\VelocimetroSinFlecha.png", D3DDevice.Instance.Device);
            velocimeter.Position = new TGCVector2(D3DDevice.Instance.Width * 0.84f, D3DDevice.Instance.Height * 0.70f);
            velocimeter.Scaling = new TGCVector2(0.2f, 0.2f);

            arrowVelocimeter = new CustomSprite();
            arrowVelocimeter.Bitmap = new CustomBitmap(MediaDir + "GUI\\HUB\\Velocimetro\\Flecha.png", D3DDevice.Instance.Device);
            arrowVelocimeter.Position = new TGCVector2(D3DDevice.Instance.Width * 0.915f, D3DDevice.Instance.Height * 0.85f);
            arrowVelocimeter.Scaling = new TGCVector2(0.2f, 0.2f);
            arrowVelocimeter.RotationCenter = new TGCVector2(0, arrowVelocimeter.Bitmap.Height / 8);
            arrowVelocimeter.Rotation = -FastMath.PI;
            //flechaVelocimetro.TransformationMatrix = TGCMatrix.Transformation2D(new TGCVector2(0,0), 0, new TGCVector2(0.2f, 0.2f), new TGCVector2(0,0), FastMath.PI + FastMath.PI_HALF, new TGCVector2(D3DDevice.Instance.Width * 0.84f, D3DDevice.Instance.Height * 0.85f));
            //flechaVelocimetro.RotationCenter = new TGCVector2(D3DDevice.Instance.Width * 0.84f + flechaVelocimetro.Bitmap.Width/2, D3DDevice.Instance.Height * 0.85f + flechaVelocimetro.Bitmap.Height/2);
            //flechaVelocimetro.Rotation = FastMath.PI + FastMath.QUARTER_PI;

            barOfLifeGreen = new CustomSprite();
            barOfLifeGreen.Bitmap = new CustomBitmap(MediaDir + "GUI\\HUB\\BarraDeVida\\1.jpg", D3DDevice.Instance.Device);
            barOfLifeGreen.Position = new TGCVector2(D3DDevice.Instance.Width * 0.80f, D3DDevice.Instance.Height * 0.95f);
            barOfLifeGreen.Scaling = new TGCVector2(0.05f, 0.05f);

            barOfLifeRed = new CustomSprite();
            barOfLifeRed.Bitmap = new CustomBitmap(MediaDir + "GUI\\HUB\\BarraDeVida\\2.jpg", D3DDevice.Instance.Device);
            barOfLifeRed.Position = new TGCVector2(D3DDevice.Instance.Width * 0.80f, D3DDevice.Instance.Height * 0.95f);
            barOfLifeRed.Scaling = new TGCVector2(0.07f, 0.05f);

            menuBackground = new CustomSprite
            {
                Bitmap = new CustomBitmap(MediaDir + "GUI\\Menu\\background.jpg", D3DDevice.Instance.Device),

                Position = new TGCVector2(0f, 0f)
            };

            var menuBackgroundHeight = menuBackground.Bitmap.Height;
            var menuBackgroundWidth = menuBackground.Bitmap.Width;
            //Se debe poner la logica para el caso en el que el size del device sea mayor que la imagen;
            var scaleWidth = deviceWidth / (float)menuBackgroundWidth;
            var scaleHeight = deviceHeight / (float)menuBackgroundHeight;
            menuBackground.Scaling = new TGCVector2(scaleWidth, scaleHeight);

            pressStart = new CustomSprite();
            pressStart.Bitmap = new CustomBitmap(MediaDir + "GUI\\Menu\\press-start.png", D3DDevice.Instance.Device);
            pressStart.Position = new TGCVector2((deviceWidth / 2f) - pressStart.Bitmap.Width / 2, deviceHeight / 8f);

            GlobalConcepts.GetInstance().SetMediaDir(this.MediaDir);
            GlobalConcepts.GetInstance().SetShaderDir(this.ShadersDir);
            GlobalConcepts.GetInstance().SetDispositivoDeAudio(this.DirectSound.DsDevice);
            GlobalConcepts.GetInstance().SetScreen(D3DDevice.Instance.Device);
            Scene.GetInstance().Init(this.MediaDir);
            this.camaraInterna = new ThirdPersonCamera(camaraDesplazamiento, 0.8f, -33);
            //this.camaraManagement = new CamaraEnTerceraPersona(camaraDesplazamiento, 3f, -50);
            this.Camara = camaraInterna;
            //this.Camara = camaraManagement;
            this.auto = new Van(camaraInterna, new TGCVector3(-60f, 0f, 0f), new SoundsManager(new TGCVector3(-0f, 0f, 0f)));
            Scene.GetInstance().SetVehiculo(this.auto);
            //manager = new ObjectManagement(MediaDir + "meshCreator\\meshes\\Habitacion\\Billetes\\Billete2\\Billete2-TgcScene.xml", camaraManagement);

            
        }

        public override void Update()
        {

            this.PreUpdate();
            GlobalConcepts.GetInstance().SetElapsedTime(ElapsedTime);

            string dialogo;

            dialogo = "Velocidad = {0}km";
            dialogo = string.Format(dialogo, auto.GetVelocidadActual());
            textoVelocidadVehiculo = Text.newText(dialogo, 120, 10);

            dialogo = "Posicion = ({0} | {1} | {2})";
            dialogo = string.Format(dialogo, auto.GetPosicion().X, auto.GetPosicion().Y, auto.GetPosicion().Z);
            textoPosicionVehiculo = Text.newText(dialogo, 120, 25);

            dialogo = "VectorAdelante = ({0} | {1} | {2})";
            dialogo = string.Format(dialogo, auto.GetVectorAdelante().X, auto.GetVectorAdelante().Y, auto.GetVectorAdelante().Z);
            textoVectorAdelante = Text.newText(dialogo, 120, 40);

            dialogo = "OffsetHeight = {0}";
            dialogo = string.Format(dialogo, this.camaraInterna.OffsetHeight);
            textoOffsetH = Text.newText(dialogo, 120, 70);

            dialogo = "OffsetForward = {0}";
            dialogo = string.Format(dialogo, this.camaraInterna.OffsetForward);
            textoOffsetF = Text.newText(dialogo, 120, 85);


            
            this.auto.SetElapsedTime(ElapsedTime);
            this.auto.Action(this.Input, this.arrowVelocimeter, this.barOfLifeGreen);
            //this.manager.Action(this.Input);
            Scene.GetInstance().HandleCollisions();

            //Comentado para que los sonidos funcionen correctamente
            //this.auto = Escena.getInstance().calculateCollisions(this.auto);

            if (Input.keyDown(Microsoft.DirectX.DirectInput.Key.Return))
            {
                enterMenu = false;
            }
            
            this.PostUpdate();
        }



        public override void Render()
        {
            
            TexturesManager.Instance.clearAll();
            D3DDevice.Instance.Device.BeginScene();
            D3DDevice.Instance.Device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1.0f, 0);
        
            if (enterMenu)
            {
                drawer.BeginDrawSprite();
                drawer.DrawSprite(menuBackground);
                drawer.DrawSprite(pressStart);
                drawer.EndDrawSprite();
            }
            else
            {

                Scene.GetInstance().Render();

                this.textoVelocidadVehiculo.render();
                this.textoPosicionVehiculo.render();
                this.textoVectorAdelante.render();
                this.textoOffsetF.render();
                this.textoOffsetH.render();

                this.auto.Transform();
                this.auto.Render();

                //this.manager.Transform();
                //this.manager.Render();

                drawer.BeginDrawSprite();

                drawer.DrawSprite(velocimeter);
                drawer.DrawSprite(arrowVelocimeter);
                drawer.DrawSprite(barOfLifeRed);
                drawer.DrawSprite(barOfLifeGreen);

                //Finalizar el dibujado de Sprites
                drawer.EndDrawSprite();
                
            }


            D3DDevice.Instance.Device.EndScene();
            D3DDevice.Instance.Device.Present();
        }

        public override void Dispose()
        {
            Scene.GetInstance().Dispose();
            this.auto.Dispose();
            velocimeter.Dispose();
            arrowVelocimeter.Dispose();
            menuBackground.Dispose();
        }

    }
}