using TGC.Core.Mathematica;

namespace TGC.Group.Model
{
    interface IShootable
    {
        bool HasRemainingProjectiles();

        TGCMatrix getShotMeshTransformation(Projectile p);
        TGCVector3 getShotMeshPosition(Projectile p);
        void updateProjectiles();
        void addProjectile(Projectile p);

        void renderProjectiles();

        MeshObb getMeshOBB();
    }
}
