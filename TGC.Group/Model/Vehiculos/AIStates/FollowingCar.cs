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
            var cuadrante = GetCuadrante(enemy.GetPosition());
            var movState = AI.GetEstado();
            switch(cuadrante)
            {
                case 0:
                    movState.Advance();
                    movState.Right();
                    break;
                case 1:
                    movState.Advance();
                    movState.Left();
                    break;
                case 2:
                    movState.Back();
                    movState.Right();
                    break;
                case 3:
                    movState.Back();
                    movState.Left();
                    break;
                default: break;
            }
        }

        override public void Run()
        {
            this.EnemySpotted();
            return;
        }

    }
}
