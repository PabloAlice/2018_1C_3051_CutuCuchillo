using System;
using TGC.Core.Mathematica;
using TGC.Core.Sound;
using TGC.Core.Collision;
using TGC.Core.SceneLoader;
using System.Collections.Generic;
using TGC.Core.BoundingVolumes;
using TGC.Core.Geometry;

namespace TGC.Group.Model.Vehiculos.Estados
{
    abstract class EstadoVehiculo
    {
        protected Vehicle auto;

        abstract public TGCVector3 GetCarDirection();

        public EstadoVehiculo(Vehicle auto)
        {
            this.auto = auto;
        }

        public override string ToString()
        {
            return this.GetType().Name;
        }

        virtual public void JumpUpdate()
        {
            float desplazamientoEnY = -0.3f;
            TGCVector3 nuevoDesplazamiento = new TGCVector3(0, desplazamientoEnY, 0);
            this.Move(nuevoDesplazamiento);
            var posiblePiso = this.GetColliding();
            if (posiblePiso == null)
            {
                this.auto.GetDeltaTiempoSalto().resetear();
                this.auto.GetDeltaTiempoSalto().acumularTiempo(this.auto.GetElapsedTime());
                this.auto.SetVelocidadActualDeSalto(0);
                this.auto.SetEstado(new Jumping(this.auto));
            }
            else
            {
                if (IsReallyTheFloor(posiblePiso))
                {
                    this.Move(-nuevoDesplazamiento);
                }
            }

            /*{
                auto.GetDeltaTiempoSalto().resetear();
                auto.SetVelocidadActualDeSalto(auto.GetVelocidadMaximaDeSalto());
                this.auto.SoundsManager.Drop();
                if (auto.GetVelocidadActual() > 0)
                {
                    this.auto.SetEstado(new Forward(this.auto));
                }
                else if (auto.GetVelocidadActual() < 0)
                {
                    this.auto.SetEstado(new Backward(this.auto));
                }
                else
                {
                    this.auto.SetEstado(new Stopped(this.auto));
                }
            }*/
        }

        protected TgcMesh GetColliding()
        {
            List<Collidable> list = Scene.GetInstance().GetPosiblesCollidables();
            foreach (Collidable element in list)
            {
                var meshito = element.GetCollidable(this.auto);
                if(meshito != null)
                {
                    return meshito;
                }

            }
            return null;
        }

        private TGCPlane SelectPlane(List<TGCPlane> planes, TGCVector3 testPoint)
        {
            planes.Sort((x, y) => (int)TgcCollisionUtils.distPointPlane(testPoint, y) - (int)TgcCollisionUtils.distPointPlane(testPoint, x));
            return planes[0];
        }

        private TGCPlane CreatePlane(TgcRay ray, TgcBoundingAxisAlignBox.Face[] faces, TGCVector3 testPoint)
        {
            float instante;
            TGCVector3 intersection;
            List<TGCPlane> candidatesPlanes = new List<TGCPlane>();
            foreach (TgcBoundingAxisAlignBox.Face face in faces)
            {
                if (TgcCollisionUtils.intersectRayPlane(ray, face.Plane, out instante, out intersection))
                {
                    candidatesPlanes.Add(face.Plane);
                }
            }

            return this.SelectPlane(candidatesPlanes, testPoint);

        }

        protected bool IsReallyTheFloor(TgcMesh element)
        {
            TGCVector3 directionOfCollision = new TGCVector3(0, -1f, 0);
            TgcRay ray = new TgcRay();
            ray.Origin = this.auto.GetLastPosition();
            ray.Direction = directionOfCollision;
            TgcBoundingAxisAlignBox.Face[] faces;
            faces = element.BoundingBox.computeFaces();
            TGCPlane plane = this.CreatePlane(ray, faces, this.auto.GetLastPosition());
            TGCVector3 normal = new TGCVector3(0, 1, 0);
            if (normal != new TGCVector3(0, 1, 0))
            {
                return false;
            }
            return true;
        }

        virtual public void Advance()
        {
            auto.GetDeltaTiempoAvance().acumularTiempo(auto.GetElapsedTime());
            auto.SetVelocidadActual(auto.VelocidadFisica());
            return;
        }

        virtual public void Back()
        {
            auto.GetDeltaTiempoAvance().acumularTiempo(auto.GetElapsedTime());
            auto.SetVelocidadActual(auto.VelocidadFisicaRetroceso());
            return;
        }

        virtual public void Jump()
        {
            this.auto.GetDeltaTiempoSalto().acumularTiempo(auto.GetElapsedTime());
            this.auto.VectorAdelanteSalto = auto.vectorAdelante;
            this.auto.SetEstado(new Jumping(this.auto));
            return;
        }

        virtual public void SpeedUpdate()
        {
            return;
        }

        virtual public void Move(TGCVector3 desplazamiento)
        {
            this.auto.Move(desplazamiento);
        }

        protected float VelocidadFisicaDeSalto()
        {
            return auto.GetVelocidadActualDeSalto() + (-auto.GetAceleracionGravedad()) * auto.GetDeltaTiempoSalto().tiempoTranscurrido();
        }

        virtual public float Right()
        {
            float rotacionReal = auto.GetVelocidadDeRotacion() * auto.GetElapsedTime();
            rotacionReal = (auto.GetVelocidadActual() > 0) ? rotacionReal : -rotacionReal;
            this.auto.Girar(rotacionReal);
            return rotacionReal;

        }

        //lo mismo que arriba
        virtual public float Left()
        {
            float rotacionReal = auto.GetVelocidadDeRotacion() * auto.GetElapsedTime();
            rotacionReal = (auto.GetVelocidadActual() < 0) ? rotacionReal : -rotacionReal;
            this.auto.Girar(rotacionReal);
            return rotacionReal;
        }

        virtual public void UpdateWheels()
        {
            var rotacionReal = this.auto.GetVelocidadDeRotacion() * this.auto.GetElapsedTime();
            this.auto.UpdateFrontWheels(rotacionReal);
        }

        virtual public void FrozenTimeUpdate()
        {
            return;
        }
    }
}
