using TGC.Core.SceneLoader;
using TGC.Core.Mathematica;
using TGC.Core.BoundingVolumes;
using TGC.Group.Model.Vehiculos;
using TGC.Core.Collision;
using TGC.Core.Particle;
using TGC.Core.Shaders;

namespace TGC.Group.Model
{
    abstract class Weapon : Collidable
    {
        public TgcMesh mesh;
        public TgcBoundingSphere sphere;
        public WeaponState weaponState;
        public TGCVector3 direction;
        public TransformationMatrix initialTransformation = new TransformationMatrix();
        public TransformationMatrix matrix = new TransformationMatrix();
        public SoundsManager soundManager;
        public ParticleTimer particle;
        private float time = 0f;
        protected float powerOfDamage;

        public Weapon(TransformationMatrix matrix, TgcMesh mesh)
        {
            this.initialTransformation = matrix;
            this.matrix = matrix;
            this.mesh = mesh;
            this.mesh.AutoTransform = false;
            TGCMatrix m = this.matrix.GetTransformation();
            this.mesh.Transform = m;
            this.mesh.BoundingBox.transform(m);
            this.sphere = TgcBoundingSphere.computeFromPoints(this.mesh.BoundingBox.computeCorners()).toClass();
            this.sphere.setValues(this.sphere.Center, 0.3f);
            this.weaponState = new InExhibition(this);
            this.soundManager = new SoundsManager();
            this.CreateParticle();
            this.InitializeEffect();
            
        }

        public void Init() { }

        public void HandleCollision(ThirdPersonCamera camera)
        {
            return;
        }

        private void InitializeEffect()
        {
            mesh.Effect = TgcShaders.loadEffect(GlobalConcepts.GetInstance().GetShadersDir() + "Arma.fx");
            mesh.Technique = "Exhibicion";
        }

        public TGCPlane GetPlaneOfCollision(TgcRay ray, Vehicle car)
        {
            return TGCPlane.FromPointNormal(this.GetPosition(), TGCVector3.Up);
        }

        public void SetTexture(float u, float v)
        {
            return;
        }

        public bool IsInto(TGCVector3 minPoint, TGCVector3 maxPoint)
        {
            return GlobalConcepts.GetInstance().IsBetweenXZ(this.GetPosition(), minPoint, maxPoint);
        }

        abstract protected void CreateParticle();

        public TransformationMatrix ReturnSame(TransformationMatrix m)
        {
            TransformationMatrix theReturn = new TransformationMatrix();
            theReturn.SetTranslation(m.GetTranslation() * TGCMatrix.Translation(new TGCVector3(0,0,0)));
            theReturn.SetScalation(m.GetScalation() * TGCMatrix.Scaling(new TGCVector3(1,1,1)));
            theReturn.SetRotation(m.GetRotation() * TGCMatrix.RotationYawPitchRoll(0, 0, 0));
            return theReturn;
        }

        abstract public TGCVector3 GetShootRotation();

        public bool IsColliding(Weapon weapon)
        {
            return false;
        }

        public bool IsColliding(Vehicle car)
        {
            return TgcCollisionUtils.testSphereOBB(this.sphere, car.GetTGCBoundingOrientedBox());
        }

        public void Update()
        {
            this.weaponState.Update();
        }


        abstract public void Shoot();

        public TGCVector3 GetPosition()
        {
            return TGCVector3.transform(new TGCVector3(0,0,0), this.matrix.GetTransformation());
        }

        public TgcMesh GetCollidable(Vehicle car)
        {
            return this.weaponState.GetCollidable(car);
        }

        public void Transform()
        {
            TGCMatrix m = this.matrix.GetTransformation();
            this.mesh.Transform = m;
            this.sphere.setCenter(TGCVector3.transform(new TGCVector3(0,0,0), m));

        }

        public void HandleCollision(Vehicle car)
        {
            this.weaponState.HandleCollision(car);
        }

        public void HandleCollision(Weapon weapon)
        {
            return;
        }

        public void Shoot(Vehicle car)
        {
            this.weaponState.Shoot(car);
        }

        public void Render()
        {
            time += GlobalConcepts.GetInstance().GetElapsedTime();
            mesh.Effect.SetValue("time", time);
            if (time > 100f) time = 0f;
            this.particle.Render(GlobalConcepts.GetInstance().GetElapsedTime());
            if (this.IsInView(this.mesh))
            {
                this.weaponState.Render();
            }   
        }

        public void RenderShadows()
        {
            if (this.IsInView(this.mesh))
            {
                Lighting.LightManager.GetInstance().RenderMyShadow(this.mesh);
            }
        }

        private bool IsInView(TgcMesh mesh)
        {
            this.Transform();
            return (int)TgcCollisionUtils.classifyFrustumSphere(GlobalConcepts.GetInstance().GetFrustum(), this.sphere) != 0;
        }

        public void Dispose()
        {
            this.weaponState.Dispose();
        }

        abstract public void Move();

        abstract public void Collide(Collidable collided);

        virtual public void IAmTheCar(Vehicle car)
        {
            car.TakeDamage(powerOfDamage);
            return;
        }

        public override string ToString()
        {
            return this.GetType().ToString();
        }

    }
}
