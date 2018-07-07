using System;
using System.Collections.Generic;
using TGC.Core.Mathematica;
using System.Linq;
using TGC.Core.Geometry;
using System.Drawing;

namespace TGC.Group.Model
{
    class SubSection
    {
        private TGCVector3 minPoint;
        private TGCVector3 maxPoint;
        private List<Collidable> elements = new List<Collidable>();

        public SubSection(TGCVector3 puntoMinimo, TGCVector3 puntoMaximo)
        {
            this.minPoint = puntoMinimo;
            this.maxPoint = puntoMaximo;
        }

        public TGCVector3 GetMinPoint()
        {
            return this.minPoint;
        }

        public TGCVector3 GetMaxPoint()
        {
            return this.maxPoint;
        }

        public List<TypeOfPortal> GetPortals()
        {
            return this.elements.FindAll(p => p is TypeOfPortal).Cast<TypeOfPortal>().ToList();
        }

        public List<Weapon> GetWeapons()
        {
            List<Weapon> weapons = this.elements.FindAll(e => e is Weapon && e.GetPosition().Y < 2f).Cast<Weapon>().ToList();
            return weapons.FindAll(w => !(w.weaponState is ReadyToShoot));
        }

        public List<Life> GetLifes()
        {
            List<Life> lifes = this.elements.FindAll(e => e is Life).Cast<Life>().ToList();
            return lifes.FindAll(l => l.IsVisible());
        }

        public void AddElement(Collidable element)
        {
            if (element.IsInto(this.minPoint, this.maxPoint))
            {
                this.elements.Add(element);                
            }
        }

        public void AddElement(Collidable element, bool verdad)
        {
            this.elements.Add(element);
        }

        public List<Collidable> GetElements()
        {
            return this.elements;
        }

        public bool Contains(TGCVector3 position)
        {
            return GlobalConcepts.GetInstance().IsBetweenXZ(position, minPoint, maxPoint);
        }

        /*public bool Contains(Collidable element)
        {
            //return element.IsInto(minPoint, maxPoint);
        }
        */

        public void Render()
        {
            foreach (Collidable element in this.elements)
            {
                element.Render();
            }
        }

        public void HandleCollisions(Vehicle car)
        {
            foreach (Collidable element in this.elements)
            {
                element.HandleCollision(car);
            }
        }

        public void Dispose()
        {
            foreach (Collidable element in elements)
            {
                element.Dispose();
            }
        }
    }
}
