using TGC.Core.Mathematica;

namespace TGC.Group.Model
{
    interface IShootable
    {
        bool HasRemainingProjectiles();

        TGCMatrix getShotMeshPosition(Projectile p);

        void addProjectile(Projectile p);

        void renderProjectiles();

        MeshObb getMeshOBB();
    }
}
