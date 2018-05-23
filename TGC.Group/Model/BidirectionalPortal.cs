using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Mathematica;

namespace TGC.Group.Model
{
    class BidirectionalPortal : TypeOfPortal
    {
        private Portal targetPortal;

        public BidirectionalPortal(Portal originPortal, Portal targetPortal) : base(originPortal)
        {
            this.targetPortal = targetPortal;
        }

        override public TGCVector3 GetTargetPosition()
        {
            return targetPortal.GetPosition();
        }

        public override void Dispose()
        {
            base.Dispose();
            this.targetPortal.Dispose();
        }

        public override void Collide(Vehiculo car)
        {
            //no implementado
            return;
        }

    }
}
