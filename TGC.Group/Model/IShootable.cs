using TGC.Core.Mathematica;

namespace TGC.Group.Model
{
    interface IShootable
    {

        TGCMatrix getShotMeshPosition(Projectile p);

        void addProjectile(Projectile p);

        void renderProjectiles();

        MeshObb getMeshOBB();
    }
}
