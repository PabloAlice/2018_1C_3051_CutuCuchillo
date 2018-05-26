using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Mathematica;
using TGC.Core.BoundingVolumes;

namespace TGC.Group.Model
{
    interface Collidable
    {
        void HandleCollisions(Vehiculo car);
        void Render();
        void Dispose();
        TGCVector3 GetPosition();
        TgcBoundingAxisAlignBox GetBoundingAlignBox();
    }
}
