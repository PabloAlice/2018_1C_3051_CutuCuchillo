using TGC.Core.BoundingVolumes;

namespace TGC.Group.Model
{
    interface Collidable
    {
        void HandleCollisions(Vehicle car);
        void Render();
        void Dispose();
        TgcBoundingAxisAlignBox GetBoundingAlignBox();
    }
}
