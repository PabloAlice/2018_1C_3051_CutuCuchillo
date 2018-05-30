using TGC.Core.Mathematica;
using TGC.Core.Sound;
using System.Collections.Generic;
using TGC.Core.Collision;
using TGC.Core.BoundingVolumes;
using System;

namespace TGC.Group.Model.Vehiculos.Estados
{
    class Descending : EstadoVehiculo
    {
        private float initialSpeed;

        public Descending(Vehicle auto, float initialSpeed) : base(auto)
        {
            this.initialSpeed = initialSpeed;
        }

        public override TGCVector3 GetCarDirection()
        {
            return new TGCVector3(0,-1,0);
        }

        public override void Advance()
        {
            if (auto.GetVelocidadActual() < 0)
            {
                auto.SetVelocidadActual(auto.GetVelocidadActual() + auto.GetConstanteFrenado() * 2);
                if (auto.GetVelocidadActual() > 0)
                {
                    auto.SetVelocidadActual(0);
                    auto.GetDeltaTiempoAvance().resetear();
                }
                return;
            }
            base.Advance();

        }

        public override void Back()
        {
            if (auto.GetVelocidadActual() > 0)
            {
                auto.SetVelocidadActual(auto.GetVelocidadActual() - auto.GetConstanteFrenado() * 2);
                if (auto.GetVelocidadActual() < 0)
                {
                    auto.SetVelocidadActual(0);
                    auto.GetDeltaTiempoAvance().resetear();
                }
                return;
            }
            base.Back();
        }

        public override void Jump()
        {
            return;
        }

        public override void JumpUpdate()
        {
            auto.SetVelocidadActualDeSalto(this.VelocidadFisicaDeSalto());
            float desplazamientoEnY = auto.GetVelocidadActualDeSalto() * auto.GetElapsedTime();
            TGCVector3 nuevoDesplazamiento = new TGCVector3(0, desplazamientoEnY, 0);
            this.Move(nuevoDesplazamiento + auto.VectorAdelanteSalto * this.initialSpeed * auto.GetElapsedTime());
            if(this.IsCollidingWithFloor())
            {
                auto.GetDeltaTiempoSalto().resetear();
                auto.SetVelocidadActualDeSalto(auto.GetVelocidadMaximaDeSalto());
                this.auto.SoundsManager.Drop();
                if (auto.GetVelocidadActual() > 0)
                {
                    this.auto.SetEstado(new Forward(this.auto));
                }
                else if(auto.GetVelocidadActual() < 0)
                {
                    this.auto.SetEstado(new Backward(this.auto));
                }
                else
                {
                    this.auto.SetEstado(new Stopped(this.auto));
                }
            }
        }

        private bool IsCollidingWithFloor()
        {
            List<Collidable> list = Scene.GetInstance().GetPosiblesCollidables();
            System.Console.WriteLine("voy a ver si es el piso");
            foreach (Collidable element in list)
            {
                System.Console.WriteLine("recorriendo la lista");
                if(TgcCollisionUtils.testObbAABB(this.auto.GetTGCBoundingOrientedBox(), element.GetBoundingAlignBox()) || this.auto.GetPosicion().Y < 0)
                {
                    System.Console.WriteLine("colisione con algo");
                    return this.IsReallyTheFloor(element);
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
            foreach(TGCPlane plane in planes)
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

        private bool IsReallyTheFloor(Collidable element)
        {
            TGCVector3 directionOfCollision = new TGCVector3(0, -1, 0);
            TgcRay ray = new TgcRay();
            ray.Origin = this.auto.GetLastPosition();
            System.Console.WriteLine("POSICION: ({0};{1};{2})", ray.Origin.X, ray.Origin.Y, ray.Origin.Z);
            ray.Direction = directionOfCollision;
            TgcBoundingAxisAlignBox.Face[] faces;
            faces = element.GetBoundingAlignBox().computeFaces();
            TGCPlane plane = this.CreatePlane(ray, faces, this.auto.GetLastPosition());
            TGCVector3 normal = GlobalConcepts.GetInstance().GetNormalPlane(plane);
            System.Console.WriteLine("//////////////////////////////////////////////////////");
            System.Console.WriteLine("//////////////////////////////////////////////////////");
            System.Console.WriteLine("Normal: ({0},{1},{2})", normal.X, normal.Y, normal.Z);
            System.Console.WriteLine("//////////////////////////////////////////////////////");
            System.Console.WriteLine("//////////////////////////////////////////////////////");
            if (normal == new TGCVector3(0,1,0))
            {
                return false;
            }
            while (TgcCollisionUtils.testObbAABB(this.auto.GetTGCBoundingOrientedBox(), element.GetBoundingAlignBox()) || this.auto.GetPosicion().Y <=0)
            {
                this.auto.Translate(TGCMatrix.Translation(-directionOfCollision * 0.1f));
                this.auto.Transform();
            }
            return true;
        }

        public override void Left()
        {
            this.auto.RotarDelanteras(-this.auto.GetVelocidadDeRotacion() * this.auto.GetElapsedTime());
            float rotacionReal = auto.GetVelocidadDeRotacion() * auto.GetElapsedTime();
            rotacionReal = (auto.GetVelocidadActual() < 0) ? rotacionReal : -rotacionReal;
            this.auto.Girar(rotacionReal);
        }

        public override void Right()
        {
            this.auto.RotarDelanteras(this.auto.GetVelocidadDeRotacion() * this.auto.GetElapsedTime());
            float rotacionReal = auto.GetVelocidadDeRotacion() * auto.GetElapsedTime();
            rotacionReal = (auto.GetVelocidadActual() > 0) ? rotacionReal : -rotacionReal;
            this.auto.Girar(rotacionReal);
        }

        public override void UpdateWheels()
        {
            return;
        }
    }
}
