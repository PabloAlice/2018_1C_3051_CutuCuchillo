using System.Collections.Generic;
using TGC.Core.Mathematica;
using TGC.Group.Lighting;
using Microsoft.DirectX.Direct3D;
using TGC.Core.Geometry;
using System.Drawing;

namespace TGC.Group.Model
{
    class Section
    {
        private List<Collidable> objetos = new List<Collidable>();
        private TGCVector3 puntoMinimo, puntoMaximo;
        private Lighting.Light light;
        private TGCBox lightMesh;
        public Section(TGCVector3 puntoMinimo, TGCVector3 puntoMaximo)
        {
            this.puntoMinimo = puntoMinimo;
            this.puntoMaximo = puntoMaximo;
            var pos = new TGCVector3((puntoMaximo.X + puntoMinimo.X) / 2, puntoMaximo.Y/4, (puntoMaximo.Z + puntoMinimo.Z) / 2);
            this.lightMesh = TGCBox.fromSize(new TGCVector3(10,10,10), Color.Red);
            this.lightMesh.Position = pos; 
            this.light = new Lighting.Light(new ColorValue(255, 255, 255), pos, 38, 0.15f);
            Lighting.LightManager.Instance.SuscribeLight(this.light);
        }

        public List<Collidable> GetElements()
        { 
            return this.objetos;
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
            this.objetos.Add(objeto);
        }

        public void remove(Collidable objeto)
        {
            objetos.Remove(objeto);
        }

        public void Render()
        {
            this.lightMesh.Render();
            foreach (Collidable objeto in this.objetos)
            {
                objeto.Render();
            }
        }

        public void HandleCollisions(Vehicle car)
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
