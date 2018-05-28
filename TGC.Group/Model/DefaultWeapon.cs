using System;
using TGC.Core.Mathematica;

namespace TGC.Group.Model
{
    class DefaultWeapon : Weapon, IShootable
    {

        private static MeshObb projectile;

        public DefaultWeapon()
        {
            projectile = new MeshObb(Core.Geometry.TGCBox.fromSize(new TGCVector3(0.01f, 0.5f, 0.01f)).ToMesh("projectile"));
        }

        public override TGCMatrix getShotMeshPosition(Projectile p)
        {
            TGCVector3 ip = p.getInitialPosition();
            TGCVector3 isd = p.getInitialSpeedDirection();
            float timeSinceShot = p.getTimeSinceShot();

            TGCMatrix rotate;
            TGCMatrix translate;
            float projectileSpeed = 100f;

            rotate = TGCMatrix.RotationYawPitchRoll(0, FastMath.PI_HALF, 0);

            if (isd.X <= 0)
            {
                rotate = rotate * (OperationsWithVectors.rotacionEntreVectores(new TGCVector3(0f, 0f, 1f), new TGCVector3(-isd.X, 0, -isd.Z))) * TGCMatrix.RotationYawPitchRoll(FastMath.PI, 0, 0);
            }
            else
            {
                rotate = rotate * (OperationsWithVectors.rotacionEntreVectores(new TGCVector3(0f, 0f, 1f), new TGCVector3(isd.X, 0, isd.Z)));
            }

            TGCVector3 finalPos = ip + isd * projectileSpeed * timeSinceShot;
            translate = TGCMatrix.Translation(finalPos);

            return rotate * translate;
        }

        public override void Dispose()
        {
            base.Dispose();
            projectile.Dispose();
        }

        public override MeshObb getMeshOBB()
        {
            return projectile;
        }

        public override void setMeshOBB(MeshObb mesh)
        {

            throw new NotImplementedException();
        }

        protected override TGCMatrix GetHeight()
        {
            //Esta arma nunca estará en el mapa, no se renderiza. 
            throw new NotImplementedException();
        }
    }
}
