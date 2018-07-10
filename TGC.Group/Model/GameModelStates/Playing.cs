using Microsoft.DirectX.DirectInput;
using TGC.Core.Direct3D;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Core.Text;
using TGC.Group.Model.Vehiculos;

namespace TGC.Group.Model.GameModelStates
{
    class Playing : GameModelState
    {
        private Vehicle auto;
        private ArtificialIntelligence AI;
        private ThirdPersonCamera camaraInterna;
        private TGCVector3 camaraDesplazamiento = new TGCVector3(0, 5, 40);
        private TgcMesh listener;
        private CustomSprite velocimeter, arrowVelocimeter, barOfLifeGreen, barOfLifeRed;
        private Drawer2D drawer;
        private string MediaDir = GlobalConcepts.GetInstance().GetMediaDir();
        private TgcText2D textoVelocidadVehiculo, textoOffsetH, textoArmaSeleccionada, textoOffsetF, textoPosicionVehiculo, textoVectorAdelante, AIPosition;
        private GameModel gameModel;

        public Playing(GameModel gameModel, Vehicle car)
        {
            this.gameModel = gameModel;

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

            this.camaraInterna = new ThirdPersonCamera(camaraDesplazamiento, 0.8f, -33);
            //this.camaraManagement = new CamaraEnTerceraPersona(camaraDesplazamiento, 3f, -50);
            this.gameModel.Camara = camaraInterna;
            //this.auto = new Van(new TGCVector3(-60f, 0f, 0f), new SoundsManager());
            this.auto = car;
            auto.ResetScale();
            //this.auto.mesh.D3dMesh.ComputeNormals();
            listener = this.auto.mesh.clone("clon");
            this.gameModel.DirectSound.ListenerTracking = listener;
            Scene.GetInstance().SetVehiculo(this.auto);
            this.AI = new ArtificialIntelligence(new TGCVector3(70f, 0f, 0f), new SoundsManager());
            Scene.GetInstance().AI = this.AI;
            Scene.GetInstance().SetCamera(camaraInterna);
            this.auto.SoundsManager.AddSound(this.auto.GetPosition(), 50f, -500, "BackgroundMusic\\Burn.wav", "YouCouldBeMine", false);
            this.auto.SoundsManager.GetSound("YouCouldBeMine").play(true);

            /*var aspectRatio = D3DDevice.Instance.AspectRatio;
            TGCMatrix view = TGCMatrix.LookAtLH(gameModel.Camara.Position, gameModel.Camara.LookAt, gameModel.Camara.UpVector);
            TGCMatrix proj = TGCMatrix.PerspectiveFovLH(Geometry.DegreeToRadian(360), aspectRatio, 0, 1);
            this.gameModel.Frustum.updateVolume(view, proj);
            D3DDevice.Instance.Device.Transform.View = view;
            D3DDevice.Instance.Device.Transform.Projection = proj;
            */

            this.Update();
        }
            
        
        

        public override void Render()
        {
            Scene.GetInstance().Render();
            this.textoVelocidadVehiculo.render();
            this.textoPosicionVehiculo.render();
            this.textoVectorAdelante.render();
            this.textoOffsetF.render();
            this.textoOffsetH.render();
            textoArmaSeleccionada.render();
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

        public override void Update()
        {
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
            AIPosition = Text.newText(dialogo, 120, 100);

            dialogo = "Arma = {0}; Cantidad = {1}";
            dialogo = string.Format(dialogo, auto.GetNameSelectWeapon(), auto.GetNumberOfBulletsOfFirstWeapon());
            textoArmaSeleccionada = Text.newText(dialogo, 120, 115);

            this.auto.Action(this.gameModel.Input);
            this.AI.Action(this.gameModel.Input);
            this.listener.Position = this.auto.GetPosition();
            Scene.GetInstance().camera.Update(auto);
            //this.manager.Action(this.Input);
            Scene.GetInstance().HandleCollisions();
            this.UpdateHub();

            if (auto.GetLife() <= 0)
            {
                this.auto.SoundsManager.GetSound("YouCouldBeMine").stop();
                // Agregar grito
                this.auto.SoundsManager.AddSound(this.auto.GetPosition(), 50f, 0, "Muerte.wav", "muerte", false);
                this.auto.SoundsManager.GetSound("muerte").play();
                gameModel.SetState(new GameOver(gameModel, auto));
            }
        }

        private void UpdateHub()
        {
            float velocidadMaxima = (this.auto.GetVelocidadActual() < 0) ? this.auto.GetMaxBackwardVelocity() : this.auto.GetMaxForwardVelocity();
            float maxAngle = (this.auto.GetVelocidadActual() > 0) ? FastMath.PI + FastMath.PI / 3 : FastMath.PI_HALF;
            this.arrowVelocimeter.Rotation = (FastMath.Abs(this.auto.GetVelocidadActual()) * (maxAngle)) / velocidadMaxima - FastMath.PI;
            this.barOfLifeGreen.Scaling = new TGCVector2((this.auto.GetLife() * 0.07f) / 100f, 0.05f);
        }

        public override void Dispose()
        {
            Scene.GetInstance().Dispose();
            this.auto.Dispose();
            velocimeter.Dispose();
            arrowVelocimeter.Dispose();
        }
    }
}
