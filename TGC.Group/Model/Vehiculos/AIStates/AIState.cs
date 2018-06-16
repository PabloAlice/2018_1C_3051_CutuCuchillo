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

        virtual protected void DeterminateState()
        {
            Vehicle car = Scene.GetInstance().auto;
            if (this.AI.IsEnemyInRadar(car))
            {
                this.AI.ChangeState(new FollowingCar(this.AI));
            }
            else if (!this.AI.DoIHaveEnoughWeapons())
            {
                this.AI.ChangeState(new SearchWeapons(this.AI));
            }
            else
            {
                this.AI.ChangeState(new TakeAWalk(this.AI));
            }
        }

        virtual public void Run()
        {
            this.DeterminateState();
        }
    }
}
