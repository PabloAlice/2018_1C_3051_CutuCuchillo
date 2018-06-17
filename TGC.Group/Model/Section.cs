using System.Collections.Generic;
using TGC.Core.Mathematica;
using TGC.Group.Lighting;
using Microsoft.DirectX.Direct3D;
using TGC.Core.Geometry;
using System.Drawing;
using System;

namespace TGC.Group.Model
{
    class Section
    {
        private List<SubSection> subSections = new List<SubSection>();
        private TGCVector3 puntoMinimo, puntoMaximo;
        private Lighting.Light light;
        private TGCBox lightMesh;
        private uint numberOfPartitions = 2;

        public Section(TGCVector3 puntoMinimo, TGCVector3 puntoMaximo)
        {
            this.puntoMinimo = puntoMinimo;
            this.puntoMaximo = puntoMaximo;
            var pos = new TGCVector3((puntoMaximo.X + puntoMinimo.X) / 2, puntoMaximo.Y, (puntoMaximo.Z + puntoMinimo.Z) / 2);
            this.light = new Lighting.Light(new ColorValue(255, 255, 255),  pos, 40, 0.15f);
            this.GenerateSubSections();
        }

        public List<Collidable> GetWeapons()
        {
            List<Collidable> weapons = new List<Collidable>();
            List<Collidable> weaponsSubsection = new List<Collidable>();
            foreach (SubSection subSection in this.subSections)
            {
                weaponsSubsection = subSection.GetWeapons();
                weapons.AddRange(weaponsSubsection);
            }
            return weapons;
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
            System.Console.WriteLine(position);
            throw new Exception("El elementoo no se encuentra en ninguna subseccion");
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
            this.subSections.ForEach(s => s.Render());
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
