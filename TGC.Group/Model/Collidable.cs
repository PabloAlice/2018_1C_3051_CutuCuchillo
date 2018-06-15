using TGC.Core.BoundingVolumes;
using TGC.Core.SceneLoader;
using TGC.Core.Mathematica;

namespace TGC.Group.Model
{
    interface Collidable
    {
        void HandleCollisions(Vehicle car);
        bool IsColliding(Weapon weapon, out Collidable element);
        void Render();
        void Dispose();
        TGCVector3 GetPosition();
        TgcMesh GetCollidable(Vehicle car);
        bool IsInView();
        //bool IsInto(TGCVector3 minPoint, TGCVector3 maxPoint);
    }
}
