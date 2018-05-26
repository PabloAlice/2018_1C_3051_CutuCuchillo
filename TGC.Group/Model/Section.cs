using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Mathematica;
using TGC.Core.Collision;
using TGC.Core.BoundingVolumes;
using System.Drawing;

namespace TGC.Group.Model
{
    class Section
    {
        private List<Collidable> objetos = new List<Collidable>();
        private TGCVector3 puntoMinimo, puntoMaximo;

        public Section(TGCVector3 puntoMinimo, TGCVector3 puntoMaximo)
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

        public void AddElements(Collidable objeto)
        {
            this.objetos.Add(objeto);
        }

        public void remove(Collidable objeto)
        {
            objetos.Remove(objeto);
        }

        public void Render(ThirdPersonCamera camara)
        {
            foreach (Collidable objeto in this.objetos)
            {
                objeto.Render();
            }
        }

        public void HandleCollisions(Vehiculo car)
        {
            foreach (Collidable objeto in objetos)
            {
                objeto.HandleCollisions(car);
            }

        }

        public void Dispose()
        {
            foreach (Collidable objeto in this.objetos)
            {
                objeto.Dispose();
            }
        }
    }
}
