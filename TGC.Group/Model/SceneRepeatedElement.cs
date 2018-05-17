using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;

namespace TGC.Group.Model
{
    class SceneRepeatedElement : SceneElement
    {
        public SceneRepeatedElement(List<TgcMesh> elementos, TGCMatrix transformacion) : base(elementos, transformacion)
        {

        }
    }
}
