using System;
using System.Collections.Generic;
using TGC.Core.Mathematica;
using TGC.Core.Geometry;

namespace TGC.Group.Model
{
    class SubSection
    {
        private TGCVector3 minPoint;
        private TGCVector3 maxPoint;
        List<Collidable> elements = new List<Collidable>();

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

        public void AddElements(Collidable element)
        {
            this.elements.Add(element);
        }

        public List<Collidable> GetElements()
        {
            return this.elements;
        }

        public bool Contains(TGCVector3 position)
        {
            return GlobalConcepts.GetInstance().IsBetween(position, minPoint, maxPoint);
        }

        public bool Contains(Collidable element)
        {
            return element.IsInto(minPoint, maxPoint);
        }

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
                element.HandleCollisions(car);
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
