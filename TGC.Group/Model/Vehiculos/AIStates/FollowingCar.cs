using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Mathematica;

namespace TGC.Group.Model.Vehiculos.AIStates
{
    class FollowingCar : AIState
    {
        public FollowingCar(ArtificialIntelligence AICar) : base(AICar)
        {
        }

        override public void EnemySpotted(Vehicle enemyCar)
        {
            var AIFrontVector = this.AI.GetVectorAdelante();
            var AIRay = new TGC.Core.Collision.TgcRay();
            AIRay.Origin = AI.GetPosition();
            AIRay.Direction = AIFrontVector;
            var collisionPoint = new TGCVector3();
            if (TGC.Core.Collision.TgcCollisionUtils.intersectRayObb(AIRay, enemyCar.GetTGCBoundingOrientedBox(), out collisionPoint))
            {
                System.Console.WriteLine("colisione el rayo con el obb del enemigo");
                this.AI.GetEstado().Advance();
            }
            else
            {
            }
        }

        override public void Run()
        {
            return;
        }

    }
}
