using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Collision;
using TGC.Core.Mathematica;

namespace TGC.Group.Model.Vehiculos.AIStates
{
    abstract class AIState
    {
        protected ArtificialIntelligence AI;

        public AIState(ArtificialIntelligence AI)
        {
            this.AI = AI;
        }

        virtual public void EnemySpotted()
        {
            return;
        }

        virtual public bool TestRayAABB(TGC.Core.Collision.TgcRay ray, TGC.Core.BoundingVolumes.TgcBoundingAxisAlignBox aabb)
        {
            var planes = aabb.computeFaces();
            float fout;
            TGCVector3 collisionPoint = new TGCVector3();
            foreach(var plane in planes)
            {
                
                if(TgcCollisionUtils.intersectRayPlane(ray, plane.Plane, out fout, out collisionPoint))
                {
                    return true;
                }
            }
            return false;
        }

        abstract public void Run();
    }
}
