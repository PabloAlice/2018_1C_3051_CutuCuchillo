using TGC.Group.Model.Vehiculos.Estados;

namespace TGC.Group.Model.Vehiculos.AIStates
{
    abstract class Quadrant
    {
        protected EstadoVehiculo state;

        public Quadrant(EstadoVehiculo state)
        {
            this.state = state;
        }

        abstract public void Execute();
    }
}
