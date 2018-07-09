using System.Collections.Generic;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Group.Model.Vehiculos.Estados;
using TGC.Group.Model.Vehiculos;
using TGC.Core.BoundingVolumes;
using TGC.Core.Input;
using Microsoft.DirectX.DirectInput;
using TGC.Core.Shaders;
using TGC.Core.Collision;
using TGC.Core.Particle;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using TGC.Core.Direct3D;
using System.Drawing;
using System.Linq;
using System;

namespace TGC.Group.Model
{
    abstract class Vehicle : Collidable
    {
        public TgcMesh mesh;
        protected BoundingOrientedBox obb;
        private Timer deltaTiempoAvance;
        private Timer deltaTiempoSalto;
        public TGCVector3 vectorAdelante;
        public TGCVector3 VectorAdelanteSalto { get; set; }
        public TransformationMatrix matrixs;
        protected List<Wheel> ruedas = new List<Wheel>();
        protected Wheel delanteraIzquierda;
        protected Wheel delanteraDerecha;
        protected TGCVector3 vectorDireccion;
        protected EstadoVehiculo estado;
        private float velocidadActual = 0f;
        private float velocidadActualDeSalto;
        protected float velocidadRotacion = 1f;
        protected float velocidadInicialDeSalto = 15f;
        protected float velocidadMaximaDeAvance = 60f;
        protected float velocidadMaximaDeRetroceso;
        protected float aceleracionAvance = 0.3f;
        protected float aceleracionRetroceso;
        private float aceleracionGravedad = 0.5f;
        private float elapsedTime = 0f;
        protected float constanteDeRozamiento = 0.2f;
        protected float constanteFrenado = 1f;
        public SoundsManager SoundsManager { get; set; }
        protected TGCVector3 escaladoInicial = new TGCVector3(0.005f, 0.005f, 0.005f);
        //se guarda el traslado inicial porque se usa como pivote
        protected TGCMatrix trasladoInicial;
        protected TGCMatrix lastTransformation;
        protected float life = 100f;
        protected ParticleEmitter smoke;
        protected ParticleTimer spark;
        protected List<Weapon> weapons = new List<Weapon>();
        protected float timeShoot = 0f;
        public Vehiculos.Light frontLights;
        public Vehiculos.Light reverseLights;
        public Vehiculos.Light breakLights;
        private PointsOfCollision pointsOfCollision;
        // FOR SHADOW MAP
        private readonly int SHADOWMAP_SIZE = 1024;
        private TGCVector3 g_LightDir; // direccion de la luz actual
        private TGCVector3 g_LightPos; // posicion de la luz actual (la que estoy analizando)
        private TGCMatrix g_LightView; // matriz de view del light
        private TGCMatrix g_mShadowProj; // Projection matrix for shadow map
        private Surface g_pDSShadow; // Depth-stencil buffer for rendering to shadow map
        private Texture g_pShadowMap; // Texture to which the shadow map is rendered
        // FOR SHADOW MAP

        //Timer shaderTime = new Timer();
        //FloatModifier shaderColorModifier = new FloatModifier(0.84f, 0.74f, 0.94f);

        public Vehicle(TGCVector3 posicionInicial, SoundsManager soundsManager)
        {
            this.matrixs = new TransformationMatrix();
            this.SoundsManager = soundsManager;
            this.vectorAdelante = new TGCVector3(0, 0, 1);
            this.velocidadActualDeSalto = this.velocidadInicialDeSalto;
            this.deltaTiempoAvance = new Timer();
            this.deltaTiempoSalto = new Timer();
            this.aceleracionRetroceso = this.aceleracionAvance * 0.8f;
            this.vectorDireccion = this.vectorAdelante;
            this.estado = new Stopped(this);
            this.velocidadMaximaDeRetroceso = this.velocidadMaximaDeAvance * 0.7f;
            g_LightPos = new TGCVector3(80, 120, 0);
            g_LightDir = TGCVector3.Empty;
            g_LightDir.Normalize();
            this.Init();
        }

        public void Init()
        {
            this.CreateSmoke();
            this.CreateSpark();
            //--------------------------------------------------------------------------------------
            // Creo el shadowmap.
            // Format.R32F
            // Format.X8R8G8B8
            g_pShadowMap = new Texture(D3DDevice.Instance.Device, SHADOWMAP_SIZE, SHADOWMAP_SIZE, 1, Usage.RenderTarget, Format.R32F, Pool.Default);

            // tengo que crear un stencilbuffer para el shadowmap manualmente
            // para asegurarme que tenga la el mismo tamano que el shadowmap, y que no tenga
            // multisample, etc etc.
            g_pDSShadow = D3DDevice.Instance.Device.CreateDepthStencilSurface(SHADOWMAP_SIZE, SHADOWMAP_SIZE, DepthFormat.D24S8, MultiSampleType.None, 0, true);
            // por ultimo necesito una matriz de proyeccion para el shadowmap, ya
            // que voy a dibujar desde el pto de vista de la luz.
            // El angulo tiene que ser mayor a 45 para que la sombra no falle en los extremos del cono de luz
            // de hecho, un valor mayor a 90 todavia es mejor, porque hasta con 90 grados es muy dificil
            // lograr que los objetos del borde generen sombras
            var aspectRatio = D3DDevice.Instance.AspectRatio;
            g_mShadowProj = TGCMatrix.PerspectiveFovLH(Geometry.DegreeToRadian(80), aspectRatio, 50, 5000);
            // D3DDevice.Instance.Device.Transform.Projection = TGCMatrix.PerspectiveFovLH(Geometry.DegreeToRadian(45.0f), aspectRatio, 2f, 1500f).ToMatrix();
        }

        public void RenderShadowMap()
        {
            // Calculo la matriz de view de la luz
            this.mesh.Effect.SetValue("g_vLightPos", new Vector4(g_LightPos.X, g_LightPos.Y, g_LightPos.Z, 1));
            this.mesh.Effect.SetValue("g_vLightDir", new Vector4(g_LightDir.X, g_LightDir.Y, g_LightDir.Z, 1));
            g_LightView = TGCMatrix.LookAtLH(g_LightPos, g_LightPos + g_LightDir, new TGCVector3(0, 0, 1));

            // inicializacion standard:
            this.mesh.Effect.SetValue("g_mProjLight", g_mShadowProj.ToMatrix());
            this.mesh.Effect.SetValue("g_mViewLightProj", (g_LightView * g_mShadowProj).ToMatrix());

            // Primero genero el shadow map, para ello dibujo desde el pto de vista de luz
            // a una textura, con el VS y PS que generan un mapa de profundidades.
            var pOldRT = D3DDevice.Instance.Device.GetRenderTarget(0);
            var pShadowSurf = g_pShadowMap.GetSurfaceLevel(0);
            D3DDevice.Instance.Device.SetRenderTarget(0, pShadowSurf);
            var pOldDS = D3DDevice.Instance.Device.DepthStencilSurface;
            D3DDevice.Instance.Device.DepthStencilSurface = g_pDSShadow;
            D3DDevice.Instance.Device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1.0f, 0);
            D3DDevice.Instance.Device.BeginScene();

            // Hago el render de la escena pp dicha
            this.mesh.Effect.SetValue("g_txShadow", g_pShadowMap);
            RenderScene(true);

            // Termino
            D3DDevice.Instance.Device.EndScene();

            //TextureLoader.Save("shadowmap.bmp", ImageFileFormat.Bmp, g_pShadowMap);

            // restuaro el render target y el stencil
            D3DDevice.Instance.Device.DepthStencilSurface = pOldDS;
            D3DDevice.Instance.Device.SetRenderTarget(0, pOldRT);
        }

        public void RenderScene(bool shadow)
        {
            // avion
            if (shadow)
            {
                this.mesh.Technique = "RenderShadow";
            }
            else
            {
                this.mesh.Technique = "RenderScene";
            }
            this.mesh.Render();
        }

        public void HandleCollision(ThirdPersonCamera camera)
        {
            return;
        }

        public void TakeDamage(float powerOfDamage)
        {
            this.life = FastMath.Max(0, life - powerOfDamage);
        }

        public void ResetScale()
        {
            matrixs.SetScalation(TGCMatrix.Scaling(escaladoInicial));
        }

        public void ResetRotation()
        {
            matrixs.SetRotation(TGCMatrix.Identity);
        }

        public bool IsColliding(Vehicle car)
        {
            return TgcCollisionUtils.testObbObb(car.GetTGCBoundingOrientedBox(), this.GetTGCBoundingOrientedBox());
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

        public bool IsInView(TgcMesh mesh)
        {
            this.Transform();
            return TgcCollisionUtils.classifyFrustumAABB(GlobalConcepts.GetInstance().GetFrustum(), mesh.BoundingBox) != 0;
        }

        protected void CreateLights(string reverseLightsPath, string breakLightsPath, string frontLightsPath)
        {
            this.reverseLights = new Vehiculos.Light(reverseLightsPath);
            this.frontLights = new Vehiculos.Light(frontLightsPath);
            this.breakLights = new Vehiculos.Light(breakLightsPath);
            this.frontLights.ActivateLight();
        }

        private void CreateSmoke()
        {
            this.smoke = new ParticleEmitter(GlobalConcepts.GetInstance().GetMediaDir() + "Texturas\\Humo\\humo.png", 10);
            this.smoke.Position = this.GetPosition();
            this.smoke.MinSizeParticle = 0.3f;
            this.smoke.MaxSizeParticle = 0.5f;
            this.smoke.ParticleTimeToLive = 0.5f;
            this.smoke.CreationFrecuency = 0.001f;
            this.smoke.Dispersion = 20;
            this.smoke.Speed = new TGCVector3(1, 1, 1);
        }

        private void CreateSpark()
        {
            ParticleEmitter particle = new ParticleEmitter(GlobalConcepts.GetInstance().GetMediaDir() + "Texturas\\Chispas\\Chispas.png", 10);
            particle.Position = this.GetPosition();
            particle.MinSizeParticle = 1f;
            particle.MaxSizeParticle = 2f;
            particle.ParticleTimeToLive = 0.5f;
            particle.CreationFrecuency = 0.1f;
            particle.Dispersion = 30;
            particle.Playing = false;
            particle.Speed = new TGCVector3(1, 1, 1);
            this.spark = new ParticleTimer(particle, 1f);
        }

        public TGCVector3 GetDirectionOfCollision()
        {
            return (this.velocidadActual >= 0) ? this.vectorAdelante : -this.vectorAdelante;
        }

        public float GetMaxForwardVelocity()
        {
            return this.velocidadMaximaDeAvance;
        }

        public float GetLife()
        {
            return this.life;
        }

        abstract protected void CreateSounds(SoundsManager soundsManager);

        public float GetMaxBackwardVelocity()
        {
            return this.velocidadMaximaDeRetroceso;
        }

        public void ChangePosition(TGCMatrix nuevaPosicion)
        {
            this.SetTranslate(nuevaPosicion);
        }

        public void AddWeapon(Weapon weapon)
        {
            this.weapons.Add(weapon);
        }

        public bool IsColliding(Weapon weapon)
        {
            return TgcCollisionUtils.testSphereOBB(weapon.sphere, this.obb.GetBoundingOrientedBox());
        }

        public void HandleCollision(Vehicle car)
        {
            if (TgcCollisionUtils.testObbObb(car.GetTGCBoundingOrientedBox(), this.GetTGCBoundingOrientedBox()))
            {
                this.Collide(car);
            }
        }

        public void Cure(float value)
        {
            life = FastMath.Min(100, life + value);
        }

        public void HandleCollision(Weapon weapon)
        {
            weapon.IAmTheCar(this);
        }

        private void Collide(Vehicle car)
        {
            TgcRay ray = new TgcRay();
            ray.Origin = this.GetLastPosition();
            ray.Direction = this.GetDirectionOfCollision();
            if (this.IntersectRayAABB(ray, car.mesh.BoundingBox))
            {
                float distanceAI = car.GetDistanceOfCollision(this.vectorAdelante, this.velocidadActual);
                float distance = this.GetDistanceOfCollision(car.GetVectorAdelante(), car.GetVelocidadActual());
                car.SetEstado(new Crashing(car, distanceAI, this.GetVectorAdelante()));
                this.SetEstado(new Crashing(this, distance, car.GetVectorAdelante()));
            }
        }

        private float GetDistanceOfCollision(TGCVector3 direction, float velocity)
        {
            return (direction * velocity).Length() * this.elapsedTime;
        }

        private bool IntersectRayAABB(TgcRay ray, TgcBoundingAxisAlignBox aabb)
        {
            TgcBoundingAxisAlignBox.Face[] faces = aabb.computeFaces();
            foreach (TgcBoundingAxisAlignBox.Face face in faces)
            {
                if (TgcCollisionUtils.intersectRayPlane(ray, face.Plane, out float t, out TGCVector3 intersection))
                {
                    return true;
                }
            }
            return false;
        }

        public TgcMesh GetCollidable(Vehicle car)
        {
            //TODO ahora devuelve null por que no podes chocar con vos mismo
            //pero cuando hayan mas autos, esto debe chequear la colision
            return null;
        }

        public void SetTranslate(TGCMatrix newTranslate)
        {
            this.matrixs.SetTranslation(newTranslate);
            this.Transform();
        }

        public EstadoVehiculo GetEstado()
        {
            return estado;
        }

        public void RotateOBB(float rotacion)
        {
            this.obb.Rotate(rotacion);
        }

        protected void CreateMesh(string rutaAMesh, TGCVector3 posicionInicial)
        {
            TgcSceneLoader loader = new TgcSceneLoader();
            TgcScene scene = loader.loadSceneFromFile(rutaAMesh);
            this.mesh = scene.Meshes[0];
            this.mesh.AutoTransform = false;
            this.matrixs.SetScalation(TGCMatrix.Scaling(escaladoInicial));
            this.matrixs.Translate(TGCMatrix.Translation(posicionInicial));
            this.trasladoInicial = this.matrixs.GetTranslation();
            this.mesh.BoundingBox.transform(this.matrixs.GetTransformation());
            this.obb = new BoundingOrientedBox(this.mesh.BoundingBox);
            this.mesh.Effect = TgcShaders.loadEffect(GlobalConcepts.GetInstance().GetShadersDir() + "EfectosVehiculo.fx");
            mesh.Technique = "Iluminate";
            this.pointsOfCollision = new PointsOfCollision(mesh.getVertexPositions());
            this.pointsOfCollision.medium = (mesh.BoundingBox.PMax + mesh.BoundingBox.PMin) * 0.5f;

        }

        public float SetDirection(TGCVector3 output, TGCVector3 normal)
        {
            float angle = GlobalConcepts.GetInstance().AngleBetweenVectors(normal, output);
            TGCVector3 result = TGCVector3.Cross(output, normal);
            //por la regla de la mano derecha
            angle = (result.Y < 0) ? angle + FastMath.PI : angle;
            this.Girar(angle);
            return angle;

        }

        public void SetVectorAdelante(TGCVector3 vector)
        {
            this.vectorAdelante = vector;
        }

        public float GetVelocidadDeRotacion()
        {
            double anguloRadianes = delanteraIzquierda.FrontVectorAngle();
            if (anguloRadianes <= 0.5f)
            {
                return this.velocidadRotacion;
            }
            return (float)anguloRadianes * 1.5f;
        }

        //sirve para imprimirlo por pantalla
        public float GetVelocidadActualDeSalto()
        {
            return this.velocidadActualDeSalto;
        }

        public float GetMaximunForwardVelocity()
        {
            return this.velocidadMaximaDeAvance;
        }

        public float VelocidadFisica()
        {
            return System.Math.Min(this.velocidadMaximaDeAvance, this.velocidadActual + this.aceleracionAvance * this.deltaTiempoAvance.tiempoTranscurrido());
        }

        public float VelocidadFisicaRetroceso()
        {
            return System.Math.Max(-this.velocidadMaximaDeRetroceso, this.velocidadActual + (-this.aceleracionRetroceso) * this.deltaTiempoAvance.tiempoTranscurrido());
        }

        public TGCVector3 GetVectorAdelante()
        {
            return this.vectorAdelante;
        }

        public void Girar(float rotacionReal)
        {
            var rotacionRueda = (rotacionReal > 0) ? 1f * this.GetElapsedTime() : -1f * this.GetElapsedTime();
            TGCMatrix matrizDeRotacion = TGCMatrix.RotationY(rotacionReal);
            this.Rotate(rotacionReal);
            this.vectorAdelante.TransformCoordinate(matrizDeRotacion);
            this.RotarDelanteras((this.GetVelocidadActual() > 0) ? rotacionRueda : -rotacionRueda);
            //this.camara.interpolador.Acumulate(rotacionReal);
            this.RotateOBB(rotacionReal);
        }


        public void SetElapsedTime()
        {
            this.elapsedTime = GlobalConcepts.GetInstance().GetElapsedTime();
            if (this.deltaTiempoAvance.tiempoTranscurrido() != 0)
            {
                this.deltaTiempoAvance.acumularTiempo(this.elapsedTime);
            }
            if (this.deltaTiempoSalto.tiempoTranscurrido() != 0)
            {
                this.deltaTiempoSalto.acumularTiempo(this.elapsedTime);
            }

        }

        public float GetVelocidadActual()
        {
            return this.velocidadActual;
        }

        private void RenderBoundingOrientedBox()
        {
            this.obb.Render();
        }

        virtual public void Render()
        {
            //shaderTime.acumularTiempo(GetElapsedTime());
            //shaderColorModifier.Modify(0.01f);
            //effect.SetValue("time", shaderTime.tiempoTranscurrido());
            //effect.SetValue("bluecolor", shaderColorModifier.Modifier);
            if (this.IsInView(this.mesh))
            {
                this.SetEffectAtributes();
                this.mesh.Render();
                this.RenderBoundingOrientedBox();
                this.mesh.BoundingBox.Render();
                delanteraIzquierda.Render();
                delanteraDerecha.Render();
                foreach (var rueda in this.ruedas)
                {
                    rueda.Render();
                }
                this.RenderLights();
                this.smoke.render(this.elapsedTime);
                this.spark.Render(this.elapsedTime);
            }


        }

        private void SetEffectAtributes()
        {
            Lighting.LightManager.GetInstance().DoLightMe(this.mesh.Effect);
            this.mesh.Effect.SetValue("pointsOfCollision", this.pointsOfCollision.CalculateDeformation(life));
            this.mesh.Effect.SetValue("radio", this.pointsOfCollision.radio);
            this.mesh.Effect.SetValue("constantOfDeformation", this.pointsOfCollision.constantOfDeformation);
            Vector4 vector = new Vector4(pointsOfCollision.medium.X, pointsOfCollision.medium.Y, pointsOfCollision.medium.Z,1);
            this.mesh.Effect.SetValue("medium", vector);
        }

        private void RenderLights()
        {
            this.reverseLights.Render();
            this.breakLights.Render();
            this.frontLights.Render();
            this.breakLights.DesactivateLight();
        }

        public TgcBoundingOrientedBox GetTGCBoundingOrientedBox()
        {
            return this.obb.GetBoundingOrientedBox();
        }

        public BoundingOrientedBox GetBoundingOrientedBox()
        {
            return this.obb;
        }

        private void DisposeLights()
        {
            this.frontLights.Dispose();
            this.breakLights.Dispose();
            this.reverseLights.Dispose();
        }

        public void Dispose()
        {
            this.mesh.Dispose();
            this.mesh.Effect.Dispose();
            this.DisposeLights();
        }

        public Timer GetDeltaTiempoAvance()
        {
            return this.deltaTiempoAvance;
        }

        public float GetElapsedTime()
        {
            return this.elapsedTime;
        }

        public void SetVelocidadActual(float nuevaVelocidad)
        {
            this.velocidadActual = nuevaVelocidad;
        }

        public void SetEstado(EstadoVehiculo estado)
        {
            this.estado = estado;
        }

        public void SetVelocidadActualDeSalto(float velocidad)
        {
            this.velocidadActualDeSalto = velocidad;
        }

        public float GetAceleracionGravedad()
        {
            return this.aceleracionGravedad;
        }

        public Timer GetDeltaTiempoSalto()
        {
            return this.deltaTiempoSalto;
        }

        public float GetVelocidadMaximaDeSalto()
        {
            return this.velocidadInicialDeSalto;
        }

        public float GetConstanteRozamiento()
        {
            return this.constanteDeRozamiento;
        }

        public float GetConstanteFrenado()
        {
            return this.constanteFrenado;
        }

        public void Move(TGCVector3 desplazamiento)
        {

            this.matrixs.Translate(TGCMatrix.Translation(desplazamiento));
            this.delanteraIzquierda.RotateX(this.GetVelocidadActual());
            this.delanteraDerecha.RotateX(this.GetVelocidadActual());
            foreach (var rueda in this.ruedas)
            {
                rueda.RotateX(this.GetVelocidadActual());
            }
            Transform();
        }

        public void Translate(TGCMatrix displacement)
        {
            this.matrixs.Translate(displacement);
        }

        public TGCVector3 GetPosition()
        {
            return TGCVector3.transform(new TGCVector3(0, 0, 0), this.matrixs.GetTransformation());
        }

        public TGCVector3 GetVectorCostadoIzquierda()
        {
            return TGCVector3.Cross(this.vectorAdelante, new TGCVector3(0, 1, 0));
        }


        public List<Wheel> GetRuedas()
        {
            return this.ruedas;

        }

        virtual public void Transform()
        {
            TGCMatrix transformacion = this.matrixs.GetTransformation();
            this.mesh.Transform = transformacion;
            this.obb.ActualizarBoundingOrientedBox(this.matrixs.GetTranslation());
            this.delanteraIzquierda.Transform(transformacion);
            this.delanteraDerecha.Transform(transformacion);
            this.mesh.BoundingBox.transform(transformacion);
            this.TransformLights(transformacion);
            foreach (var rueda in this.ruedas)
            {
                rueda.Transform(transformacion);
            }
        }

        private void TransformLights(TGCMatrix matrix)
        {
            this.breakLights.SetTransformation(matrix);
            this.frontLights.SetTransformation(matrix);
            this.reverseLights.SetTransformation(matrix);
            this.breakLights.Transform();
            this.frontLights.Transform();
            this.reverseLights.Transform();
        }

        public void Rotate(float rotacion)
        {
            this.matrixs.Rotate(TGCMatrix.RotationY(rotacion));
        }
        /// <summary>
        /// Rotar delanteras con A,D
        /// </summary>
        public void RotarDelanteras(float rotacion)
        {
            delanteraIzquierda.RotateY(rotacion);
            delanteraDerecha.RotateY(rotacion);
        }

        public void UpdateFrontWheels(float rotacion)
        {
            delanteraIzquierda.UpdateRotationY(rotacion);
            delanteraDerecha.UpdateRotationY(rotacion);
        }

        public TGCVector3 GetLastPosition()
        {
            return TGCVector3.transform(new TGCVector3(0, 0, 0), this.lastTransformation);
        }

        virtual public void Crash(float angle)
        {
            this.TakeDamage(2.5f);
            this.deltaTiempoAvance.resetear();
            this.velocidadActual *= 0.5f;
            this.SoundsManager.GetSound("Choque").play();
            this.spark.Play(this.GetPosition());
        }

        private void NextWeapon()
        {
            if (!HaveWeapons()) return;
            Weapon actualWeapon = weapons[0];
            Type type = actualWeapon.GetType();
            Weapon nextWeapon = weapons.Find(w => w != actualWeapon && !w.Equals(type));
            if (nextWeapon == null) return;
            weapons.Remove(nextWeapon);
            weapons.Insert(0, nextWeapon);
        }

        public void Shoot()
        {
            Weapon weapon = (HaveWeapons()) ? this.weapons[0] : null;
            if (weapon != null && this.timeShoot == 0f)
            {
                weapon.Shoot(this);
                this.timeShoot = 0.5f;
                Remove(weapon);
                NextSameWeapon(weapon);
            }
        }

        private void NextSameWeapon(Weapon weapon)
        {
            Type type = weapon.GetType();
            Weapon nextWeapon = weapons.Find(w => w.GetType() == type);
            if (nextWeapon == null) return;
            Remove(nextWeapon);
            weapons.Insert(0, nextWeapon);
        }

        public string GetNameSelectWeapon()
        {
            if (!HaveWeapons()) return "EMPTY";
            return weapons[0].ToString();
        }


        public string GetNumberOfBulletsOfFirstWeapon()
        {
            if (!HaveWeapons()) return "EMPTY";
            Weapon weapon = weapons[0];
            Type type = weapon.GetType();
            int count = 1;
            foreach (Weapon w in weapons)
            {
                if(w.GetType() == type && w != weapon)
                {
                    count++;
                }
            }
            return count.ToString();
        }

        public void Action(TgcD3dInput input)
        {
            this.SetElapsedTime();
            this.lastTransformation = this.matrixs.GetTransformation();
            this.UpdateValues();
            this.ManageEntry(input);

        }

        virtual protected void ManageEntry(TgcD3dInput input)
        {
            if (input.keyDown(Key.NumPad4))
            {
                Scene.GetInstance().camera.rotateY(-0.005f);
            }
            if (input.keyDown(Key.NumPad6))
            {
                Scene.GetInstance().camera.rotateY(0.005f);
            }

            if (input.keyDown(Key.RightArrow))
            {
                Scene.GetInstance().camera.OffsetHeight += 0.05f;
            }
            if (input.keyDown(Key.LeftArrow))
            {
                Scene.GetInstance().camera.OffsetHeight -= 0.05f;
            }

            if (input.keyDown(Key.UpArrow))
            {
                Scene.GetInstance().camera.OffsetForward += 0.05f;
            }
            if (input.keyDown(Key.DownArrow))
            {
                Scene.GetInstance().camera.OffsetForward -= 0.05f;
            }

            if (input.keyDown(Key.W))
            {
                this.estado.Advance();
            }

            if (input.keyDown(Key.S))
            {
                this.estado.Back();
            }

            float rotation;

            if (input.keyDown(Key.D))
            {
                rotation = this.estado.Right();
                Scene.GetInstance().camera.rotateY(rotation);

            }
            else if (input.keyDown(Key.A))
            {
                rotation = this.estado.Left();
                Scene.GetInstance().camera.rotateY(rotation);
            }

            if (!input.keyDown(Key.A) && !input.keyDown(Key.D))
            {
                this.estado.UpdateWheels();
            }

            if (input.keyDown(Key.Space))
            {
                this.estado.Jump();
            }

            if (!input.keyDown(Key.W) && !input.keyDown(Key.S))
            {
                this.estado.SpeedUpdate();
            }

            if (input.keyDown(Key.P))
            {
                this.Shoot();
            }

            if (input.keyDown(Key.E))
            {
                this.SoundsManager.GetSound("Bocina").play();
            }

            if (input.keyDown(Key.R))
            {
                this.SoundsManager.GetSound("Alarma").play();
            }

            if (input.keyUp(Key.U))
            {
                NextWeapon();
            }

            if (input.keyDown(Key.C))
            {
                estado = new Frozen(this);
            }
            if (input.keyDown(Key.NumPad1))
            {
                this.pointsOfCollision.constantOfDeformation += 0.01f;
            }
            if (input.keyDown(Key.NumPad2))
            {
                this.pointsOfCollision.constantOfDeformation -= 0.01f;
            }

            if (input.keyDown(Key.NumPad7))
            {
                this.pointsOfCollision.radio -= 0.01f;
            }
            if (input.keyDown(Key.NumPad9))
            {
                this.pointsOfCollision.radio += 0.01f;
            }
        }

        virtual protected void UpdateValues()
        {
            this.SoundsManager.UpdatePositions(this.GetPosition());
            this.UpdateSmoke();
            this.UpdateShootTime();
            this.SoundsManager.Update(this.velocidadActual);
            this.estado.JumpUpdate();
            this.estado.FrozenTimeUpdate();
            //this.camara.UpdateInterpolation(this.elapsedTime);
        }

        private void UpdateShootTime()
        {
            if (this.timeShoot > 0)
            {
                this.timeShoot -= this.elapsedTime;
                this.timeShoot = FastMath.Max(0f, this.timeShoot);

            }
        }

        private void UpdateSmoke()
        {
            if (this.life >= 50)
            {
                this.smoke.Playing = false;
            }
            else if (this.life < 50 && this.life >= 20) {
                this.smoke.Playing = true;
            }
            else
            {
                this.smoke.changeTexture(GlobalConcepts.GetInstance().GetMediaDir() + "Texturas\\Humo\\fuego.png");
            }
            this.smoke.Position = this.GetPosition();
        }

        public void Remove(Weapon weapon)
        {
            this.weapons.Remove(weapon);
        }

        public bool HaveWeapons()
        {
            return this.weapons.Count > 0;
        }

        public void ChangeToFreeze()
        {
            this.estado = new Frozen(this);
        }

        public void Freeze()
        {
            this.mesh.Technique = "Freeze";
        }

        public void UnFreeze()
        {
            this.mesh.Technique = "Iluminate";
        }

        private class PointsOfCollision{
            private Vector4[] pointsOfCollision;
            private int index = 0;
            //si se cambia el numero máximo de elementos, hay que cambiarlo tambien en el shader
            private int max = 12;
            public float radio = 8;
            public float constantOfDeformation = 0.89f;
            public TGCVector3 medium;
            private TGCVector3[] vertexsPositions;

            public PointsOfCollision(TGCVector3[] vertexsPosition)
            {
                pointsOfCollision = new Vector4[max];
                this.Reset();
                this.vertexsPositions = new TGCVector3[max];
                this.vertexsPositions[0] = (vertexsPosition[6]);
                this.vertexsPositions[1] = (vertexsPosition[7]);
                this.vertexsPositions[2] = (vertexsPosition[28]);
                this.vertexsPositions[3] = (vertexsPosition[33]);
                this.vertexsPositions[4] = (vertexsPosition[44]);
                this.vertexsPositions[5] = (vertexsPosition[47]);
                this.vertexsPositions[6] = (vertexsPosition[70]);
                this.vertexsPositions[7] = (vertexsPosition[130]);
                this.vertexsPositions[8] = (vertexsPosition[169]);
                this.vertexsPositions[9] = (vertexsPosition[239]);
                this.vertexsPositions[10] = (vertexsPosition[414]);
                this.vertexsPositions[11] = (vertexsPosition[773]);


            }

            public void Reset()
            {
                for (int i = 0; i < max; i++)
                {
                    pointsOfCollision[i] = new Vector4(0, 0, 0, 0);
                }
                index = 0;
            }

            public void AddFirst(int numberOfElements)
            {
                for (int i = 0; i < numberOfElements; i++)
                {
                    this.AddPointOfCollision(this.vertexsPositions[i]);
                }
            }

            public Vector4[] CalculateDeformation(float life)
            {
                this.Reset();
                if (life > 75)
                {
                }
                else if (life > 50)
                {
                    this.AddFirst(4);
                }
                else if(life > 25)
                {
                    this.AddFirst(8);
                }
                else
                {
                    this.AddFirst(12);
                }
                return this.pointsOfCollision;
            }

            public void AddPointOfCollision(TGCVector3 point)
            {
                if (index >= max) return;
                pointsOfCollision[index] = new Vector4(point.X, point.Y, point.Z, 1);
                index++;
            }

        }
    }
}
