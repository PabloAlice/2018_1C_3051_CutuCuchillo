using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGC.Group.Model
{
    interface Collidable
    {
        void HandleCollisions(Vehiculo car);
        void Render();
        void Dispose();
    }
}
