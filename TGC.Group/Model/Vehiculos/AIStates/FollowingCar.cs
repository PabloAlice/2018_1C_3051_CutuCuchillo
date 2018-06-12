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
            var AIFrontVector = this.AI.GetVectorAdelante();
            var AIRay = new TgcRay
            {
                Origin = AI.GetPosition(),
                Direction = AIFrontVector
            };
            
            var collisionPoint = new TGCVector3();
            if (TgcCollisionUtils.intersectRayAABB(AIRay, enemy.mesh.BoundingBox, out collisionPoint))
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
            this.EnemySpotted();
            return;
        }

    }
}
