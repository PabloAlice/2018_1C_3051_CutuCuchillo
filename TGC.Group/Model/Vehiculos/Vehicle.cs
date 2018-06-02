using System.Collections.Generic;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Group.Model.Vehiculos;
using TGC.Core.BoundingVolumes;
using TGC.Core.Input;
using Microsoft.DirectX.DirectInput;
using System;

namespace TGC.Group.Model
{
    abstract class Vehicle
    {

        public TgcMesh mesh;
        private BoundingOrientedBox obb { get; set; }
        private Timer deltaTiempoAvance { get; set; }
        private Timer deltaTiempoSalto { get; set; }
        public TGCVector3 vectorAdelante;
        public TGCVector3 VectorAdelanteSalto { get; set; }
        private TransformationMatrix matrixs;
        protected List<Wheel> ruedas = new List<Wheel>();
        protected Wheel delanteraIzquierda;
        protected Wheel delanteraDerecha;
        protected TGCVector3 vectorDireccion;
        private TGCVector3 Velocity;
        private TGCVector3 FinalForce { get; set; }
        protected float velocidadRotacion = 1f;
        protected float velocidadMaximaDeAvance = 60f;
        protected float velocidadMaximaDeRetroceso;
        private float elapsedTime { get; set; } = 0f;
        protected float constanteDeRozamiento { get; set; } = 0.2f;
        private TGCVector3 GravityForce = new TGCVector3(0, -10f, 0);
        private float EngineForce = 50f;
        private float BrakeForce = 100f;
        private float Mass = 10f;
        public SoundsManager SoundsManager { get; set; }
        protected TGCVector3 escaladoInicial = new TGCVector3(0.005f, 0.005f, 0.005f);
        //se guarda el traslado inicial porque se usa como pivote
        protected TGCMatrix trasladoInicial;
        protected ThirdPersonCamera camara;
        protected TGCMatrix lastTransformation;
        protected float life = 100f;

        private List<IShootable> weapons = new List<IShootable>();
        private int currentWeaponIndex = 0;

        public Vehicle(ThirdPersonCamera camara, TGCVector3 posicionInicial, SoundsManager soundsManager)
        {
            this.matrixs = new TransformationMatrix();
            this.camara = camara;
            this.SoundsManager = soundsManager;
            this.vectorAdelante = new TGCVector3(0, 0, 1);
            this.CrearMesh(GlobalConcepts.GetInstance().GetMediaDir() + "meshCreator\\meshes\\Vehiculos\\Camioneta\\Camioneta-TgcScene.xml", posicionInicial);
            this.Velocity = new TGCVector3(0, 0, 0);
            this.FinalForce = new TGCVector3(0, 0, 0);
            this.deltaTiempoAvance = new Timer();
            this.deltaTiempoSalto = new Timer();
            this.vectorDireccion = this.vectorAdelante;
            this.mesh.BoundingBox.transform(this.matrixs.GetTransformation());
            this.obb = new BoundingOrientedBox(this.mesh.BoundingBox);
            this.weapons.Add(new DefaultWeapon());
            this.camara.SetPlane(this.vectorAdelante);
            this.velocidadMaximaDeRetroceso = this.velocidadMaximaDeAvance * 0.7f;
        }

        public TGCVector3 GetDirectionOfCollision()
        {
            return this.Velocity;
        }

        public ThirdPersonCamera GetCamara()
        {
            return this.camara;
        }

        public void ChangePosition(TGCMatrix nuevaPosicion)
        {
            this.SetTranslate(nuevaPosicion);
        }

        public void SetTranslate(TGCMatrix newTranslate)
        {
            this.matrixs.SetTranslation(newTranslate);
            this.Transform();
        }

        public void RotateOBB(float rotacion)
        {
            this.obb.Rotate(rotacion);
        }

        private void CrearMesh(string rutaAMesh, TGCVector3 posicionInicial)
        {
            TgcSceneLoader loader = new TgcSceneLoader();
            TgcScene scene = loader.loadSceneFromFile(rutaAMesh);
            this.mesh = scene.Meshes[0];
            this.mesh.AutoTransform = false;
            this.matrixs.SetScalation(TGCMatrix.Scaling(escaladoInicial));
            this.matrixs.Translate(TGCMatrix.Translation(posicionInicial));
            this.trasladoInicial = this.matrixs.GetTranslation();

        }

        public void SetDirection(TGCVector3 output, TGCVector3 normal)
        {
            float angle = GlobalConcepts.GetInstance().AngleBetweenVectors(normal, output);
            TGCVector3 result = TGCVector3.Cross(output, normal);
            //por la regla de la mano derecha
            angle = (result.Y < 0)? angle + FastMath.PI : angle;
            this.Girar(angle);
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
            return this.Velocity.Y;
        }

        private TGCVector3 GetAcceleration()
        {
            return this.FinalForce * (1 / this.Mass);
        }

        public TGCVector3 ApplyForce(TGCVector3 force)
        {
            this.FinalForce = FinalForce + force;
            return this.FinalForce;
        }

        public void UpdateVelocity()
        {
            this.Velocity = this.Velocity + this.GetAcceleration() * this.elapsedTime;
            Console.WriteLine(this.GetAcceleration());
        }

        public TGCVector3 GetVectorAdelante()
        {
            return this.vectorAdelante;
        }

        public void Girar(float rotacionReal)
        {
            var rotacionRueda = (rotacionReal > 0) ? 1f * this.elapsedTime : -1f * this.elapsedTime;
            TGCMatrix matrizDeRotacion = TGCMatrix.RotationY(rotacionReal);
            this.Rotate(rotacionReal);
            this.vectorAdelante.TransformCoordinate(matrizDeRotacion);
            this.RotarDelanteras((this.GetXZVelocity() > 0) ? rotacionRueda : -rotacionRueda);
            this.camara.rotateY(rotacionReal);
            this.RotateOBB(rotacionReal);
        }

        public float GetXZVelocity()
        {
            return new TGCVector2(this.Velocity.X, this.Velocity.Z).Length();
        }
       
        public void SetElapsedTime(float time)
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
            this.obb.Render();
            delanteraIzquierda.Render();
            delanteraDerecha.Render();
            foreach (var rueda in this.ruedas)
            {
                rueda.Render();
            }
            foreach(IShootable w in weapons)
            {
                w.renderProjectiles();
            }
        }

        //-------------------------------------------------------
        // WEAPON
        public void addWeapon(Weapon weapon)
        {
            this.weapons.Add(weapon);
        }

        public void shoot()
        {
            weapons[currentWeaponIndex].addProjectile(new Projectile(this.GetPosicion(), this.vectorAdelante));
        }



        //-------------------------------------------------------


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
        }

        public void Move(TGCVector3 desplazamiento)
        {

            this.matrixs.Translate(TGCMatrix.Translation(desplazamiento));
            this.delanteraIzquierda.RotateX(this.GetXZVelocity());
            this.delanteraDerecha.RotateX(this.GetXZVelocity());
            foreach (var rueda in this.ruedas)
            {
                rueda.RotateX(this.GetXZVelocity());
            }
        }

        public void Translate(TGCMatrix displacement)
        {
            this.matrixs.Translate(displacement);
        }

        public TGCVector3 GetPosicion()
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

        public void Crash()
        {
            this.life = (this.life - 5f < 0) ? 0 : this.life - 5f;
        }

        private void ApplyFrictionForce(float constant = 0.2f)
        {
            if (this.GetXZVelocity() < 0.01) return;
            this.ApplyForce(this.Velocity * (1/ this.Velocity.Length()) * (-constant) * this.Mass * this.GravityForce.Length());
        }

        public void Action(TgcD3dInput input, CustomSprite velocimetro, CustomSprite bar)
        {
            this.lastTransformation = this.matrixs.GetTransformation();
            this.SoundsManager.Update(this.GetXZVelocity());

            this.camara.Update(input);
            if (input.keyDown(Key.W))
            {
                this.ApplyForce(this.vectorAdelante * this.EngineForce);
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
                this.ApplyForce(-this.vectorAdelante * this.EngineForce);
            }
            if (this.GetXZVelocity() != 0) {
                if (input.keyDown(Key.D))
                {
                    float rotacionReal = this.GetVelocidadDeRotacion() * this.elapsedTime;
                    rotacionReal = (TGCVector3.Dot(this.Velocity, this.vectorAdelante) > 0) ? rotacionReal : -rotacionReal;
                    this.Girar(rotacionReal);

                }
                else if (input.keyDown(Key.A))
                {
                    float rotacionReal = this.GetVelocidadDeRotacion() * this.elapsedTime;
                    rotacionReal = (TGCVector3.Dot(this.Velocity, this.vectorAdelante) < 0) ? rotacionReal : -rotacionReal;
                    this.Girar(rotacionReal);
                }
            }

            if (!input.keyDown(Key.A) && !input.keyDown(Key.D))
            {
                var rotacionReal = this.GetVelocidadDeRotacion() * this.elapsedTime;
                this.UpdateFrontWheels(rotacionReal);
            }

            if (input.keyDown(Key.Space))
            {
                this.ApplyForce(new TGCVector3(0, 100f, 0));
            }

            if (input.keyDown(Key.P))
            {
                this.SoundsManager.Shoot();
                this.shoot();
            }
            else
            {
                this.SoundsManager.StopShoot();
            }

            if (input.keyDown(Key.E))
            {
                this.SoundsManager.Horn();
            }

            if (input.keyDown(Key.R))
            {
                this.SoundsManager.Alarm();
            }
            this.UpdateVelocity();
            float velocidadMaxima = (this.GetXZVelocity() < 0) ? this.velocidadMaximaDeRetroceso : this.velocidadMaximaDeAvance;
            float maxAngle = (this.GetXZVelocity() > 0) ? FastMath.PI + FastMath.PI / 3 : FastMath.PI_HALF;
            velocimetro.Rotation = (FastMath.Abs(this.GetXZVelocity()) * (maxAngle)) / velocidadMaxima - FastMath.PI;
            bar.Scaling = new TGCVector2((this.life * 0.07f) / 100f, 0.05f);
            this.camara.Target = (this.GetPosicion()) + this.GetVectorAdelante() * 30;
        }
    }
}
