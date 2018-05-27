using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Mathematica;

namespace TGC.Group.Model
{
    class TransformationVector
    {
        TGCVector3 scalation, rotation, translation;

        public TransformationVector(TGCVector3 scalation, TGCVector3 rotation, TGCVector3 translation)
        {
            this.scalation = scalation;
            this.rotation = rotation;
            this.translation = translation;
        }

        public TransformationVector()
        {
            this.scalation = new TGCVector3(1,1,1);
            this.rotation = new TGCVector3(0,0,0);
            this.translation = new TGCVector3(0,0,0);
        }

        public TGCVector3 GetScalation()
        {
            return this.scalation;
        }

        public TGCVector3 GetRotation()
        {
            return this.rotation;
        }

        public TGCVector3 GetTranslation()
        {
            return this.translation;
        }
    }
}
