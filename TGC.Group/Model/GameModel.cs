using BulletSharp;
using Microsoft.DirectX.DirectInput;
using System.Drawing;
using TGC.Core.Example;
using TGC.Core.Geometry;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
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
        private DiscreteDynamicsWorld dynamicsWorld;
        private CollisionDispatcher dispatcher;
        private DefaultCollisionConfiguration collisionConfiguration;
        private SequentialImpulseConstraintSolver constraintSolver;
        private BroadphaseInterface overlappingPairCache;
        private Camioneta auto;
        private CamaraEnTerceraPersona camaraInterna;
        private TGCVector3 camaraDesplazamiento = new TGCVector3(0,5,40);
        private TGCBox cubo;
        private TgcScene scene;
        private TgcText2D textoVelocidadVehiculo, textoAlturaVehiculo;
        private TgcMesh jabon;
        private RigidBody ballBody;
        public override void Init()
        {
            this.SetPhysicWorld();

            //en caso de querer cargar una escena
            TgcSceneLoader loader = new TgcSceneLoader();
            this.scene = loader.loadSceneFromFile(MediaDir + "Texturas\\Habitacion\\escenaFinal-TgcScene.xml");
            foreach (var mesh in this.scene.Meshes)
            {
                mesh.Scale = new TGCVector3(12f, 12f, 12f);
            }

            this.jabon = new TgcSceneLoader().loadSceneFromFile(MediaDir + "MeshCreator\\Meshes\\Bathroom\\Jabon\\Jabon-TgcScene.xml").Meshes[0];

            //creo el vehiculo liviano
            //si quiero crear un vehiculo pesado (camion) hago esto
            // VehiculoPesado camion = new VehiculoPesado(rutaAMesh);
            // se hace esta distinción de vehiculo liviano y pesado por que cada uno tiene diferentes velocidades,
            // peso, salto, etc.
            this.auto = new Camioneta(MediaDir, new TGCVector3 (0f, 100f, 0f), this.dynamicsWorld, 800, 1f, new VehicleTuning());

            //creo un cubo para tomarlo de referencia (para ver como se mueve el auto)
            this.cubo = TGCBox.fromSize(new TGCVector3(-50, 10, -20), new TGCVector3(15, 15, 15), Color.Black);

            //creo la camara en tercera persona (la clase CamaraEnTerceraPersona hereda de la clase real del framework
            //que te permite configurar la posicion, el lookat, etc. Lo que hacemos al heredar, es reescribir algunos
            //metodos y setear valores default para que la camara quede mirando al auto en 3era persona

            this.camaraInterna = new CamaraEnTerceraPersona(new TGCVector3(auto.vehicle.RigidBody.CenterOfMassPosition) + camaraDesplazamiento, 7.5f, -55);
            this.Camara = camaraInterna;

            var floorShape = new StaticPlaneShape(TGCVector3.Up.ToBsVector, 0);
            var floorMotionState = new DefaultMotionState();
            var floorInfo = new RigidBodyConstructionInfo(0, floorMotionState, floorShape);
            var floorBody = new RigidBody(floorInfo);
            floorBody.Friction = 1;
            floorBody.RollingFriction = 1;
            // ballBody.SetDamping(0.1f, 0.9f);
            floorBody.Restitution = 1f;
            floorBody.UserObject = "floorBody";
            dynamicsWorld.AddRigidBody(floorBody);
        }

        public RigidBody CreateBox(float size, float mass, float x, float y, float z, float yaw, float pitch, float roll)
        {
            //Se crea una caja de tamaño 20 con rotaciones y origien en 10,100,10 y 1kg de masa.
            var boxShape = new BoxShape(size, size, size);
            var boxTransform = TGCMatrix.RotationYawPitchRoll(yaw, pitch, roll).ToBsMatrix;
            boxTransform.Origin = new TGCVector3(x, y, z).ToBsVector;
            DefaultMotionState boxMotionState = new DefaultMotionState(boxTransform);
            //Es importante calcular la inercia caso contrario el objeto no rotara.
            var boxLocalInertia = boxShape.CalculateLocalInertia(mass);
            var boxInfo = new RigidBodyConstructionInfo(1f, boxMotionState, boxShape, boxLocalInertia);
            var boxBody = new RigidBody(boxInfo);
            boxBody.LinearFactor = TGCVector3.One.ToBsVector;
            //boxBody.SetDamping(0.7f, 0.9f);
            //boxBody.Restitution = 1f;
            return boxBody;
        }
 

         private void SetPhysicWorld()
        {
            //Creamos el mundo fisico por defecto.
            collisionConfiguration = new DefaultCollisionConfiguration();
            dispatcher = new CollisionDispatcher(collisionConfiguration);
            GImpactCollisionAlgorithm.RegisterAlgorithm(dispatcher);
            constraintSolver = new SequentialImpulseConstraintSolver();
            overlappingPairCache = new DbvtBroadphase(); //AxisSweep3(new BsVector3(-5000f, -5000f, -5000f), new BsVector3(5000f, 5000f, 5000f), 8192);
            dynamicsWorld = new DiscreteDynamicsWorld(dispatcher, overlappingPairCache, constraintSolver, collisionConfiguration);
            dynamicsWorld.Gravity = new TGCVector3(0, -10f, 0).ToBsVector;
        }

        public override void Update()
        {
            this.PreUpdate();
            dynamicsWorld.StepSimulation(1 / 60f, 10);
            this.auto.setElapsedTime(ElapsedTime);

            this.textoVelocidadVehiculo = new TgcText2D();
            string dialogo = "Velocidad = {0}km";
            this.textoVelocidadVehiculo.Text = string.Format(dialogo, this.auto.gEngineForce);
            //text3.Align = TgcText2D.TextAlign.RIGHT;
            this.textoVelocidadVehiculo.Position = new Point(55, 15);
            this.textoVelocidadVehiculo.Size = new Size(0, 0);
            this.textoVelocidadVehiculo.Color = Color.Gold;

            this.textoAlturaVehiculo = new TgcText2D();
            string dialogo2 = "Velocidad salto = {0}";
            this.textoAlturaVehiculo.Text = string.Format(dialogo2, 0);
            //text3.Align = TgcText2D.TextAlign.RIGHT;
            this.textoAlturaVehiculo.Position = new Point(55, 25);
            this.textoAlturaVehiculo.Size = new Size(0, 0);
            this.textoAlturaVehiculo.Color = Color.Gold;

            //si el usuario teclea la W y ademas no tecla la D o la A
            if (Input.keyDown(Key.W))
            {
               // this.ballBody = this.CreateBox(10f, 0.1f, 0, 110, 0, 0, 0, 0);
                //ballBody.SetDamping(0.1f, 0.1f);
                //ballBody.Restitution = 0.9f;
                //dynamicsWorld.AddRigidBody(ballBody);
                //hago avanzar al auto hacia adelante. Le paso el Elapsed Time que se utiliza para
                //multiplicarlo a la velocidad del auto y no depender del hardware del computador
                this.auto.onForward();
            }

            //lo mismo que para avanzar pero para retroceder
            if (Input.keyDown(Key.S))
            {
                this.auto.onBack();
            }

            //si el usuario teclea D
            if (Input.keyDown(Key.D))
            {
                this.auto.onRight();
                
            }else if (Input.keyDown(Key.A))
            {
                this.auto.onLeft();
            }

            //Si apreta espacio, salta
            if (Input.keyDown(Key.Space))
            {
                this.auto.onBreak();
            }
            if (Input.keyDown(Key.NumPad4))
            {
                this.camaraInterna.rotateY(-0.05f);
            }
            if (Input.keyDown(Key.NumPad6))
            {
                this.camaraInterna.rotateY(0.05f);
            }
            if (Input.keyDown(Key.RightArrow))
            {
                this.camaraInterna.OffsetHeight += 0.3f;
            }
            if (Input.keyDown(Key.LeftArrow))
            {
                this.camaraInterna.OffsetHeight -= 0.3f;
            }

            if (Input.keyDown(Key.UpArrow))
            {
                this.camaraInterna.OffsetForward += 0.3f;
            }
            if (Input.keyDown(Key.DownArrow))
            {
                this.camaraInterna.OffsetForward -= 0.3f;
            }
            //Hacer que la camara siga al personaje en su nueva posicion
            this.camaraInterna.Target = new TGCVector3(auto.vehicle.RigidBody.CenterOfMassPosition) + auto.vectorAdelante * 30 ;
            this.auto.VehicleOnUpdate();
            this.PostUpdate();
        }

        public override void Render()
        {

            this.PreRender();

            this.textoVelocidadVehiculo.render();
            this.textoAlturaVehiculo.render();

            this.scene.RenderAll();

            this.auto.Render();

           
            this.cubo.Transform = new TGCMatrix(this.auto.vehicle.RigidBody.InterpolationWorldTransform);
            this.cubo.Render();
           
            //this.jabon.Render();
            this.PostRender();
        }

        public override void Dispose()
        {
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
        }
    }
}