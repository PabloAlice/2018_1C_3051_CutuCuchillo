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
        protected TransformationMatrix matrixs;
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
        protected List<Weapon> weapons = new List<Weapon>();

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
            this.CreateSmoke();

        }

        public bool IsInView()
        {
            //por ahora lo unico que tiene obb son los vehiculos
            //y se renderizan por su cuenta
            return true;
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
            this.smoke.Speed = new TGCVector3(1,1,1);
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

        public bool IsColliding(Weapon weapon, out Collidable element)
        {

            if (TgcCollisionUtils.testSphereOBB(weapon.sphere, this.obb.GetBoundingOrientedBox()))
            {
                element = this;
                return true;
            }
            element = null;
            return false;
        }

        public void HandleCollisions(Vehicle car)
        {
            /*if (TgcCollisionUtils.testObbObb(car.GetTGCBoundingOrientedBox(), this.GetObb()))
            {
                this.Collide(car);
            }*/
            return;
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

        protected void CrearMesh(string rutaAMesh, TGCVector3 posicionInicial)
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
            this.mesh.Effect =  TgcShaders.loadEffect(GlobalConcepts.GetInstance().GetShadersDir() + "FrozenMeshShader.fx");
            mesh.Technique = "Unfreeze";

        }

        public float SetDirection(TGCVector3 output, TGCVector3 normal)
        {
            float angle = GlobalConcepts.GetInstance().AngleBetweenVectors(normal, output);
            TGCVector3 result = TGCVector3.Cross(output, normal);
            //por la regla de la mano derecha
            angle = (result.Y < 0)? angle + FastMath.PI : angle;
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
            if(anguloRadianes <= 0.5f )
            {
                return this.velocidadRotacion;
            }
            return (float) anguloRadianes * 1.5f;
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
            if(this.deltaTiempoAvance.tiempoTranscurrido() != 0)
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
            Lighting.LightManager.GetInstance().DoLightMe(this.mesh);
            this.mesh.Render();
            this.RenderBoundingOrientedBox();
            delanteraIzquierda.Render();
            delanteraDerecha.Render();
            foreach (var rueda in this.ruedas)
            {
                rueda.Render();
            }
            this.smoke.render(this.elapsedTime);
        }


        public TgcBoundingOrientedBox GetTGCBoundingOrientedBox()
        {
            return this.obb.GetBoundingOrientedBox();
        }

        public BoundingOrientedBox GetBoundingOrientedBox()
        {
            return this.obb;
        }

        public void Dispose()
        {
            this.mesh.Dispose();
            this.mesh.Effect.Dispose();
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

        public TGCVector3 GetVectorCostado()
        {
            return TGCVector3.Cross(this.vectorAdelante, new TGCVector3(0, 1, 0));
        }


        public List<Wheel> GetRuedas()
        {
            return this.ruedas;

        }

        virtual public void Transform()
        {
            var transformacion = this.matrixs.GetTransformation();
            this.mesh.Transform = transformacion;
            this.obb.ActualizarBoundingOrientedBox(this.matrixs.GetTranslation());
            this.delanteraIzquierda.Transform(transformacion);
            this.delanteraDerecha.Transform(transformacion);

            foreach (var rueda in this.ruedas)
            {
                rueda.Transform(transformacion);
            }
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
            return TGCVector3.transform(new TGCVector3(0,0,0), this.lastTransformation);
        }

        virtual public void Crash(float angle)
        {
            this.life = (this.life - 5f < 0) ? 0 : this.life - 5f;
            this.SoundsManager.GetSound("Choque").play();
        }

        private int NumberOfWeapons()
        {
            return this.weapons.Count;
        }

        private void Shoot()
        {
            Weapon weapon = (this.NumberOfWeapons() > 0) ? this.weapons[0] : null;
            if (weapon != null)
            {
                weapon.Shoot(this);
            }
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
            else
            {
                // int freq = (int)(this.constanteDeRozamiento / this.GetElapsedTime());
                // int f2 = this.SoundsManager.desacceleratingVelSound.SoundBuffer.Frequency;
                // this.SoundsManager.SetDesAccFrequency(f2);
            }

            if (input.keyDown(Key.S))
            {
                //int freq = (int)(this.constanteFrenado / this.GetElapsedTime());
                //this.SoundsManager.SetDesAccFrequency(freq);
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

            if (input.keyDown(Key.C))
            {
                estado = new Frozen(this);
            }
        }

        virtual protected void UpdateValues()
        {
            this.SoundsManager.UpdatePositions(this.GetPosition());
            this.UpdateSmoke();
            this.SoundsManager.Update(this.velocidadActual);
            this.estado.JumpUpdate();
            this.estado.FrozenTimeUpdate();
            //this.camara.UpdateInterpolation(this.elapsedTime);
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

        public void Congelar()
        {
            this.mesh.Technique = "Freeze";
        }

        public void Descongelar()
        {
            this.mesh.Technique = "Unfreeze";
        }
    }
}
