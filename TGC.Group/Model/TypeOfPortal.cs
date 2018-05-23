using TGC.Core.Mathematica;
using TGC.Core.BoundingVolumes;

namespace TGC.Group.Model
{
    abstract class TypeOfPortal
    {
        protected Portal originPortal;

        public TypeOfPortal(Portal originPortal)
        {
            this.originPortal = originPortal;
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

        abstract public void Collide(Vehiculo car);

        /*
        public void checkCollision(Vehiculo car)
        {
            foreach(TgcMesh mesh in meshes)
            {
                mesh.Transform = transformIN;
                if(car.mesh.BoundingBox.)

                mesh.Transform = transformOUT;
            }
        }

        private void Move(TGCMatrix transformation)
        {
                portal.AutoTransform = false;
                portal.Transform = transformation;
                portal.BoundingBox.transform(transformation);
        }

        private void CheckPortals(Vehiculo car)
        {
            foreach(UnidirectionalPortal portal in portals)
            {
                Move(portal.GetTransformation());
                if (!AreDistantPositions(this.portal.Position, car.GetPosicion()))
                {
                    if (Collide(car))
                    {
                        car.Move(portal.getTargetPosition());
                        car.Rotate(portal.getTargetRotation());
                    }
                }

            }
        }

        private Boolean Collide(Vehiculo car)
        {
            return false;
        }

        private Boolean AreDistantPositions(TGCVector3 posA, TGCVector3 posB)
        {
            float distX = Math.Abs(posA.X - posB.X);
            float distZ = Math.Abs(posA.Z - posB.Z);

            return distX > 10f || distZ > 10f ? true : false;
        }

        public void Render()
        {
            foreach(UnidirectionalPortal portal in this.portals)
            {
                this.RenderPortal(portal);
            }
        }

        private void RenderPortal(UnidirectionalPortal portal)
        {
                this.portal.AutoTransform = false;
                this.portal.Transform = portal.GetTransformation();
                portal.Rotate(TGCMatrix.RotationZ(0.05f));
                this.portal.Render();
        }

        public void Dispose()
        {
            this.portal.Dispose();
        }
        */
    }
}
