using TGC.Core.Direct3D;
using TGC.Core.Example;
using TGC.Core.Mathematica;
using TGC.Group.Model.Vehiculos;
using TGC.Core.Text;
using System.Windows.Forms;
using Microsoft.DirectX.Direct3D;
using System.Drawing;
using System;

namespace TGC.Group.Model
{
    public class GameModel : TgcExample
    {

        public enum tipoCursor
        {
            sin_cursor,
            targeting,
            over,
            progress,
            pressed,
            gripped
        }

        public struct st_dialog
        {
            public int item_0;
            public bool trapezoidal_style;
            public bool autohide;
            public bool hoover_enabled;
        };

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

        public Line line;
        public Microsoft.DirectX.Direct3D.Font font, font_small, font_medium;
        private DXGui gui = new DXGui();
        public int dialog_sel = 0;
        GUIItem item;

        private Drawer2D drawer;
        private CustomSprite velocimeter, arrowVelocimeter, barOfLife;

        

        public override void Init()
        {
            drawer = new Drawer2D();
            velocimeter = new CustomSprite();
            velocimeter.Bitmap = new CustomBitmap(MediaDir + "GUI\\HUB\\Velocimetro\\VelocimetroSinFlecha.png", D3DDevice.Instance.Device);
            velocimeter.Position = new TGCVector2(D3DDevice.Instance.Width * 0.84f, D3DDevice.Instance.Height * 0.70f);
            velocimeter.Scaling = new TGCVector2(0.2f, 0.2f);

            arrowVelocimeter = new CustomSprite();
            arrowVelocimeter.Bitmap = new CustomBitmap(MediaDir + "GUI\\HUB\\Velocimetro\\Flecha.png", D3DDevice.Instance.Device);
            arrowVelocimeter.Position = new TGCVector2(D3DDevice.Instance.Width * 0.84f, D3DDevice.Instance.Height * 0.85f);
            arrowVelocimeter.Scaling = new TGCVector2(0.2f, 0.2f);
            //flechaVelocimetro.TransformationMatrix = TGCMatrix.Transformation2D(new TGCVector2(0,0), 0, new TGCVector2(0.2f, 0.2f), new TGCVector2(0,0), FastMath.PI + FastMath.PI_HALF, new TGCVector2(D3DDevice.Instance.Width * 0.84f, D3DDevice.Instance.Height * 0.85f));
            //flechaVelocimetro.RotationCenter = new TGCVector2(D3DDevice.Instance.Width * 0.84f + flechaVelocimetro.Bitmap.Width/2, D3DDevice.Instance.Height * 0.85f + flechaVelocimetro.Bitmap.Height/2);
            //flechaVelocimetro.Rotation = FastMath.PI + FastMath.QUARTER_PI;

            //barOfLife = new CustomSprite();
            //barOfLife.Bitmap = new CustomBitmap(MediaDir + "GUI\\HUB\\Velocimetro\\BarraDeVida.png", D3DDevice.Instance.Device);
            //barOfLife.Position = new TGCVector2(D3DDevice.Instance.Width * 0.84f, D3DDevice.Instance.Height * 0.85f);
            //barOfLife.Scaling = new TGCVector2(0.2f, 0.2f);

            //item = gui.InsertImage("HUB\\Velocimetro\\VelocimetroSinFlecha.png", 50, 50, MediaDir);     

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

            /*
            Device d3dDevice = D3DDevice.Instance.Device;
            d3dDevice.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.FromArgb(35,56,68), 1.0f, 0);
            gui_render(ElapsedTime);
            */

            //Iniciar dibujado de todos los Sprites de la escena (en este caso es solo uno)
            drawer.BeginDrawSprite();

            //Dibujar sprite (si hubiese mas, deberian ir todos aqu�)
            drawer.DrawSprite(velocimeter);
            drawer.DrawSprite(arrowVelocimeter);

            //Finalizar el dibujado de Sprites
            drawer.EndDrawSprite();
            this.PostRender();
        }

        public void gui_render(float elapsedTime)
        {
            // ------------------------------------------------
            GuiMessage msg = gui.Update(elapsedTime, Input);
            // proceso el msg
            switch (msg.message)
            {
                case MessageType.WM_COMMAND:
                    switch (msg.id)
                    {
                        case 0:
                        case 1:
                            // Resultados OK, y CANCEL del ultimo messagebox
                            gui.EndDialog();

                            if (dialog_sel == 1)
                            {
                                // Estaba en el dialogo de configurar, paso al dialogo principal
                                dialog_sel = 0;
                            }
                            break;

                        case 100:
                            // Abro un nuevo dialogo
                            Configurar();
                            break;

                        default:
                            break;
                    }
                    break;

                default:
                    break;
            }
            gui.Render();
        }

        public void Configurar()
        {
            gui.InitDialog(false, false);
            int W = D3DDevice.Instance.Width;
            int H = D3DDevice.Instance.Height;
            int x0 = 50;
            int y0 = 50;
            int dy = H - 100;
            int dx = 320;
            dialog_sel = 1;
            /*
            gui.InsertItem(new static_text(gui, "WARRIOR", x0, y0, 400, 25));
            y0 += 45;
            gui.InsertItem(new menu_item2(gui, "WARRIOR 1", "transformers//w1.png", 103, x0, y0, MediaDir, dx, 70));
            y0 += 75;
            gui.InsertItem(new menu_item2(gui, "WARRIOR 2", "transformers//w2.png", ID_WARRIOR, x0, y0, MediaDir, dx, 70));
            y0 += 75;
            gui.InsertItem(new menu_item2(gui, "WARRIOR 3", "transformers//w3.png", ID_WARRIOR, x0, y0, MediaDir, dx, 70));
            y0 += 75;
            gui.InsertItem(new menu_item2(gui, "WARRIOR 4", "transformers//w4.png", ID_WARRIOR, x0, y0, MediaDir, dx, 70));
            y0 += 95;
            */
            gui_circle_button button = gui.InsertCircleButton(0, "SELECT", "ok.png", x0 + 70, y0, MediaDir, 40);
            button.texto_derecha = true;
            button = gui.InsertCircleButton(1, "BACK", "cancel.png", x0 + 300, y0, MediaDir, 40);
            button.texto_derecha = true;
        }

        public override void Dispose()
        {
            //gui.Dispose();
            //Cursor.Show();
            Scene.GetInstance().Dispose();
            this.auto.Dispose();
            velocimeter.Dispose();
            arrowVelocimeter.Dispose();
        }

    }
}