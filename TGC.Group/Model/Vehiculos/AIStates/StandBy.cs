using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGC.Group.Model.Vehiculos.AIStates
{
    class StandBy : AIState
    {
        public StandBy(ArtificialIntelligence AICar) : base(AICar)
        {
        }

        override public void EnemySpotted(Vehicle enemyCar)
        {
            this.AI.aiState = new FollowingCar(this.AI);
        }
    }
}
