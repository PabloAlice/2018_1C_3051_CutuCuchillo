﻿using TGC.Core.Mathematica;
using TGC.Core.BoundingVolumes;
using TGC.Core.Collision;
using TGC.Core.SceneLoader;
using System.Drawing;
using System;

namespace TGC.Group.Model
{
    abstract class TypeOfPortal : Collidable
    {
        protected Portal originPortal;
        protected TGCVector3 outDirection;

        public TypeOfPortal(Portal originPortal, TGCVector3 outDirection)
        {
            this.outDirection = outDirection;
            this.originPortal = originPortal;
        }

        public TgcMesh GetCollidable(Vehicle car)
        {
            return originPortal.mesh;
        }

        public TgcBoundingAxisAlignBox GetBoundingAlignBox()
        {
            return this.originPortal.mesh.BoundingBox;
        }

        abstract public TGCVector3 GetTargetPosition();

        virtual public void Dispose()
        {
            this.originPortal.Dispose();
        }

        public void Render()
        {
            this.originPortal.Render();
        }

        public TgcBoundingAxisAlignBox GetBoundingBox()
        {
            return this.originPortal.GetBoundingBox();
        }

        public void HandleCollisions(Vehicle car)
        {
            if (TgcCollisionUtils.testObbAABB(car.GetTGCBoundingOrientedBox(), this.GetBoundingBox())){
                this.GetBoundingBox().setRenderColor(Color.Red);
                this.Collide(car);
                car.SoundsManager.Teletransport();
            }
        }

        virtual public void Collide(Vehicle car)
        {
            var dot = TGCVector3.Dot(car.vectorAdelante, this.outDirection);
            var modulusProduct = car.vectorAdelante.Length() * this.outDirection.Length();
            var acos = (float)Math.Acos(dot / modulusProduct);
            var yCross = TGCVector3.Cross(car.vectorAdelante, this.outDirection).Y;
            car.Girar((yCross > 0) ? acos : -acos);
            if (car.GetVelocidadActual() < 0)
            {
                car.Girar(FastMath.PI);
            }
        }

        public TGCVector3 GetPosition()
        {
            return this.originPortal.GetPosition();
        }
    }
}
