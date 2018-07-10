using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Mathematica;

namespace TGC.Group.Model.Vehiculos.Estados
{
    class Dead : EstadoVehiculo
    {

        public Dead(Vehicle auto) : base(auto)
        {
        }

        public override TGCVector3 GetCarDirection()
        {
            return this.auto.GetVectorAdelante();
        }

        public override void Advance()
        {
            return;
        }

        public override void Back()
        {
            return;
        }

        public override void Jump()
        {
            return;
        }

        public override void Move(TGCVector3 desplazamiento)
        {
            return;
        }

        public override float Right()
        {
            return 0;
        }

        public override float Left()
        {
            return 0;
        }

        public override void FrozenTimeUpdate()
        {
        }

        public override void JumpUpdate()
        {
        }
    }
}
