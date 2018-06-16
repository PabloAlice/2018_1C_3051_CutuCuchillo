using TGC.Group.Model.Vehiculos.Estados;

namespace TGC.Group.Model.Vehiculos.AIStates
{
    class FollowingCar : AIState
    {
        public FollowingCar(ArtificialIntelligence AICar) : base(AICar)
        {
        }

        override protected void DeterminateState()
        {
            Vehicle car = Scene.GetInstance().auto;
            if (!this.AI.IsEnemyInRadar(car)) {
                if (!this.AI.DoIHaveEnoughWeapons())
                {
                    this.AI.ChangeState(new SearchWeapons(this.AI));
                }
                else
                {
                    this.AI.ChangeState(new TakeAWalk(this.AI));
                }
            }
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
