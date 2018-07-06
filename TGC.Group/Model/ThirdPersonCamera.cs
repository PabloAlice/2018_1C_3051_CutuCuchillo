using TGC.Core.Camara;
using TGC.Core.Mathematica;
using TGC.Core.Collision;
using TGC.Core.BoundingVolumes;
using TGC.Core.Interpolation;

namespace TGC.Group.Model
{
    class ThirdPersonCamera : TgcCamera
    {
        private TGCVector3 position;
        public LinearInterpolation interpolador;
        public TgcBoundingAxisAlignBox aabb;

        /// <summary>
        ///     Crear una nueva camara
        /// </summary>
        public ThirdPersonCamera()
        {
            resetValues();
            aabb = new TgcBoundingAxisAlignBox(new TGCVector3(-1, -1, -1), new TGCVector3(1, 1, 1));
        }

        public void Update(Vehicle car)
        {
            Target = (car.GetPosition()) + car.GetVectorAdelante() * 30;
            OffsetForward = -33;
            UpdateCamera(GlobalConcepts.GetInstance().GetElapsedTime());
            CalculateAABB();
            Scene.GetInstance().HandleCollisions(this);
        }

        private void CalculateAABB()
        {
            aabb.scaleTranslate(Position, new TGCVector3(1, 1, 1));
        }

        public void ZoomIn()
        {
            OffsetForward += 0.01f;
            UpdateCamera(GlobalConcepts.GetInstance().GetElapsedTime());
            CalculateAABB();
        }

        public TGCVector3 GetPosition()
        {
            return Position;
        }

        public ThirdPersonCamera(TGCVector3 target, float offsetHeight, float offsetForward) : this()
        {
            Target = target;
            OffsetHeight = offsetHeight;
            OffsetForward = offsetForward;
            interpolador = new LinearInterpolation();
        }

        public void UpdateInterpolation(float elapsedTime)
        {
                float rotate = this.interpolador.Update(elapsedTime);
                this.rotateY(rotate);
        }

        public ThirdPersonCamera(TGCVector3 target, TGCVector3 targetDisplacement, float offsetHeight, float offsetForward)
            : this()
        {
            Target = target;
            TargetDisplacement = targetDisplacement;
            OffsetHeight = offsetHeight;
            OffsetForward = offsetForward;
            aabb = new TgcBoundingAxisAlignBox(new TGCVector3(-1, -1, -1), new TGCVector3(1, 1, 1));
        }

        /// <summary>
        ///     Desplazamiento en altura de la camara respecto del target
        /// </summary>
        public float OffsetHeight { get; set; }

        /// <summary>
        ///     Desplazamiento hacia adelante o atras de la camara repecto del target.
        ///     Para que sea hacia atras tiene que ser negativo.
        /// </summary>
        public float OffsetForward { get; set; }

        /// <summary>
        ///     Desplazamiento final que se le hace al target para acomodar la camara en un cierto
        ///     rincon de la pantalla
        /// </summary>
        public TGCVector3 TargetDisplacement { get; set; }

        /// <summary>
        ///     Rotacion absoluta en Y de la camara
        /// </summary>
        public float RotationY { get; set; }

        /// <summary>
        ///     Objetivo al cual la camara tiene que apuntar
        /// </summary>
        public TGCVector3 Target { get; set; }

        public override void UpdateCamera(float elapsedTime)
        {
            TGCVector3 targetCenter;
            CalculatePositionTarget(out position, out targetCenter);
            SetCamera(position, targetCenter);
        }

        /// <summary>
        ///     Carga los valores default de la camara y limpia todos los cálculos intermedios
        /// </summary>
        public void resetValues()
        {
            OffsetHeight = 20;
            OffsetForward = -120;
            RotationY = 0;
            TargetDisplacement = TGCVector3.Empty;
            Target = TGCVector3.Empty;
            position = TGCVector3.Empty;
        }

        /// <summary>
        ///     Configura los valores iniciales de la cámara
        /// </summary>
        /// <param name="target">Objetivo al cual la camara tiene que apuntar</param>
        /// <param name="offsetHeight">Desplazamiento en altura de la camara respecto del target</param>
        /// <param name="offsetForward">Desplazamiento hacia adelante o atras de la camara repecto del target.</param>
        public void setTargetOffsets(TGCVector3 target, float offsetHeight, float offsetForward)
        {
            Target = target;
            OffsetHeight = offsetHeight;
            OffsetForward = offsetForward;
        }

        /// <summary>
        ///     Genera la proxima matriz de view, sin actualizar aun los valores internos
        /// </summary>
        /// <param name="pos">Futura posicion de camara generada</param>
        /// <param name="targetCenter">Futuro centro de camara a generada</param>
        public void CalculatePositionTarget(out TGCVector3 pos, out TGCVector3 targetCenter)
        {
            //alejarse, luego rotar y lueg ubicar camara en el centro deseado
            targetCenter = TGCVector3.Add(Target, TargetDisplacement);
            var m = TGCMatrix.Translation(0, OffsetHeight, OffsetForward) * TGCMatrix.RotationY(RotationY) * TGCMatrix.Translation(targetCenter);

            //Extraer la posicion final de la matriz de transformacion
            pos = new TGCVector3(m.M41, m.M42, m.M43);
        }

        /// <summary>
        ///     Rotar la camara respecto del eje Y
        /// </summary>
        /// <param name="angle">Ángulo de rotación en radianes</param>
        public void rotateY(float angle)
        {
            RotationY += angle;
        }
    }
}
