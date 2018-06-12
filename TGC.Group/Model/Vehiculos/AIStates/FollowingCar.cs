using TGC.Core.Collision;
using TGC.Core.Mathematica;

namespace TGC.Group.Model.Vehiculos.AIStates
{
    class FollowingCar : AIState
    {
        public FollowingCar(ArtificialIntelligence AICar) : base(AICar)
        {
        }

        override public void EnemySpotted()
        {
            Vehicle enemy = Scene.GetInstance().auto;
            if (TgcCollisionUtils.testPlaneAABB(AI.directionPlane, enemy.mesh.BoundingBox) && GlobalConcepts.GetInstance().IsInFrontOf(enemy.GetPosition(), AI.planoCostado))
            {
                this.AI.GetEstado().Advance();
            }
            else
            {
                this.AI.GetEstado().Advance();
                this.AI.GetEstado().Left();
            }
        }

        override public void Run()
        {
            this.EnemySpotted();
            return;
        }

    }
}
