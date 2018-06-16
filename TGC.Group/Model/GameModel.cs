using TGC.Core.Direct3D;
using TGC.Core.Example;
using TGC.Core.Mathematica;
using TGC.Group.Model.Vehiculos;
using TGC.Core.Text;
using Microsoft.DirectX.Direct3D;
using System.Drawing;
using TGC.Core.Textures;
using TGC.Core.SceneLoader;
using TGC.Core.Geometry;

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
        private ArtificialIntelligence AI;
        private ThirdPersonCamera camaraInterna;
        private TGCVector3 camaraDesplazamiento = new TGCVector3(0, 5, 40);
        private TgcText2D textoVelocidadVehiculo, textoOffsetH, textoOffsetF, textoPosicionVehiculo, textoVectorAdelante, AIPosition;
        private Drawer2D drawer;
        private TgcMesh listener;
        private CustomSprite velocimeter, arrowVelocimeter, barOfLifeGreen, barOfLifeRed, menuBackground, pressStart;
        private bool enterMenu = false;
        private TgcArrow arrow;

        public override void Init()
        {
            var deviceHeight = D3DDevice.Instance.Height;
            var deviceWidth = D3DDevice.Instance.Width;

            GlobalConcepts.GetInstance().SetMediaDir(this.MediaDir);
            GlobalConcepts.GetInstance().SetShaderDir(this.ShadersDir);
            GlobalConcepts.GetInstance().SetDispositivoDeAudio(this.DirectSound.DsDevice);
            GlobalConcepts.GetInstance().SetScreen(D3DDevice.Instance.Device);
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

            Scene.GetInstance().Init();
            this.camaraInterna = new ThirdPersonCamera(camaraDesplazamiento, 0.8f, -33);
            //this.camaraManagement = new CamaraEnTerceraPersona(camaraDesplazamiento, 3f, -50);
            this.Camara = camaraInterna;
            this.auto = new Van(new TGCVector3(-60f, 0f, 0f), new SoundsManager());
            //this.auto.mesh.D3dMesh.ComputeNormals();
            listener = this.auto.mesh.clone("clon");
            this.DirectSound.ListenerTracking = listener;
            Scene.GetInstance().SetVehiculo(this.auto);
            this.AI = new ArtificialIntelligence(new TGCVector3(70f, 0f, 0f), new SoundsManager());
            Scene.GetInstance().AI = this.AI;
            Scene.GetInstance().SetCamera(camaraInterna);
            this.auto.SoundsManager.AddSound(this.auto.GetPosition(), 50f, -2500, "BackgroundMusic\\YouCouldBeMine.wav", "YouCouldBeMine", false);
            this.auto.SoundsManager.GetSound("YouCouldBeMine").play(true);
            //this.Camara = camaraManagement;
            
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
            dialogo = string.Format(dialogo, auto.GetPosition().X, auto.GetPosition().Y, auto.GetPosition().Z);
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

            dialogo = "AI Position = ({0} | {1} | {2}";
            dialogo = string.Format(dialogo, AI.GetPosition().X, AI.GetPosition().Y, AI.GetPosition().Z);
            AIPosition = Text.newText(dialogo, 120, 95);

            this.auto.Action(this.Input);
            this.AI.Action(this.Input);
            this.listener.Position = this.auto.GetPosition();
            Scene.GetInstance().camera.Update(auto);
            //this.manager.Action(this.Input);
            Scene.GetInstance().HandleCollisions();
            this.UpdateHub();
            this.arrow = TgcArrow.fromDirection(this.auto.GetPosition(), Scene.GetInstance().camera.GetNormal());

            //Comentado para que los sonidos funcionen correctamente
            //this.auto = Escena.getInstance().calculateCollisions(this.auto);

            if (Input.keyDown(Microsoft.DirectX.DirectInput.Key.Return))
            {
                enterMenu = false;
            }
            
            this.PostUpdate();
        }

        private void UpdateHub()
        {
            float velocidadMaxima = (this.auto.GetVelocidadActual() < 0) ? this.auto.GetMaxBackwardVelocity() : this.auto.GetMaxForwardVelocity();
            float maxAngle = (this.auto.GetVelocidadActual() > 0) ? FastMath.PI + FastMath.PI / 3 : FastMath.PI_HALF;
            this.arrowVelocimeter.Rotation = (FastMath.Abs(this.auto.GetVelocidadActual()) * (maxAngle)) / velocidadMaxima - FastMath.PI;
            this.barOfLifeGreen.Scaling = new TGCVector2((this.auto.GetLife() * 0.07f) / 100f, 0.05f);
        }

        public override void Render()
        {
            D3DDevice.Instance.ParticlesEnabled = true;
            D3DDevice.Instance.EnableParticles();
            TexturesManager.Instance.clearAll();
            
            D3DDevice.Instance.Device.BeginScene();
            D3DDevice.Instance.Device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1.0f, 0);
            //D3DDevice.Instance.Device.RenderState.FillMode = FillMode.WireFrame;
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
                this.arrow.Render();
                this.textoVelocidadVehiculo.render();
                this.textoPosicionVehiculo.render();
                this.textoVectorAdelante.render();
                this.textoOffsetF.render();
                this.textoOffsetH.render();
                this.AIPosition.render();

                this.auto.Transform();
                this.auto.Render();

                this.AI.Transform();
                this.AI.Render();

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
            this.RenderFPS();
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