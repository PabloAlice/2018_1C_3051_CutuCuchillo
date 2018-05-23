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
    class Seccion
    {
        private List<SceneElement> objetos = new List<SceneElement>();
        private List<TypeOfPortal> portales = new List<TypeOfPortal>();
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

        public void AddElements(SceneElement objeto)
        {
            this.objetos.Add(objeto);
        }

        public void AddPortals(TypeOfPortal portal)
        {
            this.portales.Add(portal);
        }

        public void Render()
        {
            this.RenderElements();
            this.RenderPortals();
        }

        public void RenderElements()
        {
            foreach (SceneElement objeto in this.objetos)
            {
                objeto.Render();
            }
        }

        public void RenderPortals()
        {
            foreach (TypeOfPortal portal in this.portales)
            {
                portal.Render();
            }
        }

        public void DetectCollision(Vehiculo car)
        {
            this.DetectPortalsCollision(car);
            this.DetectObjetsCollision(car);
        }

        public void DetectPortalsCollision(Vehiculo car)
        {
            System.Console.WriteLine("holi");
            foreach (TypeOfPortal portal in this.portales)
            {
                if(TgcCollisionUtils.testObbAABB(car.GetTGCBoundingOrientedBox(), portal.GetBoundingBox()))
                {
                    portal.GetBoundingBox().setRenderColor(Color.Red);
                    portal.Collide(car);
                }
            }
        }

        public void DetectObjetsCollision(Vehiculo car)
        {
            return;
        }

        public void Dispose()
        {
            foreach (SceneElement objeto in this.objetos)
            {
                objeto.Dispose();
            }

            foreach (TypeOfPortal portal in this.portales)
            {
                portal.Dispose();
            }
        }
    }
}
