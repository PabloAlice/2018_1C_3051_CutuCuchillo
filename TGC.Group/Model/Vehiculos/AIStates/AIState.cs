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

        virtual public int GetCuadrante(TGCVector3 testPoint)
        {
            var global = GlobalConcepts.GetInstance();
            if(global.IsInFrontOf(testPoint, AI.planoCostado) && global.IsInFrontOf(testPoint, AI.directionPlane))
            {
                return 0;
            }
            if(global.IsInFrontOf(testPoint, AI.planoCostado) && !global.IsInFrontOf(testPoint, AI.directionPlane))
            {
                return 1;
            }
            if(!global.IsInFrontOf(testPoint, AI.planoCostado) && !global.IsInFrontOf(testPoint, AI.directionPlane))
            {
                return 2;
            }
            if(!global.IsInFrontOf(testPoint, AI.planoCostado) && global.IsInFrontOf(testPoint, AI.directionPlane))
            {
                return 3;
            }
            return 0;
        }

        abstract public void Run();
    }
}
