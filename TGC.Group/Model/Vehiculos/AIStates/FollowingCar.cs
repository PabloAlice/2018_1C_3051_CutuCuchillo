using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGC.Group.Model.Vehiculos.AIStates
{
    class FollowingCar : AIState
    {
        public FollowingCar(ArtificialIntelligence AICar) : base(AICar)
        {
        }

        override public void EnemySpotted(Vehicle enemyCar)
        {
            this.AI.GetEstado().Advance();
        }

        override public void Run()
        {
            return;
        }

    }
}
