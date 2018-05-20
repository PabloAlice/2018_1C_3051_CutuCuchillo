using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Core.Geometry;

namespace TGC.Group.Model.Vehiculos
{
    class Rueda
    {
        //public TGCMatrix transformacion;
        public TgcMesh mesh;
        public TGCMatrix trasladoInicial;
        public TGCMatrix rotationX = TGCMatrix.Identity;
        public TGCMatrix rotationY = TGCMatrix.Identity;
        public float anguloLimite = FastMath.QUARTER_PI;
        public TGCVector3 vectorAdelante = new TGCVector3(0, 0, 1);
        public TGCVector3 vectorCostado = TGCVector3.Cross(new TGCVector3(0,0,1), new TGCVector3(0,1,0));
        public TGCVector3 vectorLimiteIzquierdo = new TGCVector3(0, 0, 1);
        public TGCVector3 vectorLimiteDerecho = new TGCVector3(0, 0, 1);

        public Rueda(TgcMesh mesh,TGCVector3 traslado)
        {
            this.mesh = mesh;
            mesh.AutoTransform = false;
            trasladoInicial = TGCMatrix.Translation(traslado);
            vectorLimiteIzquierdo.TransformCoordinate(TGCMatrix.RotationY(-FastMath.QUARTER_PI));
            vectorLimiteDerecho.TransformCoordinate(TGCMatrix.RotationY(FastMath.QUARTER_PI));

        }

        public void Transform(TGCMatrix matrizAuto)
        {
            this.mesh.Transform = rotationX * rotationY * trasladoInicial * matrizAuto;
        }

        public void Render()
        {
            mesh.Render();
        }

        public void RotateY(float rotacion)
        {
            if (rotacion < 0)
            {
                var dot = TGCVector3.Dot(this.vectorAdelante, this.vectorLimiteDerecho);
                if (dot > 0)
                {
                    this.rotationY = TGCMatrix.RotationY(rotacion) * this.rotationY;
                    this.vectorAdelante.TransformCoordinate(TGCMatrix.RotationY(rotacion));
                }
            }
            else
            {
                var dot = TGCVector3.Dot(this.vectorLimiteIzquierdo, this.vectorAdelante);
                if(dot > 0)
                {
                    this.rotationY = TGCMatrix.RotationY(rotacion) * this.rotationY;
                    this.vectorAdelante.TransformCoordinate(TGCMatrix.RotationY(rotacion));
                }
            }
        }


        /// <summary>
        /// Rota la rueda sobre si misma
        /// </summary>
        public void RotateX(float velocidad)
        {
            this.rotationX = TGCMatrix.RotationX(-velocidad * 0.01f) * this.rotationX;
        }

        public void UpdateRotationY(float rotation)
        {
            //Si entra es porque la rueda está clavada en el centro, no hace nada.
            if (TGCVector3.Dot(vectorCostado, vectorAdelante) == 0){
                return;
            }
            var updatedRotation = this.CheckFrontVector(rotation);
            this.rotationY = TGCMatrix.RotationY(updatedRotation) * this.rotationY;
            this.vectorAdelante.TransformCoordinate(TGCMatrix.RotationY(updatedRotation));

            if(updatedRotation > 0 && TGCVector3.Dot(this.vectorCostado, this.vectorAdelante) < 0)
            {
                this.vectorAdelante = new TGCVector3(0, 0, 1);
                this.rotationY = TGCMatrix.Identity;
            }
        }

        private float CheckFrontVector(float rotation)
        {
            if(TGCVector3.Dot(this.vectorCostado, this.vectorAdelante) > 0)
            {
                return rotation;
            }
            else
            {
                return -rotation;
            }
        }

        public double FrontVectorAngle()
        {
            var dot = TGCVector3.Dot(this.vectorAdelante, new TGCVector3(0, 0, 1));
            var modulusProduct = this.vectorAdelante.Length() * new TGCVector3(0, 0, 1).Length();
            return Math.Acos(dot / modulusProduct);
        }
    }
}