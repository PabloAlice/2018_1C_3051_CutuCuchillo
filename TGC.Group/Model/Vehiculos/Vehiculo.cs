using BulletSharp;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Group.Model.Vehiculos;

namespace TGC.Group.Model
{
    abstract class Vehiculo
    {
        const int rightIndex = 0;
        const int upIndex = 1;
        const int forwardIndex = 2;
        TGCVector3 wheelDirectionCS0 = new TGCVector3(0, -1, 0);
        TGCVector3 wheelAxleCS = new TGCVector3(-1, 0, 0);

        const int maxProxies = 32766;
        const int maxOverlap = 65535;

        // btRaycastVehicle is the interface for the constraint that implements the raycast vehicle
        // notice that for higher-quality slow-moving vehicles, another approach might be better
        // implementing explicit hinged-wheel constraints with cylinder collision, rather then raycasts
        float gEngineForce = 0.0f;
        float gBreakingForce = 0.0f;

        const float maxEngineForce = 1500.0f;//this should be engine/velocity dependent
        const float maxBreakingForce = 100.0f;

        float gVehicleSteering = 0.0f;
        const float steeringIncrement = 1.0f;
        const float steeringClamp = 0.3f;
        public const float wheelRadius = 0.7f;
        public const float wheelWidth = 0.4f;
        const float wheelFriction = 1000;//BT_LARGE_FLOAT;
        const float suspensionStiffness = 20.0f;
        const float suspensionDamping = 2.3f;
        const float suspensionCompression = 4.4f;
        const float rollInfluence = 0.1f;//1.0f;

        const float suspensionRestLength = 0.6f;
        const float CUBE_HALF_EXTENTS = 1;

        public TgcMesh mesh;
        private Timer deltaTiempoAvance;
        private Timer deltaTiempoSalto;
        public TGCVector3 vectorAdelante { get; set;}
        protected Ruedas ruedas;
        
        private float elapsedTime;
        protected VehicleTuning vehicleTuning;
        protected RaycastVehicle vehicle;
        protected RigidBody chassis;

        public Vehiculo(string mediaDir, TGCVector3 posicionInicial)
        {
            this.vectorAdelante = new TGCVector3(0, 0, 1);
            this.crearMesh(mediaDir + "meshCreator\\meshes\\Vehiculos\\Camioneta\\Camioneta-TgcScene.xml", posicionInicial);
            this.elapsedTime = 0f;
            this.deltaTiempoAvance = new Timer();
            this.deltaTiempoSalto = new Timer();
        }
        public Vehiculo(string mediaDir, TGCVector3 posicionInicial, DynamicsWorld world, float mass, float size, VehicleTuning vehicleTuning)
        {
            this.elapsedTime = 0f;
            this.deltaTiempoAvance = new Timer();
            this.deltaTiempoSalto = new Timer();
            this.crearMesh(mediaDir + "meshCreator\\meshes\\Vehiculos\\Camioneta\\Camioneta-TgcScene.xml", posicionInicial);

            this.chassis = RigidBodies.CreateVehicleRigidBody(mass, size, posicionInicial);
            world.AddRigidBody(this.chassis);
            this.chassis.ActivationState = ActivationState.DisableDeactivation;
            this.mesh.Transform = new TGCMatrix(this.chassis.InterpolationWorldTransform);
            this.vehicleTuning = vehicleTuning;
            this.vehicle = new RaycastVehicle(vehicleTuning, chassis, new DefaultVehicleRaycaster(world));
            world.AddAction(this.vehicle);
            world.AddRigidBody(this.chassis);
            this.SetWheels();
        }

        public void VehicleOnUpdate()
        {
            gEngineForce *= (1.0f - this.elapsedTime);

            vehicle.ApplyEngineForce(gEngineForce, 2);
            vehicle.SetBrake(gBreakingForce, 2);
            vehicle.ApplyEngineForce(gEngineForce, 3);
            vehicle.SetBrake(gBreakingForce, 3);

            vehicle.SetSteeringValue(gVehicleSteering, 0);
            vehicle.SetSteeringValue(gVehicleSteering, 1);
        }

        private void SetWheels()
        {
            const float connectionHeight = 1.2f;
            bool isFrontWheel = true;

            // choose coordinate system
            this.vehicle.SetCoordinateSystem(rightIndex, upIndex, forwardIndex);

            TGCVector3 connectionPointCS0 = new TGCVector3(CUBE_HALF_EXTENTS - (0.3f * wheelWidth), connectionHeight, 2 * CUBE_HALF_EXTENTS - wheelRadius);
            vehicle.AddWheel(connectionPointCS0.ToBsVector, wheelDirectionCS0.ToBsVector, wheelAxleCS.ToBsVector, suspensionRestLength, wheelRadius, this.vehicleTuning, isFrontWheel);

            connectionPointCS0 = new TGCVector3(-CUBE_HALF_EXTENTS + (0.3f * wheelWidth), connectionHeight, 2 * CUBE_HALF_EXTENTS - wheelRadius);
            vehicle.AddWheel(connectionPointCS0.ToBsVector, wheelDirectionCS0.ToBsVector, wheelAxleCS.ToBsVector, suspensionRestLength, wheelRadius, this.vehicleTuning, isFrontWheel);

            isFrontWheel = false;
            connectionPointCS0 = new TGCVector3(-CUBE_HALF_EXTENTS + (0.3f * wheelWidth), connectionHeight, -2 * CUBE_HALF_EXTENTS + wheelRadius);
            vehicle.AddWheel(connectionPointCS0.ToBsVector, wheelDirectionCS0.ToBsVector, wheelAxleCS.ToBsVector, suspensionRestLength, wheelRadius, this.vehicleTuning, isFrontWheel);

            connectionPointCS0 = new TGCVector3(CUBE_HALF_EXTENTS - (0.3f * wheelWidth), connectionHeight, -2 * CUBE_HALF_EXTENTS + wheelRadius);
            vehicle.AddWheel(connectionPointCS0.ToBsVector, wheelDirectionCS0.ToBsVector, wheelAxleCS.ToBsVector, suspensionRestLength, wheelRadius, this.vehicleTuning, isFrontWheel);


            for (var i = 0; i < vehicle.NumWheels; i++)
            {
                WheelInfo wheel = vehicle.GetWheelInfo(i);
                wheel.SuspensionStiffness = suspensionStiffness;
                wheel.WheelsDampingRelaxation = suspensionDamping;
                wheel.WheelsDampingCompression = suspensionCompression;
                wheel.FrictionSlip = wheelFriction;
                wheel.RollInfluence = rollInfluence;
            }
        }

        private void crearMesh(string rutaAMesh, TGCVector3 posicionInicial)
        {
            TgcSceneLoader loader = new TgcSceneLoader();
            TgcScene scene = loader.loadSceneFromFile(rutaAMesh);
            this.mesh = scene.Meshes[0];
            this.mesh.RotateY(FastMath.PI);
            this.mesh.Scale = new TGCVector3(0.05f, 0.05f, 0.05f);
            this.mesh.Position = posicionInicial;
        }

        public void onBack()
        {
            gEngineForce = -maxEngineForce;
        }

        public void onBreak()
        {
            gBreakingForce = maxBreakingForce;
        }

        public void onBreakRelease()
        {
            gBreakingForce = 0;
        }

        public void onLeft()
        {
            gVehicleSteering -= this.elapsedTime * steeringIncrement;
            if (gVehicleSteering < -steeringClamp)
                gVehicleSteering = -steeringClamp;
        }
        public void onRight()
        {
            gVehicleSteering += this.elapsedTime * steeringIncrement;
            if (gVehicleSteering > steeringClamp)
                gVehicleSteering = steeringClamp;
        }
        public void onForward()
        {
            gEngineForce = maxEngineForce;
        }

        public void setElapsedTime(float time)
        {
            this.elapsedTime = time;
            if(this.deltaTiempoAvance.tiempoTranscurrido() != 0)
            {
                this.deltaTiempoAvance.acumularTiempo(this.elapsedTime);
            }
            if (this.deltaTiempoSalto.tiempoTranscurrido() != 0)
            {
                this.deltaTiempoSalto.acumularTiempo(this.elapsedTime);
            }

        }

        public void Render()
        {
            this.mesh.Render();
            this.ruedas.Render();
        }

        public void dispose()
        {
            this.mesh.Dispose();
        }

        public Timer getDeltaTiempoAvance()
        {
            return this.deltaTiempoAvance;
        }

        public float getElapsedTime()
        {
            return this.elapsedTime;
        }

        public Timer getDeltaTiempoSalto()
        {
            return this.deltaTiempoSalto;
        }

        public Ruedas GetRuedas()
        {
            return this.ruedas;
        }
    }
}
