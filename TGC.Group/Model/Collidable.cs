using TGC.Core.BoundingVolumes;
using TGC.Core.SceneLoader;
using TGC.Core.Mathematica;
using TGC.Core.Collision;

namespace TGC.Group.Model
{
    interface Collidable
    {
        void HandleCollisions(Vehicle car);
        bool IsColliding(Weapon weapon, out Collidable element);
        bool IsColliding(Vehicle car);
        void Render();
        void Dispose();
        TGCVector3 GetPosition();
        TgcMesh GetCollidable(Vehicle car);
        TGCPlane GetPlaneOfCollision(TgcRay ray, Vehicle car);
        bool IsInto(TGCVector3 minPoint, TGCVector3 maxPoint);
    }
}
