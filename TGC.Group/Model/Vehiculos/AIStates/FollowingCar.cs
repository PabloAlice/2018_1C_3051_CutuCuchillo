using TGC.Group.Model.Vehiculos.Estados;

namespace TGC.Group.Model.Vehiculos.AIStates
{
    class FollowingCar : AIState
    {
        public FollowingCar(ArtificialIntelligence AICar) : base(AICar)
        {
        }

        override public void Run()
        {
            base.Run();
            Vehicle enemy = Scene.GetInstance().auto;
            Quadrant quadrant = GetCuadrante(enemy.GetPosition());
            quadrant.Execute();
        }

    }
}
