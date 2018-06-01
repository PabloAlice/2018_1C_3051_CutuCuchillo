using System;
using TGC.Core.Mathematica;
using TGC.Core.Sound;
using TGC.Core.Collision;
using TGC.Core.SceneLoader;
using System.Collections.Generic;
using TGC.Core.BoundingVolumes;

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
            float desplazamientoEnY = -0.15f;
            System.Console.WriteLine("Posicion en Y: {0}", this.auto.GetPosicion().Y);
            TGCVector3 nuevoDesplazamiento = new TGCVector3(0, desplazamientoEnY, 0);
            System.Console.WriteLine("Lo voy a mover: {0}", desplazamientoEnY);
            this.Move(nuevoDesplazamiento);
            System.Console.WriteLine("Posicion en Y despues de mover: {0}", this.auto.GetPosicion().Y);
            if (!this.IsCollidingWithFloor())
            {
                System.Console.WriteLine("No colisione");
                this.auto.GetDeltaTiempoSalto().resetear();
                this.auto.GetDeltaTiempoSalto().acumularTiempo(this.auto.GetElapsedTime());
                this.auto.SetVelocidadActualDeSalto(0);
                this.auto.SetEstado(new Jumping(this.auto));
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

        protected bool IsCollidingWithFloor()
        {
            List<Collidable> list = Scene.GetInstance().GetPosiblesCollidables();
            foreach (Collidable element in list)
            {
                if (TgcCollisionUtils.testObbAABB(this.auto.GetTGCBoundingOrientedBox(), element.GetCollidable(this.auto).BoundingBox) || this.auto.GetPosicion().Y < 0)
                {
                    TgcMesh realMesh = element.GetCollidable(this.auto);
                    return this.IsReallyTheFloor(realMesh);
                }
            }
            return false;
        }

        private bool IsInFrontOf(TGCVector3 testpoint, TGCPlane plane)
        {
            return plane.A * testpoint.X + plane.B * testpoint.Y + plane.C * testpoint.Z + plane.D > 0;
        }

        private TGCPlane SelectPlane(List<TGCPlane> planes, TGCVector3 testPoint)
        {
            planes.Sort((x, y) => IsInFrontOf(testPoint, x).CompareTo(IsInFrontOf(testPoint, y)));
            planes.Reverse();
            foreach (TGCPlane plane in planes)
            {
                System.Console.WriteLine(")))))))))))))))))))))))))))))))))))))))))))))))))");
                System.Console.WriteLine(")))))))))))))))))))))))))))))))))))))))))))))))))");
                System.Console.WriteLine("Plano: ({0},{1},{2})", plane.A, plane.B, plane.C);
                System.Console.WriteLine(")))))))))))))))))))))))))))))))))))))))))))))))))");
                System.Console.WriteLine(")))))))))))))))))))))))))))))))))))))))))))))))))");
            }
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

        private bool IsReallyTheFloor(TgcMesh element)
        {
            TGCVector3 directionOfCollision = new TGCVector3(0, -1, 0);
            TgcRay ray = new TgcRay();
            ray.Origin = this.auto.GetLastPosition();
            System.Console.WriteLine("POSICION: ({0};{1};{2})", ray.Origin.X, ray.Origin.Y, ray.Origin.Z);
            ray.Direction = directionOfCollision;
            TgcBoundingAxisAlignBox.Face[] faces;
            faces = element.BoundingBox.computeFaces();
            TGCPlane plane = this.CreatePlane(ray, faces, this.auto.GetLastPosition());
            TGCVector3 normal = GlobalConcepts.GetInstance().GetNormalPlane(plane);
            System.Console.WriteLine("//////////////////////////////////////////////////////");
            System.Console.WriteLine("//////////////////////////////////////////////////////");
            System.Console.WriteLine("Normal: ({0},{1},{2})", normal.X, normal.Y, normal.Z);
            System.Console.WriteLine("//////////////////////////////////////////////////////");
            System.Console.WriteLine("//////////////////////////////////////////////////////");
            if (normal == new TGCVector3(0, 1, 0))
            {
                return false;
            }
            while (TgcCollisionUtils.testObbAABB(this.auto.GetTGCBoundingOrientedBox(), element.BoundingBox) || this.auto.GetPosicion().Y <= 0)
            {
                this.auto.Translate(TGCMatrix.Translation(-directionOfCollision * 0.1f));
                this.auto.Transform();
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

        virtual public void Right()
        {
            float rotacionReal = auto.GetVelocidadDeRotacion() * auto.GetElapsedTime();
            rotacionReal = (auto.GetVelocidadActual() > 0) ? rotacionReal : -rotacionReal;
            this.auto.Girar(rotacionReal);

        }

        //lo mismo que arriba
        virtual public void Left()
        {
            float rotacionReal = auto.GetVelocidadDeRotacion() * auto.GetElapsedTime();
            rotacionReal = (auto.GetVelocidadActual() < 0) ? rotacionReal : -rotacionReal;
            this.auto.Girar(rotacionReal);
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
