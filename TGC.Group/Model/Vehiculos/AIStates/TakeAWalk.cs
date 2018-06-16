using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGC.Group.Model.Vehiculos.AIStates
{
    class TakeAWalk : AIState
    {
        public TakeAWalk(ArtificialIntelligence AI) : base(AI)
        {

        }

        override protected void DeterminateState()
        {
            Vehicle car = Scene.GetInstance().auto;
            if (this.AI.IsEnemyInRadar(car))
            {
                this.AI.ChangeState(new FollowingCar(this.AI));
            }
        }

        public override void Run()
        {

        }
    }
}
