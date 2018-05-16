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
        private List<Objeto> objetos = new List<Objeto>();
        private TGCVector3 puntoMinimo, puntoMaximo;

        public Seccion(TGCVector3 puntoMinimo, TGCVector3 puntoMaximo)
        {
            this.puntoMinimo = puntoMinimo;
            this.puntoMaximo = puntoMaximo;
        }

        public TGCVector3 GetPuntoMinimo()
        {
            return this.puntoMinimo;
        }

        public TGCVector3 GetPuntoMaximo()
        {
            return this.puntoMaximo;
        }

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

        public void Dispose()
        {
            foreach (Objeto objeto in this.objetos)
            {
                objeto.Dispose();
            }
        }
    }
}
