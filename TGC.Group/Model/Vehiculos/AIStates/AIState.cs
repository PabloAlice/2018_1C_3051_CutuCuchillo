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

        virtual public Quadrant GetCuadrante(TGCVector3 testPoint)
        {
            GlobalConcepts global = GlobalConcepts.GetInstance();
            if (global.IsInFrontOf(testPoint, AI.planoCostado) && TgcCollisionUtils.testPlaneAABB(AI.directionPlane, Scene.GetInstance().auto.mesh.BoundingBox))
            {
                return new QuadranInFrontOf(this.AI.GetEstado());
            }
            if(global.IsInFrontOf(testPoint, AI.planoCostado) && global.IsInFrontOf(testPoint, AI.directionPlane))
            {
                return new QuadrantTopRight(this.AI.GetEstado());
            }
            if(global.IsInFrontOf(testPoint, AI.planoCostado) && !global.IsInFrontOf(testPoint, AI.directionPlane))
            {
                return new QuadrantTopLeft(this.AI.GetEstado());
            }
            if(!global.IsInFrontOf(testPoint, AI.planoCostado) && !global.IsInFrontOf(testPoint, AI.directionPlane))
            {
                return new QuadrantBottomLeft(this.AI.GetEstado());
            }
            if(!global.IsInFrontOf(testPoint, AI.planoCostado) && global.IsInFrontOf(testPoint, AI.directionPlane))
            {
                return new QuadrantBottomRight(this.AI.GetEstado());
            }
            throw new Exception("No se encuentra en ningun cuadrante");
        }

        abstract protected void DeterminateState();

        virtual public void Run()
        {
            this.DeterminateState();
        }
    }
}
