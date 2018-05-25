using System;
using System.Collections.Generic;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Group.Model.Vehiculos.Estados;
using TGC.Group.Model.Vehiculos;
using TGC.Core.BoundingVolumes;
using TGC.Core.Input;
using Microsoft.DirectX.DirectInput;

namespace TGC.Group.Model
{
    abstract class Vehiculo
    {

        public TgcMesh mesh;
        private BoundingOrientedBox obb;
        private Timer deltaTiempoAvance;
        private Timer deltaTiempoSalto;
        public TGCVector3 vectorAdelante;
        public TGCMatrix traslado, rotado, escalado;
        protected List<Rueda> ruedas = new List<Rueda>();
        protected Rueda delanteraIzquierda;
        protected Rueda delanteraDerecha;
        protected TGCVector3 vectorDireccion;
        private EstadoVehiculo estado;
        private float velocidadActual = 0f;
        private float velocidadActualDeSalto;
        protected float velocidadRotacion = 1f;
        protected float velocidadInicialDeSalto = 15f;
        protected float velocidadMaximaDeAvance = 60f;
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
        protected CamaraEnTerceraPersona camara;

        private List<Weapon> weapons = new List<Weapon>();

        public Vehiculo(CamaraEnTerceraPersona camara, TGCVector3 posicionInicial, SoundsManager soundsManager)
        {
            this.camara = camara;
            this.SoundsManager = soundsManager;
            this.vectorAdelante = new TGCVector3(0, 0, 1);
            this.CrearMesh(ConceptosGlobales.GetInstance().GetMediaDir() + "meshCreator\\meshes\\Vehiculos\\Camioneta\\Camioneta-TgcScene.xml", posicionInicial);
            this.velocidadActualDeSalto = this.velocidadInicialDeSalto;
            this.deltaTiempoAvance = new Timer();
            this.deltaTiempoSalto = new Timer();
            this.aceleracionRetroceso = this.aceleracionAvance * 0.8f;
            this.vectorDireccion = this.vectorAdelante;
            this.estado = new Stopped(this);
            this.obb = new BoundingOrientedBox(this.mesh.BoundingBox);

        }

        public void ChangePosition(TGCMatrix nuevaPosicion)
        {
            this.SetTranslate(nuevaPosicion);
        }

        public void SetTranslate(TGCMatrix newTranslate)
        {
            this.traslado = newTranslate;
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

        private void CrearMesh(string rutaAMesh, TGCVector3 posicionInicial)
        {
            TgcSceneLoader loader = new TgcSceneLoader();
            TgcScene scene = loader.loadSceneFromFile(rutaAMesh);
            this.mesh = scene.Meshes[0];
            this.mesh.AutoTransform = false;
            this.rotado = TGCMatrix.RotationYawPitchRoll(0,0,0);
            this.escalado = TGCMatrix.Scaling(this.escaladoInicial);
            this.traslado = TGCMatrix.Translation(posicionInicial.X, posicionInicial.Y, posicionInicial.Z);
            this.trasladoInicial = traslado;

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

        public float VelocidadFisica()
        {
            return System.Math.Min(this.velocidadMaximaDeAvance, this.velocidadActual + this.aceleracionAvance * this.deltaTiempoAvance.tiempoTranscurrido());
        }

        public float VelocidadFisicaRetroceso()
        {
            return System.Math.Max(-this.velocidadMaximaDeAvance, this.velocidadActual + (-this.aceleracionRetroceso) * this.deltaTiempoAvance.tiempoTranscurrido());
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
            this.camara.rotateY(rotacionReal);
            this.RotateOBB(rotacionReal);
        }

        public void addWeapon(Weapon weapon)
        {
            this.weapons.Add(weapon);
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

        public float GetVelocidadActual()
        {
            return this.velocidadActual;
        }

        private void RenderBoundingOrientedBox()
        {
            this.obb.Render();
        }

        public void Render()
        {
            this.mesh.Render();
            this.RenderBoundingOrientedBox();
            delanteraIzquierda.Render();
            delanteraDerecha.Render();
            foreach (var rueda in this.ruedas)
            {
                rueda.Render();
            }
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

            this.traslado = this.traslado * TGCMatrix.Translation(desplazamiento.X, desplazamiento.Y, desplazamiento.Z);
            this.delanteraIzquierda.RotateX(this.GetVelocidadActual());
            this.delanteraDerecha.RotateX(this.GetVelocidadActual());
            foreach (var rueda in this.ruedas)
            {
                rueda.RotateX(this.GetVelocidadActual());
            }
        }

        public TGCVector3 GetPosicion()
        {
            return TGCVector3.transform(new TGCVector3(0, 0, 0), this.rotado * this.traslado);
        }

        public TGCVector3 GetVectorCostado()
        {
            return TGCVector3.Cross(this.vectorAdelante, new TGCVector3(0, 1, 0));
        }


        public List<Rueda> GetRuedas()
        {
            return this.ruedas;

        }

        virtual public void Transform()
        {
            var transformacion = GetTransformacion();
            this.mesh.Transform = transformacion;
            this.mesh.BoundingBox.transform(this.escalado * this.traslado);
            this.obb.ActualizarBoundingOrientedBox(this.mesh.BoundingBox);
            this.delanteraIzquierda.Transform(this.GetTransformacion());
            this.delanteraDerecha.Transform(this.GetTransformacion());

            foreach (var rueda in this.ruedas)
            {
                rueda.Transform(this.GetTransformacion());
            }
        }

        public TGCMatrix GetTransformacion()
        {
            return this.escalado * this.rotado * this.traslado;
        }

        public void Rotate(float rotacion)
        {
            this.rotado = TGCMatrix.RotationY(rotacion) * this.rotado;
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

        public void Action(TgcD3dInput input)
        {
            if (input.keyDown(Key.NumPad4))
            {
                this.camara.rotateY(-0.005f);
            }
            if (input.keyDown(Key.NumPad6))
            {
                this.camara.rotateY(0.005f);
            }

            if (input.keyDown(Key.RightArrow))
            {
                this.camara.OffsetHeight += 0.05f;
            }
            if (input.keyDown(Key.LeftArrow))
            {
                this.camara.OffsetHeight -= 0.05f;
            }

            if (input.keyDown(Key.UpArrow))
            {
                this.camara.OffsetForward += 0.05f;
            }
            if (input.keyDown(Key.DownArrow))
            {
                this.camara.OffsetForward -= 0.05f;
            }

            if (input.keyDown(Key.W))
            {
                this.SoundsManager.PlayAccelerating();
                this.estado.Advance();
            }
            else
            {
                this.SoundsManager.PlayDesaccelerating();
            }

            if (input.keyDown(Key.S))
            {
                this.estado.Back();
            }

            if (input.keyDown(Key.D))
            {
                this.estado.Right();

            }
            else if (input.keyDown(Key.A))
            {
                this.estado.Left();
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

            this.estado.JumpUpdate();
            this.camara.Target = (this.GetPosicion()) + this.GetVectorAdelante() * 30;
        }
    }
}
