using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGC.Group.Model.Vehiculos.AIStates
{
    class Stopped : AIState
    {
        float elapsedTime = 0f;

        public Stopped(ArtificialIntelligence AICar) : base(AICar)
        {
        }

        protected override void DeterminateState()
        {
            this.elapsedTime += GlobalConcepts.GetInstance().GetElapsedTime();
            if(this.elapsedTime >= 10f)
            {
                this.AI.ChangeState(new FollowingCar(this.AI));
            }
        }

        override public void Run()
        {
            this.DeterminateState();
            return;
        }
    }
}
