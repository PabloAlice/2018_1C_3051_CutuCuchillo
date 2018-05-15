using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Mathematica;

namespace TGC.Group.Model
{
    class Seccion
    {
        private List<Objeto> objetos;
        private TGCVector3 limite1, limite2, limite3, limite4;

        public void Add(Objeto objeto)
        {
            this.objetos.Add(objeto);
        }

        public void Render()
        {
            foreach (Objeto objeto in this.objetos)
            {
                objeto.Render();
            }
        }
    }
}
