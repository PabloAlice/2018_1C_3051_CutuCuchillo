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
            var AIPlane = new TGCPlane();
            AIPlane = TGCPlane.FromPointNormal(AI.GetPosition(), TGCVector3.Cross(AI.GetVectorAdelante(), TGCVector3.Up));
            if (TgcCollisionUtils.testPlaneAABB(AIPlane, enemy.mesh.BoundingBox))
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
