using System.Collections.Generic;
using System.Linq;

namespace TGC.Group.Model.Vehiculos.AIStates
{
    class FoundLife : AIState
    {
        public FoundLife(ArtificialIntelligence AI) : base(AI)
        {

        }

        override public void Run()
        {
            List<Life> lifes = Scene.GetInstance().GetLifes(this.AI);
            if (lifes.Count == 0)
            {
                AI.ChangeState(new GoToAnotherSection(AI));
                AI.aiState.Run();
                return;
            }
            Life life = this.SelectTheNearest(lifes);
            Quadrant quadrant = GetCuadrante(life.GetPosition());
            quadrant.Execute();
        }

        private Life SelectTheNearest(List<Life> lifes)
        {
            GlobalConcepts g = GlobalConcepts.GetInstance();
            lifes.Sort((l1, l2) => g.DistanceBetweenTwoPoints(this.AI.GetPosition(), l1.GetPosition()).CompareTo(g.DistanceBetweenTwoPoints(this.AI.GetPosition(), l2.GetPosition())));
            return lifes.First();
        }
    }
}