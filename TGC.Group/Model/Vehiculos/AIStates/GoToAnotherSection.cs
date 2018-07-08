using System.Collections.Generic;
using System.Linq;

namespace TGC.Group.Model.Vehiculos.AIStates
{
    class GoToAnotherSection : AIState
    {
        public GoToAnotherSection(ArtificialIntelligence AI) : base(AI)
        {

        }

        override public void Run()
        {
            Section mySection = Scene.GetInstance().VehicleUbication(AI);
            Section nextSection = mySection.NextSection();
            List<TypeOfPortal> portals = mySection.GetPortalsThatGoTo(nextSection);
            TypeOfPortal portal = this.SelectTheNearest(portals);
            Quadrant quadrant = this.GetCuadrante(portal.GetPosition());
            quadrant.Execute();
        }

        private TypeOfPortal SelectTheNearest(List<TypeOfPortal> portals)
        {
            GlobalConcepts g = GlobalConcepts.GetInstance();
            portals.Sort((p1, p2) => g.DistanceBetweenTwoPoints(this.AI.GetPosition(), p1.GetPosition()).CompareTo(g.DistanceBetweenTwoPoints(this.AI.GetPosition(), p2.GetPosition())));
            return portals.First();
        }
    }
}
