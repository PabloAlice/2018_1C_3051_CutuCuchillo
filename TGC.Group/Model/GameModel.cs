using TGC.Core.Direct3D;
using TGC.Core.Example;
using TGC.Core.Mathematica;
using TGC.Group.Model.Vehiculos;
using TGC.Core.Text;
using Microsoft.DirectX.Direct3D;
using System;
using System.Drawing;

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
        private CustomSprite velocimeter, arrowVelocimeter, barOfLife, menuBackground, pressStart;



        public override void Init()
        {
            var deviceHeight = D3DDevice.Instance.Height;
            var deviceWidth = D3DDevice.Instance.Width;

            drawer = new Drawer2D();
            velocimeter = new CustomSprite
            {
                Bitmap = new CustomBitmap(MediaDir + "GUI\\HUB\\Velocimetro\\VelocimetroSinFlecha.png", D3DDevice.Instance.Device),
                Position = new TGCVector2(D3DDevice.Instance.Width * 0.84f, D3DDevice.Instance.Height * 0.70f),
                Scaling = new TGCVector2(0.2f, 0.2f)
            };

            arrowVelocimeter = new CustomSprite
            {
                Bitmap = new CustomBitmap(MediaDir + "GUI\\HUB\\Velocimetro\\Flecha.png", D3DDevice.Instance.Device),
                Position = new TGCVector2(D3DDevice.Instance.Width * 0.84f, D3DDevice.Instance.Height * 0.85f),
                Scaling = new TGCVector2(0.2f, 0.2f)
            };
            //flechaVelocimetro.TransformationMatrix = TGCMatrix.Transformation2D(new TGCVector2(0,0), 0, new TGCVector2(0.2f, 0.2f), new TGCVector2(0,0), FastMath.PI + FastMath.PI_HALF, new TGCVector2(D3DDevice.Instance.Width * 0.84f, D3DDevice.Instance.Height * 0.85f));
            //flechaVelocimetro.RotationCenter = new TGCVector2(D3DDevice.Instance.Width * 0.84f + flechaVelocimetro.Bitmap.Width/2, D3DDevice.Instance.Height * 0.85f + flechaVelocimetro.Bitmap.Height/2);
            //flechaVelocimetro.Rotation = FastMath.PI + FastMath.QUARTER_PI;

            //barOfLife = new CustomSprite();
            //barOfLife.Bitmap = new CustomBitmap(MediaDir + "GUI\\HUB\\Velocimetro\\BarraDeVida.png", D3DDevice.Instance.Device);
            //barOfLife.Position = new TGCVector2(D3DDevice.Instance.Width * 0.84f, D3DDevice.Instance.Height * 0.85f);
            //barOfLife.Scaling = new TGCVector2(0.2f, 0.2f);

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

            pressStart = new CustomSprite
            {
                Bitmap = new CustomBitmap(MediaDir + "GUI\\Menu\\press-start.png", D3DDevice.Instance.Device),
                Position = new TGCVector2((deviceWidth / 2) - pressStart.Bitmap.Width / 2, (deviceHeight / 2) - pressStart.Bitmap.Height)
            };
            

            GlobalConcepts.GetInstance().SetMediaDir(this.MediaDir);
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
            this.auto.Action(this.Input);
            //this.manager.Action(this.Input);
            Scene.GetInstance().HandleCollisions();

            //Comentado para que los sonidos funcionen correctamente
            //this.auto = Escena.getInstance().calculateCollisions(this.auto);
            
            
            
            
            /*
            if (collide)
            {
                    var movementRay = lastPosition - TGCMatrix.Translation(auto.getPosicion());
                    var rs = TGCVector3.Empty;
                    if (((auto.mesh.BoundingBox.PMax.X > collider.BoundingBox.PMax.X && movementRay.X > 0) ||
                        (auto.mesh.BoundingBox.PMin.X < collider.BoundingBox.PMin.X && movementRay.X < 0)) &&
                        ((auto.mesh.BoundingBox.PMax.Z > collider.BoundingBox.PMax.Z && movementRay.Z > 0) ||
                        (auto.mesh.BoundingBox.PMin.Z < collider.BoundingBox.PMin.Z && movementRay.Z < 0)))
                    {
                        if (auto.mesh.Position.X > collider.BoundingBox.PMin.X && auto.mesh.Position.X < collider.BoundingBox.PMax.X)
                        {
                            rs = new TGCVector3(movementRay.X, movementRay.Y, 0);
                        }
                        if (auto.mesh.Position.Z > collider.BoundingBox.PMin.Z && auto.mesh.Position.Z < collider.BoundingBox.PMax.Z)
                        {
                            rs = new TGCVector3(0, movementRay.Y, movementRay.Z);
                        }

                    }
                    else
                    {
                        if ((auto.mesh.BoundingBox.PMax.X > collider.BoundingBox.PMax.X && movementRay.X > 0) ||
                            (auto.mesh.BoundingBox.PMin.X < collider.BoundingBox.PMin.X && movementRay.X < 0))
                        {
                            rs = new TGCVector3(0, movementRay.Y, movementRay.Z);
                        }
                        if ((auto.mesh.BoundingBox.PMax.Z > collider.BoundingBox.PMax.Z && movementRay.Z > 0) ||
                            (auto.mesh.BoundingBox.PMin.Z < collider.BoundingBox.PMin.Z && movementRay.Z < 0))
                        {
                            rs = new TGCVector3(movementRay.X, movementRay.Y, 0);
                        }
                    }
                    auto.mesh.Position = lastPos - rs;
            }*/

            this.PostUpdate();
        }

        

        public override void Render()
        {

            this.PreRender();



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

            //Iniciar dibujado de todos los Sprites de la escena
            drawer.BeginDrawSprite();

            //Dibujar sprite (si hubiese mas, deberian ir todos aquí)
            drawer.DrawSprite(velocimeter);
            drawer.DrawSprite(arrowVelocimeter);
            drawer.DrawSprite(menuBackground);

            pressStart.render();
            //Finalizar el dibujado de Sprites
            drawer.EndDrawSprite();
            this.PostRender();
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