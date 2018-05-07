using Microsoft.DirectX.DirectInput;
using System.Drawing;
using System;
using TGC.Core.Direct3D;
using TGC.Core.Example;
using TGC.Core.Geometry;
using TGC.Core.Input;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Core.Textures;
using TGC.Group.Model;
using TGC.Group.Model.Vehiculos;
using TGC.Core.Text;

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

        private Camioneta auto;
        private CamaraEnTerceraPersona camaraInterna;
        private TGCVector3 camaraDesplazamiento = new TGCVector3(0,5,40);
        private TGCBox cubo;
        private TgcScene scene;
        private TgcText2D textoVelocidadVehiculo, textoAlturaVehiculo, textoPosX, textoPosZ;
        private TgcMesh jabon, mLibroAnalisis, mLibroOperativos, mMoneda, mPelota, mRegla;

        public override void Init()
        {

            //en caso de querer cargar una escena
            TgcSceneLoader loader = new TgcSceneLoader();
            this.scene = loader.loadSceneFromFile(MediaDir + "MeshCreator\\Scenes\\Habitacion\\habitacion-TgcScene.xml");
            foreach (var mesh in this.scene.Meshes)
            {
                mesh.Scale = new TGCVector3(12f, 12f, 12f);
            }

            initMeshes();

            
            //creo el vehiculo liviano
            //si quiero crear un vehiculo pesado (camion) hago esto
            // VehiculoPesado camion = new VehiculoPesado(rutaAMesh);
            // se hace esta distinción de vehiculo liviano y pesado por que cada uno tiene diferentes velocidades,
            // peso, salto, etc.
            this.auto = new Camioneta(MediaDir, new TGCVector3(0f, 0f, 0f));
            this.auto.mesh.AutoTransform = false;
            //creo un cubo para tomarlo de referencia (para ver como se mueve el auto)
            this.cubo = TGCBox.fromSize(new TGCVector3(-50, 10, -20), new TGCVector3(15, 15, 15), Color.Black);

            //creo la camara en tercera persona (la clase CamaraEnTerceraPersona hereda de la clase real del framework
            //que te permite configurar la posicion, el lookat, etc. Lo que hacemos al heredar, es reescribir algunos
            //metodos y setear valores default para que la camara quede mirando al auto en 3era persona

            this.camaraInterna = new CamaraEnTerceraPersona(auto.posicion() + camaraDesplazamiento, 7.5f, -55);
            this.Camara = camaraInterna;

        }

        public override void Update()
        {
            this.PreUpdate();
           
            if (Input.keyDown(Key.NumPad4))
            {
                this.camaraInterna.rotateY(-0.005f);
            }
            if (Input.keyDown(Key.NumPad6))
            {
                this.camaraInterna.rotateY(0.005f);
            }
            if (Input.keyDown(Key.RightArrow))
            {
                this.camaraInterna.OffsetHeight += 0.05f;
            }
            if (Input.keyDown(Key.LeftArrow))
            {
                this.camaraInterna.OffsetHeight -= 0.05f;
            }

            if (Input.keyDown(Key.UpArrow))
            {
                this.camaraInterna.OffsetForward += 0.05f;
            }
            if (Input.keyDown(Key.DownArrow))
            {
                this.camaraInterna.OffsetForward -= 0.05f;
            }

            this.textoVelocidadVehiculo = new TgcText2D();
            string dialogo = "Velocidad = {0}km";
            this.textoVelocidadVehiculo.Text = string.Format(dialogo, auto.getVelocidadActual());
            //text3.Align = TgcText2D.TextAlign.RIGHT;
            this.textoVelocidadVehiculo.Position = new Point(55, 15);
            this.textoVelocidadVehiculo.Size = new Size(0, 0);
            this.textoVelocidadVehiculo.Color = Color.Gold;

            this.textoAlturaVehiculo = new TgcText2D();
            string dialogo2 = "Velocidad salto = {0}";
            this.textoAlturaVehiculo.Text = string.Format(dialogo2, auto.getVelocidadActualDeSalto());
            //text3.Align = TgcText2D.TextAlign.RIGHT;
            this.textoAlturaVehiculo.Position = new Point(55, 25);
            this.textoAlturaVehiculo.Size = new Size(0, 0);
            this.textoAlturaVehiculo.Color = Color.Gold;

            this.textoPosX = new TgcText2D();
            dialogo = "Posicion X = {0}";
            this.textoPosX.Text = string.Format(dialogo, auto.posicion().X);
            this.textoPosX.Position = new Point(70, 50);
            this.textoPosX.Size = new Size(0, 0);
            this.textoPosX.Color = Color.Gold;

            this.textoPosZ = new TgcText2D();
            dialogo = "Posicion Z = {0}";
            this.textoPosZ.Text = string.Format(dialogo, auto.posicion().Z.ToString());
            this.textoPosZ.Position = new Point(70, 80);
            this.textoPosZ.Size = new Size(0, 0);
            this.textoPosZ.Color = Color.Gold;


            this.auto.setElapsedTime(ElapsedTime);

            //si el usuario teclea la W y ademas no tecla la D o la A
            if (Input.keyDown(Key.W))
            {
                //hago avanzar al auto hacia adelante. Le paso el Elapsed Time que se utiliza para
                //multiplicarlo a la velocidad del auto y no depender del hardware del computador
                this.auto.getEstado().advance();

            }

            //lo mismo que para avanzar pero para retroceder
            if (Input.keyDown(Key.S))
            {
                this.auto.getEstado().back();
            }

            //si el usuario teclea D
            if (Input.keyDown(Key.D))
            {
                this.auto.getEstado().right(camaraInterna);
                
            }else if (Input.keyDown(Key.A))
            {
                this.auto.getEstado().left(camaraInterna);
            }

            //Si apreta espacio, salta
            if (Input.keyDown(Key.Space))
            {
                this.auto.getEstado().jump();
            }

            if (!Input.keyDown(Key.W) && !Input.keyDown(Key.S))
            {
                this.auto.getEstado().speedUpdate();
            }

            //esto es algo turbio que tengo que hacer, por que sino es imposible modelar el salto
            this.auto.getEstado().jumpUpdate();


            //Hacer que la camara siga al personaje en su nueva posicion
            this.camaraInterna.Target = (TGCVector3.transform(auto.posicion(), auto.transformacion)) + auto.getVectorAdelante() * 30 ;

            this.PostUpdate();
        }

        public override void Render()
        {

            this.PreRender();

            renderMeshes();

            this.textoVelocidadVehiculo.render();
            this.textoAlturaVehiculo.render();
            this.textoPosX.render();
            this.textoPosZ.render();

            this.scene.RenderAll();
            
            this.auto.Transform();
            this.auto.Render();

            this.cubo.Transform =
                TGCMatrix.Scaling(cubo.Scale)
                            * TGCMatrix.RotationYawPitchRoll(cubo.Rotation.Y, cubo.Rotation.X, cubo.Rotation.Z)
                            * TGCMatrix.Translation(cubo.Position);
            this.cubo.Render();
            //this.jabon.Render();
            this.PostRender();
        }

        public override void Dispose()
        {

            disposeMeshes();

            //Dispose del auto.
            this.auto.dispose();

            //Dispose del cubo
            this.cubo.Dispose();
            //Dispose Scene
            this.scene.DisposeAll();
            //Dispose TextoVelocidadVehiculo
            this.textoAlturaVehiculo.Dispose();
            //Dispose TextoAlturaVehiculo
            this.textoAlturaVehiculo.Dispose();

            this.textoPosX.Dispose();
            this.textoPosZ.Dispose();
        }

        private void initMeshes()
        {
            this.jabon = new TgcSceneLoader().loadSceneFromFile(MediaDir + "MeshCreator\\Meshes\\Bathroom\\Jabon\\Jabon-TgcScene.xml").Meshes[0];
            mLibroAnalisis = new TgcSceneLoader().loadSceneFromFile(MediaDir + "MeshCreator\\Meshes\\Habitacion\\Libros\\Analisis\\Analisis-TgcScene.xml").Meshes[0];
            mLibroOperativos = new TgcSceneLoader().loadSceneFromFile(MediaDir + "MeshCreator\\Meshes\\Habitacion\\Libros\\Operativos\\Operativos-TgcScene.xml").Meshes[0];
            mMoneda = new TgcSceneLoader().loadSceneFromFile(MediaDir + "MeshCreator\\Meshes\\Habitacion\\Moneda\\Moneda-TgcScene.xml").Meshes[0];
            mPelota = new TgcSceneLoader().loadSceneFromFile(MediaDir + "MeshCreator\\Meshes\\Habitacion\\Pelota\\Pelota-TgcScene.xml").Meshes[0];
            //mRegla = new TgcSceneLoader().loadSceneFromFile(MediaDir + "MeshCreator\\Meshes\\Habitacion\\Regla2\\Regla2-TgcScene.xml").Meshes[0];  

        }

        private void renderMeshes()
        {
            Random rnd = new Random();

            jabon.Position = new TGCVector3(-1400f, 0f, 1820f);
            jabon.Rotation = new TGCVector3(0f, 0f, 0f);
            jabon.Render();
            jabon.RotateX(-FastMath.QUARTER_PI * 2 / 3);
            jabon.Move(0f, 7f, -20f);
            jabon.Render();

            mLibroAnalisis.Position = new TGCVector3(0f, 0f, 500f);
            mLibroAnalisis.Rotation = new TGCVector3(0f, 0f, 0f);
            mLibroAnalisis.Scale = new TGCVector3(1.2f, 1.2f, 1.2f);
            mLibroAnalisis.RotateY(FastMath.PI_HALF);
            mLibroAnalisis.RotateX(-FastMath.PI_HALF * 7 / 9);
            mLibroAnalisis.Move(275f, 32f, -370f);
            mLibroAnalisis.Render();

            mLibroOperativos.Position = new TGCVector3(0f, 0f, 500f);
            mLibroOperativos.Rotation = new TGCVector3(0f, 0f, 0f);
            mLibroOperativos.Scale = new TGCVector3(1.2f, 1.2f, 1.2f);
            mLibroOperativos.RotateY(FastMath.PI_HALF);
            mLibroOperativos.RotateX(-FastMath.PI_HALF * 7 / 9);
            mLibroOperativos.Move(265f, 32f, -320f);
            mLibroOperativos.Render();

            mMoneda.Position = new TGCVector3(-1500f, 0f, -500);
            mMoneda.Rotation = new TGCVector3(0f, 0f, 0f);
            mMoneda.Render();
            mMoneda.Move(5f, 1f, 5f);
            mMoneda.Render();
            mMoneda.Move(-7F, 1f, 0f);
            mMoneda.RotateZ(0.2f);
            mMoneda.Render();

            mMoneda.Position = new TGCVector3(1000f, 0f, -750);
            mMoneda.Rotation = new TGCVector3(0f, 0f, 0f);
            mMoneda.Render();
            mMoneda.Move(5f, 1f, 5f);
            mMoneda.Render();
            mMoneda.Move(-7F, 1f, 0f);
            mMoneda.RotateZ(0.2f);
            mMoneda.Render();

            mMoneda.Position = new TGCVector3(200f, 0f, 200f);
            mMoneda.Rotation = new TGCVector3(0f, 0f, 0f);
            mMoneda.Render();
            mMoneda.Move(5f, 1f, 5f);
            mMoneda.Render();
            mMoneda.Move(-7F, 1f, 0f);
            mMoneda.RotateZ(0.2f);
            mMoneda.Render();

            mPelota.Position = new TGCVector3(-1000f, 0f, 0f);
            mPelota.Scale = new TGCVector3(4f, 4f, 4f);
            mPelota.Render();

            //mRegla.Position = new TGCVector3(0f, 0f, 0f);
           // mRegla.Render();


        }

        private void disposeMeshes()
        {
            jabon.Dispose();
            mLibroAnalisis.Dispose();
            mLibroOperativos.Dispose();
            mMoneda.Dispose();
            mPelota.Dispose();
        }
    }
}