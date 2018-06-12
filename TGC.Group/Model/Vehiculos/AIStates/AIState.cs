using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Collision;

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
            return false;
        }

        abstract public void Run();
    }
}
