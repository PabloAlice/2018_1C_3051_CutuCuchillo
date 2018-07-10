using System.Collections.Generic;
using TGC.Core.Mathematica;
using System.Linq;
using Microsoft.DirectX.Direct3D;
using System;

namespace TGC.Group.Model
{
    class Section
    {
        private List<SubSection> subSections = new List<SubSection>();
        private TGCVector3 puntoMinimo, puntoMaximo;
        private Lighting.Light light;
        private uint numberOfPartitions = 4;
        private Section nextSection;

        public Section(TGCVector3 puntoMinimo, TGCVector3 puntoMaximo)
        {
            this.puntoMinimo = puntoMinimo;
            this.puntoMaximo = puntoMaximo;
            var pos = new TGCVector3((puntoMaximo.X + puntoMinimo.X) / 2, 180, (puntoMaximo.Z + puntoMinimo.Z) / 2);
            this.light = new Lighting.Light(new ColorValue(255, 255, 255), pos, 10, 0.1f);
            this.GenerateSubSections();

        }

        public void Init()
        {
            subSections.ForEach(ss => ss.Init());
        }

        public void SetNextSection(Section section)
        {
            nextSection = section;
        }

        public List<TypeOfPortal> GetPortalsThatGoTo(Section section)
        {
            List<TypeOfPortal> candidates = new List<TypeOfPortal>();
            foreach (SubSection subsection in subSections)
            {
                candidates.AddRange(subsection.GetPortals().FindAll(p => p.GoTo(section)));
            }
            return candidates;
        }

        public Section NextSection()
        {
            return nextSection;
        }

        public List<Collidable> GetPlanes()
        {
            List<Collidable> candidates = new List<Collidable>();
            foreach(SubSection subsection in subSections)
            {
                candidates.AddRange(subsection.GetElements().FindAll(x => x is Plane));
            }
            return candidates;
        }

        public List<Weapon> GetWeapons()
        {
            List<Weapon> weapons = new List<Weapon>();
            foreach (SubSection subSection in this.subSections)
            {
                weapons.AddRange(subSection.GetWeapons());
            }
            return weapons;
        }

        public List<Life> GetLifes()
        {
            List<Life> lifes = new List<Life>();
            foreach (SubSection subSection in this.subSections)
            {
                lifes.AddRange(subSection.GetLifes());
            }
            return lifes;
        }

        public TGCVector3 Center()
        {
            return new TGCVector3((puntoMaximo.X + puntoMinimo.X) / 2, 0, (puntoMaximo.Z + puntoMinimo.Z) / 2);
        }

        private void GenerateSubSections()
        {
            float width = (puntoMaximo.X - puntoMinimo.X) / this.numberOfPartitions;
            float height = (puntoMaximo.Z - puntoMinimo.Z) / this.numberOfPartitions;
            for (int i = 0; i < numberOfPartitions; i++)
            {
                for (int j = 0; j < numberOfPartitions; j++)
                {

                    TGCVector3 minPoint = new TGCVector3(this.puntoMinimo.X + j * width, 0, this.puntoMinimo.Z + i * height);
                    TGCVector3 maxPoint = new TGCVector3(minPoint.X + width, 0, minPoint.Z + height);
                    SubSection subSection = new SubSection(minPoint, maxPoint);
                    this.subSections.Add(subSection);
                }
            }

        }

        public List<Collidable> GetPosiblesCollidables(TGCVector3 position)
        {
            return this.SubsectionUbication(position).GetElements();
        }

        private SubSection SubsectionUbication(TGCVector3 position)
        {
            foreach (SubSection subsection in this.subSections)
            {
                if (subsection.Contains(position))
                {
                    return subsection;
                }
            }
            return this.subSections[0];
        }

        public TGCVector3 GetPuntoMinimo()
        {
            return this.puntoMinimo;
        }

        public TGCVector3 GetPuntoMaximo()
        {
            return this.puntoMaximo;
        }

        public void AddElement(Collidable objeto)
        {
            this.subSections.ForEach(s => s.AddElement(objeto));
        }

        public void AddElement(Collidable objeto, bool verdad)
        {
            this.subSections.ForEach(s => s.AddElement(objeto, true));
        }

        public void Render()
        {
            Lighting.LightManager.GetInstance().ResetLights();
            Lighting.LightManager.GetInstance().SuscribeLight(this.light);
            List<Collidable> elements = new List<Collidable>();
            foreach (SubSection subsection in this.subSections)
            {
                elements.AddRange(subsection.GetElements());
            }
            elements = elements.Distinct().ToList();
            elements.ForEach(e => e.Render());
        }

        public void HandleCollisions(Vehicle car)
        {
            this.SubsectionUbication(car.GetPosition()).HandleCollisions(car);

        }

        public void Dispose()
        {
            this.subSections.ForEach(s => s.Dispose());
        }

        public List<SubSection> GetSubSections()
        {
            return this.subSections;
        }
    }
}
