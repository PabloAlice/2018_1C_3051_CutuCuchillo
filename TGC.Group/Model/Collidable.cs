using TGC.Core.BoundingVolumes;
using TGC.Core.SceneLoader;

namespace TGC.Group.Model
{
    interface Collidable
    {
        void HandleCollisions(Vehicle car);
        void Render();
        void Dispose();
        TgcMesh GetCollidable(Vehicle car);
    }
}
