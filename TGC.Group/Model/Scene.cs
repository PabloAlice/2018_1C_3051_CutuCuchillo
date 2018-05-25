using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Group.Model.Vehiculos;
using TGC.Group.Model.Vehiculos.Estados;

namespace TGC.Group.Model
{
    class Scene
    {
        private static Scene instance;
        private Vehiculo auto;

        private SceneElement scene;
        private Seccion cocina, banio, habitacion;

        private Scene()
        {
            this.cocina = new Seccion(new TGCVector3(-52, 0, 145), new TGCVector3(226, 0, 380));
            this.habitacion = new Seccion(new TGCVector3(-221,0,-174), new TGCVector3(226,0,145));
            this.banio = new Seccion(new TGCVector3(-221, 0, 145), new TGCVector3(-52, 0, 300));

        }

        public static Scene GetInstance()
        {
            if (instance == null)
            {
                instance = new Scene();
            }

            return instance;
        }

        public void SetVehiculo(Vehiculo auto)
        {
            this.auto = auto;
        }

        public void HandleCollisions()
        {
            this.VehicleUbication().HandleCollisions(this.auto);
        }

        private bool IsBetween(TGCVector3 interes, TGCVector3 pmin, TGCVector3 pmax)
        {
            return interes.X > pmin.X && interes.X < pmax.X && interes.Z > pmin.Z && interes.Z < pmax.Z;
        }

        private bool IsIn(Seccion seccion)
        {
            TGCVector3 posicion = this.auto.GetPosicion();
            return IsBetween(posicion, seccion.GetPuntoMinimo(), seccion.GetPuntoMaximo());
            
        }

        private Seccion VehicleUbication()
        {
            if (this.IsIn(this.cocina))
            {
                return this.cocina;
            }
            else if (this.IsIn(this.habitacion))
            {
                return this.habitacion;
            }
            else
            {
                return this.banio;
            }
        }

        public void Init(string mediaDir)
        {



            List<TGCMatrix> transformaciones;

            this.scene = this.GiveMeAnObject("Texturas\\Habitacion\\escenaFinal-TgcScene.xml", GenerateTransformation(new TGCVector3(1, 1, 1), new TGCVector3(0, 0, 0), new TGCVector3(0, 0, 0)));
            //cocina
            this.cocina.AddElements(this.GiveMeAnObject("MeshCreator\\Meshes\\Cocina\\DispenserAgua\\DispenserAgua-TgcScene.xml", GenerateTransformation(new TGCVector3(1f, 1f, 1f), new TGCVector3(0, 0, 0), new TGCVector3(-32f, -1f, 360f))));
            this.cocina.AddElements(this.GiveMeAnObject("MeshCreator\\Meshes\\Cocina\\Mesa\\Mesa-TgcScene.xml", GenerateTransformation(new TGCVector3(1f, 1f, 1f), new TGCVector3(0, FastMath.PI_HALF, 0), new TGCVector3(94f, 0f, 335f))));
            this.cocina.AddElements(this.GiveMeAnObject("MeshCreator\\Meshes\\Cocina\\Heladera\\Heladera-TgcScene.xml", GenerateTransformation(new TGCVector3(1f, 1f, 1f), new TGCVector3(0, 0, 0), new TGCVector3(191f, -1f, 184f))));
            this.cocina.AddElements(this.GiveMeAnObject("MeshCreator\\Meshes\\Cocina\\Tacho\\Tacho-TgcScene.xml", GenerateTransformation(new TGCVector3(0.6f, 0.6f, 0.6f), new TGCVector3(0, 0, 0), new TGCVector3(-30f, -1f, 165f))));

            transformaciones = new List<TGCMatrix>();
            transformaciones.Add(GenerateTransformation(new TGCVector3(0.4f, 0.4f, 0.4f), new TGCVector3(0, -FastMath.QUARTER_PI, 0), new TGCVector3(116f, 0f, 295f)));
            transformaciones.Add(GenerateTransformation(new TGCVector3(0.4f, 0.4f, 0.4f), new TGCVector3(0, -FastMath.QUARTER_PI, 0), new TGCVector3(90f, 0f, 265f)));
            transformaciones.Add(GenerateTransformation(new TGCVector3(0.4f, 0.4f, 0.4f), new TGCVector3(0, -FastMath.PI_HALF, 0), new TGCVector3(140f, 0f, 335f)));
            this.cocina.AddElements(this.GiveMeAnObject("MeshCreator\\Meshes\\Cocina\\Silla\\Silla-TgcScene.xml", transformaciones));
            this.cocina.AddElements(this.GiveMeAnObject("MeshCreator\\Meshes\\Cocina\\Mueble\\Mueble-TgcScene.xml", GenerateTransformation(new TGCVector3(1f, 1f, 0.5f), new TGCVector3(0, 0, 0), new TGCVector3(14f, -1f, 357f))));

            transformaciones = new List<TGCMatrix>();
            transformaciones.Add(GenerateTransformation(new TGCVector3(0.15f, 0.15f, 0.15f), new TGCVector3(0, 0, 0), new TGCVector3(110f, 23.85f, 290f)));
            transformaciones.Add(GenerateTransformation(new TGCVector3(0.15f, 0.15f, 0.15f), new TGCVector3(0, -FastMath.QUARTER_PI, 0), new TGCVector3(110f, 25.1f, 290f)));
            this.cocina.AddElements(this.GiveMeAnObject("MeshCreator\\Meshes\\Habitacion\\Libros\\Comun\\Comun-TgcScene.xml", transformaciones));
            this.cocina.AddElements(this.GiveMeAnObject("MeshCreator\\Meshes\\Habitacion\\Libros\\Arquitectura\\Arquitectura-TgcScene.xml", GenerateTransformation(new TGCVector3(0.15f, 0.15f, 0.15f), new TGCVector3(0, -FastMath.PI_HALF, 0), new TGCVector3(110f, 26.35f, 290f))));
            this.cocina.AddElements(this.GiveMeAnObject("MeshCreator\\Meshes\\Habitacion\\Caja\\Caja-TgcScene.xml", GenerateTransformation(new TGCVector3(0.25f, 0.26f, 0.25f), new TGCVector3(0, -FastMath.QUARTER_PI, 0), new TGCVector3(65f, 0f, 235f))));
            this.cocina.AddElements(this.GiveMeAnObject("MeshCreator\\Meshes\\Cocina\\Mueble\\Mueble-TgcScene.xml", GenerateTransformation(new TGCVector3(1f, 1f, 0.5f), new TGCVector3(0, FastMath.PI_HALF, 0), new TGCVector3(202f, 0f, 342f))));
            this.cocina.AddElements(this.GiveMeAnObject("MeshCreator\\Meshes\\Habitacion\\Toma\\Toma-TgcScene.xml", GenerateTransformation(new TGCVector3(0.5f, 0.5f, 0.5f), new TGCVector3(0, 0f, FastMath.PI_HALF), new TGCVector3(60f, 15f, 378f))));

            transformaciones = new List<TGCMatrix>();
            transformaciones.Add(GenerateTransformation(new TGCVector3(0.07f, 0.07f, 0.07f), new TGCVector3(0, 0, 0), new TGCVector3(94f, 43.5f, 340f)));
            transformaciones.Add(GenerateTransformation(new TGCVector3(0.07f, 0.07f, 0.07f), new TGCVector3(0, FastMath.PI, 0), new TGCVector3(70f, 43.5f, 315f)));
            transformaciones.Add(GenerateTransformation(new TGCVector3(0.07f, 0.07f, 0.07f), new TGCVector3(0, FastMath.PI_HALF, 0), new TGCVector3(125f, 43.5f, 355f)));
            transformaciones.Add(GenerateTransformation(new TGCVector3(0.07f, 0.07f, 0.07f), new TGCVector3(0, -FastMath.PI_HALF, 0), new TGCVector3(130f, 43.5f, 325f)));
            this.cocina.AddElements(this.GiveMeAnObject("MeshCreator\\Meshes\\Cocina\\CopaMadera\\CopaMadera-TgcScene.xml", transformaciones));

            transformaciones = new List<TGCMatrix>();
            transformaciones.Add(GenerateTransformation(new TGCVector3(0.07f, 0.07f, 0.07f), new TGCVector3(0, 0, 0), new TGCVector3(120f, 39.3f, 312f)));
            transformaciones.Add(GenerateTransformation(new TGCVector3(0.07f, 0.07f, 0.07f), new TGCVector3(0, -FastMath.PI_HALF, 0), new TGCVector3(50f, 39.3f, 350f)));
            transformaciones.Add(GenerateTransformation(new TGCVector3(0.07f, 0.07f, 0.07f), new TGCVector3(0, FastMath.PI, 0), new TGCVector3(80f, 39.3f, 340f)));
            transformaciones.Add(GenerateTransformation(new TGCVector3(0.07f, 0.07f, 0.07f), new TGCVector3(0, -FastMath.PI, 0), new TGCVector3(130f, 39.3f, 330f)));
            transformaciones.Add(GenerateTransformation(new TGCVector3(0.07f, 0.07f, 0.07f), new TGCVector3(0, 0, 0), new TGCVector3(105f, 39.3f, 327f)));
            transformaciones.Add(GenerateTransformation(new TGCVector3(0.07f, 0.07f, 0.07f), new TGCVector3(0, FastMath.PI_HALF, 0), new TGCVector3(67f, 39.3f, 328f)));
            this.cocina.AddElements(this.GiveMeAnObject("MeshCreator\\Meshes\\Cocina\\Tenedor\\Tenedor-TgcScene.xml", transformaciones));
            
            this.cocina.AddElements(this.GiveMeAnObject("MeshCreator\\Meshes\\Cocina\\Esponja\\Esponja-TgcScene.xml", GenerateTransformation(new TGCVector3(0.2f, 0.2f, 0.2f), new TGCVector3(0, 0, 0), new TGCVector3(94f, 39.3f, 325f))));
            this.cocina.AddElements(this.GiveMeAnObject("MeshCreator\\Meshes\\Cocina\\Mueble2\\Mueble2-TgcScene.xml", GenerateTransformation(new TGCVector3(1f, 1f, 0.5f), new TGCVector3(0, FastMath.PI_HALF, 0), new TGCVector3(202f, 0f, 270f))));

            transformaciones = new List<TGCMatrix>();
            transformaciones.Add(GenerateTransformation(new TGCVector3(0.65f, 0.65f, 0.65f), new TGCVector3(0, -FastMath.PI_HALF, 0), new TGCVector3(202f, 100f, 270f)));
            transformaciones.Add(GenerateTransformation(new TGCVector3(0.65f, 0.65f, 0.65f), new TGCVector3(0, -FastMath.PI_HALF, 0), new TGCVector3(202f, 100f, 342f)));
            this.cocina.AddElements(this.GiveMeAnObject("MeshCreator\\Meshes\\Cocina\\MueblePared\\MueblePared-TgcScene.xml", transformaciones));

            transformaciones = new List<TGCMatrix>();
            transformaciones.Add(GenerateTransformation(new TGCVector3(0.25f, 0.25f, 0.25f), new TGCVector3(0, 0, 0), new TGCVector3(3f, 0f, 310f)));
            transformaciones.Add(GenerateTransformation(new TGCVector3(0.25f, 0.25f, 0.25f), new TGCVector3(0, 0, 0), new TGCVector3(109f, 0f, 324f)));
            transformaciones.Add(GenerateTransformation(new TGCVector3(0.25f, 0.25f, 0.25f), new TGCVector3(0, 0, 0), new TGCVector3(85f, 0f, 241f)));
            this.cocina.AddElements(this.GiveMeAnObject("MeshCreator\\Meshes\\Cocina\\Latas\\Pepsi\\Pepsi-TgcScene.xml", transformaciones));

            transformaciones = new List<TGCMatrix>();
            transformaciones.Add(GenerateTransformation(new TGCVector3(0.25f, 0.25f, 0.25f), new TGCVector3(0, 0, 0), new TGCVector3(60f, 0f, 344f)));
            transformaciones.Add(GenerateTransformation(new TGCVector3(0.25f, 0.25f, 0.25f), new TGCVector3(0, 0, 0), new TGCVector3(58f, 0f, 293f)));
            transformaciones.Add(GenerateTransformation(new TGCVector3(0.25f, 0.25f, 0.25f), new TGCVector3(0, 0, 0), new TGCVector3(114f, 0f, 282f)));
            this.cocina.AddElements(this.GiveMeAnObject("MeshCreator\\Meshes\\Cocina\\Latas\\Fanta\\Fanta-TgcScene.xml", transformaciones));

            transformaciones = new List<TGCMatrix>();
            transformaciones.Add(GenerateTransformation(new TGCVector3(0.25f, 0.25f, 0.25f), new TGCVector3(0, 0, 0), new TGCVector3(177f, 0f, 341f)));
            transformaciones.Add(GenerateTransformation(new TGCVector3(0.25f, 0.25f, 0.25f), new TGCVector3(0, 0, 0), new TGCVector3(-43f, 0f, 315f)));
            transformaciones.Add(GenerateTransformation(new TGCVector3(0.25f, 0.25f, 0.25f), new TGCVector3(0, 0, 0), new TGCVector3(77f, 0f, 154f)));
            this.cocina.AddElements(this.GiveMeAnObject("MeshCreator\\Meshes\\Cocina\\Latas\\CocaCola\\CocaCola-TgcScene.xml", transformaciones));

            transformaciones = new List<TGCMatrix>();
            transformaciones.Add(GenerateTransformation(new TGCVector3(0.25f, 0.25f, 0.25f), new TGCVector3(0, 0, 0), new TGCVector3(-48f, 0f, 174f)));
            transformaciones.Add(GenerateTransformation(new TGCVector3(0.25f, 0.25f, 0.25f), new TGCVector3(0, 0, 0), new TGCVector3(127f, 0f, 254f)));
            this.cocina.AddElements(this.GiveMeAnObject("MeshCreator\\Meshes\\Cocina\\Latas\\Frijoles\\Frijoles-TgcScene.xml", transformaciones));

            this.cocina.AddElements(this.GiveMeAnObject("MeshCreator\\Meshes\\Cocina\\Microondas\\Microondas-TgcScene.xml", GenerateTransformation(new TGCVector3(0.5f, 0.5f, 0.5f), new TGCVector3(0, FastMath.PI_HALF + FastMath.PI, 0), new TGCVector3(10f, 63f, 344f))));

            

            //banio
            this.banio.AddElements(this.GiveMeAnObject("MeshCreator\\Meshes\\Bathroom\\Jabon\\Jabon-TgcScene.xml", GenerateTransformation(new TGCVector3(0.7f, 0.7f, 0.7f), new TGCVector3(0, 0, 0), new TGCVector3(-75f, 0f, 265f))));
            this.banio.AddElements(this.GiveMeAnObject("MeshCreator\\Meshes\\Bathroom\\Bathtub\\Bathtub-TgcScene.xml", GenerateTransformation(new TGCVector3(1.5f, 1.8f, 1.5f), new TGCVector3(0, 0, 0), new TGCVector3(-164f, 0f, 270f))));
            this.banio.AddElements(this.GiveMeAnObject("MeshCreator\\Meshes\\Bathroom\\InodoroCuadrado\\InodoroCuadrado-TgcScene.xml",GenerateTransformation(new TGCVector3(2f, 3f, 3f), new TGCVector3(0, 0, 0), new TGCVector3(-85, 0f, 175))));
            this.banio.AddElements(this.GiveMeAnObject("MeshCreator\\Meshes\\Bathroom\\Cepillo\\Cepillo-TgcScene.xml", GenerateTransformation(new TGCVector3(1.4f, 1.4f, 1.4f), new TGCVector3(0, FastMath.QUARTER_PI, 0), new TGCVector3(-125, 0f, 170f))));
            this.banio.AddElements(this.GiveMeAnObject("MeshCreator\\Meshes\\Bathroom\\Esponja\\Esponja-TgcScene.xml", GenerateTransformation(new TGCVector3(1f, 1f, 1f), new TGCVector3(0, FastMath.QUARTER_PI, 0), new TGCVector3(-165, 0f, 190))));
            this.banio.AddElements(this.GiveMeAnObject("MeshCreator\\Meshes\\Habitacion\\Banqueta\\Banqueta-TgcScene.xml", GenerateTransformation(new TGCVector3(0.4f, 0.35f, 0.4f), new TGCVector3(0, 0, 0), new TGCVector3(-170, 0f, 215))));
            this.banio.AddElements(this.GiveMeAnObject("MeshCreator\\Meshes\\Bathroom\\Espejo\\Espejo-TgcScene.xml", GenerateTransformation(new TGCVector3(1f, 1f, 1f), new TGCVector3(0, 0, 0), new TGCVector3(-90f, 0f, 298f))));
           
            this.banio.AddElements(this.GiveMeAnObject("MeshCreator\\Meshes\\Bathroom\\TuboMetal\\TuboMetal-TgcScene.xml", GenerateTransformation(new TGCVector3(0.1f, 0.1f, 0.1f), new TGCVector3(0, -FastMath.PI_HALF, 0), new TGCVector3(-215f, 3.7f, 193f))));
            this.banio.AddElements(this.GiveMeAnObject("MeshCreator\\Meshes\\Bathroom\\TuboMetal2\\TuboMetal2-TgcScene.xml", GenerateTransformation(new TGCVector3(0.1f, 0.1f, 0.1f), new TGCVector3(0, 0, 0), new TGCVector3(-89f, 3.2f, 207f))));
            this.banio.AddElements(this.GiveMeAnObject("MeshCreator\\Meshes\\Bathroom\\TuboMetal3\\TuboMetal3-TgcScene.xml", GenerateTransformation(new TGCVector3(0.1f, 0.1f, 0.1f), new TGCVector3(0, 0, 0), new TGCVector3(-108f, 11f, 294f))));

            //habitacion
            this.habitacion.AddElements(new Misile(TGCMatrix.Translation(0f, 0f, 10f)));

            this.habitacion.AddElements(this.GiveMeAnObject("MeshCreator\\Meshes\\Habitacion\\Cama\\Cama-TgcScene.xml", GenerateTransformation(new TGCVector3(1, 1, 1), new TGCVector3(0, FastMath.PI, 0), new TGCVector3(-36f, 0, -124f))));
            this.habitacion.AddElements(this.GiveMeAnObject("MeshCreator\\Meshes\\Habitacion\\MesaDeLuz\\MesaDeLuz-TgcScene.xml", GenerateTransformation(new TGCVector3(1, 1, 1), new TGCVector3(0, FastMath.PI, 0), new TGCVector3(22f, 0, -158f))));
            this.habitacion.AddElements(this.GiveMeAnObject("MeshCreator\\Meshes\\Habitacion\\Placard\\Placard-TgcScene.xml", GenerateTransformation(new TGCVector3(1.2f, 1.5f, 1), new TGCVector3(0, -FastMath.PI_HALF, 0), new TGCVector3(-200f, 0, -105f))));
            this.habitacion.AddElements(this.GiveMeAnObject("MeshCreator\\Meshes\\Habitacion\\Armario\\Armario-TgcScene.xml", GenerateTransformation(new TGCVector3(1, 1, 1), new TGCVector3(0, -FastMath.PI_HALF, 0), new TGCVector3(-30f, 0, 110f))));
            this.habitacion.AddElements(this.GiveMeAnObject("MeshCreator\\Meshes\\Habitacion\\Escritorio\\Escritorio-TgcScene.xml", GenerateTransformation(new TGCVector3(1, 1, 1), new TGCVector3(0, 0, 0), new TGCVector3(183f, 0f, -107f))));
            this.habitacion.AddElements(this.GiveMeAnObject("MeshCreator\\Meshes\\Cocina\\Silla\\Silla-TgcScene.xml", GenerateTransformation(new TGCVector3(0.6f, 0.6f, 0.6f), new TGCVector3(0, FastMath.PI_HALF, 0), new TGCVector3(151f, -1f, -101f))));
            this.habitacion.AddElements(this.GiveMeAnObject("MeshCreator\\Meshes\\Habitacion\\CajaZapatillas\\CajaZapatillas-TgcScene.xml", GenerateTransformation(new TGCVector3(0.5f, 0.5f, 0.5f), new TGCVector3(0, FastMath.PI_HALF, 0), new TGCVector3(100f, 0f, -147f))));
            this.habitacion.AddElements(this.GiveMeAnObject("MeshCreator\\Meshes\\Habitacion\\Sillon\\Sillon-TgcScene.xml", GenerateTransformation(new TGCVector3(1f, 1f, 1f), new TGCVector3(0f, FastMath.PI+FastMath.PI_HALF, 0f), new TGCVector3(-180f, -0.5f, 20f))));
            this.habitacion.AddElements(this.GiveMeAnObject("MeshCreator\\Meshes\\Habitacion\\Escoba\\Escoba-TgcScene.xml", GenerateTransformation(new TGCVector3(1.3f, 1.3f, 1.3f), new TGCVector3(FastMath.PI_HALF, 0, 0), new TGCVector3(-105f, 1f, -60f))));
            this.habitacion.AddElements(this.GiveMeAnObject("MeshCreator\\Meshes\\Habitacion\\Rubik\\Rubik-TgcScene.xml", GenerateTransformation(new TGCVector3(0.4f, 0.4f, 0.4f), new TGCVector3(0, 0, 0), new TGCVector3(100f, 0f, -100f))));

            transformaciones = new List<TGCMatrix>();
            transformaciones.Add(GenerateTransformation(new TGCVector3(0.2f, 0.2f, 0.2f), new TGCVector3(0, 0, FastMath.PI_HALF), new TGCVector3(-10f, 1f, 3f)));
            transformaciones.Add(GenerateTransformation(new TGCVector3(0.2f, 0.2f, 0.2f), new TGCVector3(0, 1f, 0), new TGCVector3(-75f, 0f, -100f)));
            transformaciones.Add(GenerateTransformation(new TGCVector3(0.2f, 0.2f, 0.2f), new TGCVector3(0, 0f, FastMath.PI_HALF), new TGCVector3(75f, 1f, 10f)));
            this.habitacion.AddElements(this.GiveMeAnObject("MeshCreator\\Meshes\\Habitacion\\Rastis\\Rasti-1\\Rastis-1-TgcScene.xml", transformaciones));

            transformaciones = new List<TGCMatrix>();
            transformaciones.Add(GenerateTransformation(new TGCVector3(0.2f, 0.2f, 0.2f), new TGCVector3(0, 1f, 0f), new TGCVector3(45f, 0f, 20f)));
            transformaciones.Add(GenerateTransformation(new TGCVector3(0.2f, 0.2f, 0.2f), new TGCVector3(0, 2f, 0f), new TGCVector3(40f, 0f, -20f)));
            transformaciones.Add(GenerateTransformation(new TGCVector3(0.2f, 0.2f, 0.2f), new TGCVector3(0, 3f, 0f), new TGCVector3(-50f, 0f, 20f)));
            this.habitacion.AddElements(this.GiveMeAnObject("MeshCreator\\Meshes\\Habitacion\\Rastis\\Rasti-2\\Rastis-2-TgcScene.xml", transformaciones));

            transformaciones = new List<TGCMatrix>();
            transformaciones.Add(GenerateTransformation(new TGCVector3(0.2f, 0.2f, 0.2f), new TGCVector3(0, 0f, 0f), new TGCVector3(22f, 0f, 0f)));
            transformaciones.Add(GenerateTransformation(new TGCVector3(0.2f, 0.2f, 0.2f), new TGCVector3(0, 2f, 0f), new TGCVector3(-100f, 0f, 45f)));
            transformaciones.Add(GenerateTransformation(new TGCVector3(0.2f, 0.2f, 0.2f), new TGCVector3(0, 1f, 0f), new TGCVector3(100f, 0f, 50f)));
            this.habitacion.AddElements(this.GiveMeAnObject("MeshCreator\\Meshes\\Habitacion\\Rastis\\Rasti-4\\Rastis-4-TgcScene.xml", transformaciones));

            transformaciones = new List<TGCMatrix>();
            transformaciones.Add(GenerateTransformation(new TGCVector3(0.2f, 0.2f, 0.2f), new TGCVector3(0, 1f, 0f), new TGCVector3(0f, 0f, -50f)));
            transformaciones.Add(GenerateTransformation(new TGCVector3(0.2f, 0.2f, 0.2f), new TGCVector3(0, 1f, 0f), new TGCVector3(-120f, 0f, -10f)));
            transformaciones.Add(GenerateTransformation(new TGCVector3(0.2f, 0.2f, 0.2f), new TGCVector3(0, 1f, 0f), new TGCVector3(-200f, 0f, -200f)));
            this.habitacion.AddElements(this.GiveMeAnObject("MeshCreator\\Meshes\\Habitacion\\Rastis\\Rasti-6\\Rastis-6-TgcScene.xml", transformaciones));

            transformaciones = new List<TGCMatrix>();
            transformaciones.Add(GenerateTransformation(new TGCVector3(0.5f, 0.5f, 0.5f), new TGCVector3(0, 0f, FastMath.PI_HALF), new TGCVector3(-110f, 15f, 144f)));
            transformaciones.Add(GenerateTransformation(new TGCVector3(0.5f, 0.5f, 0.5f), new TGCVector3(0, 0f, FastMath.PI_HALF), new TGCVector3(60f, 15f, 144f)));
            this.habitacion.AddElements(this.GiveMeAnObject("MeshCreator\\Meshes\\Habitacion\\Toma\\Toma-TgcScene.xml", transformaciones));

            this.habitacion.AddElements(this.GiveMeAnObject("MeshCreator\\Meshes\\Habitacion\\Pelota\\Pelota-TgcScene.xml", GenerateTransformation(new TGCVector3(0.4f, 0.4f, 0.4f), new TGCVector3(0, 0f, 0), new TGCVector3(153f, 0f, 85f))));
            this.habitacion.AddElements(this.GiveMeAnObject("MeshCreator\\Meshes\\Habitacion\\Notebook\\Notebook-TgcScene.xml", GenerateTransformation(new TGCVector3(0.5f, 0.5f, 0.5f), new TGCVector3(0, 0, 0), new TGCVector3(174f, 44f, -99f))));
            this.habitacion.AddElements(this.GiveMeAnObject("MeshCreator\\Meshes\\Habitacion\\Libros\\Arquitectura\\Arquitectura-TgcScene.xml", GenerateTransformation(new TGCVector3(0.5f, 0.3f, 0.5f), new TGCVector3(0, 0, 0), new TGCVector3(178f, 44f, -139f))));
            this.habitacion.AddElements(this.GiveMeAnObject("MeshCreator\\Meshes\\Habitacion\\Cargador\\Cargador-TgcScene.xml", GenerateTransformation(new TGCVector3(0.2f, 0.2f, 0.2f), new TGCVector3(0, 0, 0), new TGCVector3(197f, 44f, -70f))));
            this.habitacion.AddElements(this.GiveMeAnObject("MeshCreator\\Meshes\\Habitacion\\Banqueta\\Banqueta-TgcScene.xml", GenerateTransformation(new TGCVector3(0.5f, 0.5f, 0.5f), new TGCVector3(0, 0, 0), new TGCVector3(182f, 0f, -13f))));
            this.habitacion.AddElements(this.GiveMeAnObject("MeshCreator\\Meshes\\Habitacion\\Celular\\Celular-TgcScene.xml", GenerateTransformation(new TGCVector3(0.1f, 0.1f, 0.1f), new TGCVector3(0, 0, 0), new TGCVector3(24f, 32.2f, -154f))));
            this.habitacion.AddElements(this.GiveMeAnObject("MeshCreator\\Meshes\\Habitacion\\CajaPizza\\CajaPizza-TgcScene.xml", GenerateTransformation(new TGCVector3(0.5f, 0.5f, 0.5f), new TGCVector3(0, 0, 0), new TGCVector3(-173f, 28f, 15f))));
            this.habitacion.AddElements(this.GiveMeAnObject("MeshCreator\\Meshes\\Habitacion\\Lapiz\\Lapiz-TgcScene.xml", GenerateTransformation(new TGCVector3(0.3f, 0.3f, 0.3f), new TGCVector3(0, FastMath.PI_HALF, 0), new TGCVector3(208f, 45f, -130f))));


            //puertas
            transformaciones = new List<TGCMatrix>();
            transformaciones.Add(GenerateTransformation(new TGCVector3(1f, 2.26f, 1.88f), new TGCVector3(0, FastMath.PI_HALF, 0), new TGCVector3(-173.6f, 0, 145)));
            transformaciones.Add(GenerateTransformation(new TGCVector3(1f, 2.26f, 2f), new TGCVector3(0, -FastMath.PI_HALF, 0), new TGCVector3(131.5f, 0, 145.5f)));
            this.habitacion.AddElements(this.GiveMeAnObject("MeshCreator\\Meshes\\Otros\\Puerta\\Puerta-TgcScene.xml", transformaciones));


            transformaciones = new List<TGCMatrix>();
            transformaciones.Add(GenerateTransformation(new TGCVector3(1f, 2.26f, 2f), new TGCVector3(0, -FastMath.PI_HALF, 0), new TGCVector3(131.5f, 0, 145.5f)));
            transformaciones.Add(GenerateTransformation(new TGCVector3(1f, 2.26f, 1.6f), new TGCVector3(0, 0, 0), new TGCVector3(-52.5f, 0, 260f)));
            this.cocina.AddElements(this.GiveMeAnObject("MeshCreator\\Meshes\\Otros\\Puerta\\Puerta-TgcScene.xml", transformaciones));

            transformaciones = new List<TGCMatrix>();
            transformaciones.Add(GenerateTransformation(new TGCVector3(1f, 2.26f, 1.6f), new TGCVector3(0, 0, 0), new TGCVector3(-52.5f, 0, 260f)));
            transformaciones.Add(GenerateTransformation(new TGCVector3(1f, 2.26f, 1.88f), new TGCVector3(0, FastMath.PI_HALF, 0), new TGCVector3(-173.6f, 0, 145)));
            this.banio.AddElements(this.GiveMeAnObject("MeshCreator\\Meshes\\Otros\\Puerta\\Puerta-TgcScene.xml", transformaciones));

            //portales
            TGCVector3 scale = new TGCVector3(0.2f, 0.2f, 0.2f);
            TGCMatrix transformation, transformation2;
            Portal portal, portal2;
            TGCVector3 targetPosition;

            //portal que va abajo del escritorio y que se dirige arriba del escritorio
            transformation = ConceptosGlobales.GetInstance().GenerateTransformation(scale, new TGCVector3(0, 0, 0), new TGCVector3(159f, 0, -162f));
            portal = new Portal(new TGCVector3(159f, 0, -162f), transformation);
            targetPosition = new TGCVector3(154,45,-160);
            this.habitacion.AddElements(new UnidirectionalPortal(portal, targetPosition, new TGCVector3(1,0,0)));

            //portal que conecta la habitacion con la cocina (bidireccional)
            transformation = ConceptosGlobales.GetInstance().GenerateTransformation(scale, new TGCVector3(0, 0, 0), new TGCVector3(133f, 0, 143f));
            portal = new Portal(new TGCVector3(133f, 0, 143f), transformation);
            transformation2 = ConceptosGlobales.GetInstance().GenerateTransformation(scale, new TGCVector3(0, 0, 0), new TGCVector3(133f, 0, 149f));
            portal2 = new Portal(new TGCVector3(133f, 0, 149f), transformation2);
            this.habitacion.AddElements(new BidirectionalPortal(portal, portal2, new TGCVector3(0,0,1)));
            this.cocina.AddElements(new BidirectionalPortal(portal2, portal, new TGCVector3(0, 0, -1)));

            //portal que va de abajo de la mesa, hacia la arriba de la mesa (unidireccional)
            transformation = ConceptosGlobales.GetInstance().GenerateTransformation(scale, new TGCVector3(0, 0, 0), new TGCVector3(65f, 0, 378f));
            portal = new Portal(new TGCVector3(65f, 0, 378f), transformation);
            targetPosition = new TGCVector3(52, 40, 358);
            this.cocina.AddElements(new UnidirectionalPortal(portal, targetPosition, new TGCVector3(0, 0, -1)));

            //portal que va de abajo del mueble de la cocina, hacia arriba del mueblePared (unidireccional)
            transformation = ConceptosGlobales.GetInstance().GenerateTransformation(scale, new TGCVector3(0, FastMath.PI_HALF, 0), new TGCVector3(224f, 0, 375.5f));
            portal = new Portal(new TGCVector3(224f, 0, 375.5f), transformation);
            targetPosition = new TGCVector3(215, 145, 363);
            this.cocina.AddElements(new UnidirectionalPortal(portal, targetPosition, new TGCVector3(0, 0, -1)));

            //portal que va de abajo del mueble de la cocina, hacia arriba del mueblecomun (unidireccional)
            transformation = ConceptosGlobales.GetInstance().GenerateTransformation(scale, new TGCVector3(0, 0, 0), new TGCVector3(197f, 0, 312f));
            portal = new Portal(new TGCVector3(197f, 0, 312f), transformation);
            targetPosition = new TGCVector3(215, 60, 369);
            this.cocina.AddElements(new UnidirectionalPortal(portal, targetPosition, new TGCVector3(0, 0, -1)));

            //portal que conecta el baño con la cocina (bidireccional)
            transformation = ConceptosGlobales.GetInstance().GenerateTransformation(scale, new TGCVector3(0, FastMath.PI_HALF, 0), new TGCVector3(-48f, 0, 258f));
            portal = new Portal(new TGCVector3(-48f, 0, 258f), transformation);
            transformation2 = ConceptosGlobales.GetInstance().GenerateTransformation(scale, new TGCVector3(0, FastMath.PI_HALF, 0), new TGCVector3(-55f, 0, 258f));
            portal2 = new Portal(new TGCVector3(-55f, 0, 258f), transformation2);
            this.cocina.AddElements(new BidirectionalPortal(portal, portal2, new TGCVector3(-1,0,0)));
            this.banio.AddElements(new BidirectionalPortal(portal2, portal, new TGCVector3(1, 0, 0)));


            //portal que conecta el baño con la habitacion
            transformation = ConceptosGlobales.GetInstance().GenerateTransformation(scale, new TGCVector3(0, 0, 0), new TGCVector3(-170f, 0, 148f));
            portal = new Portal(new TGCVector3(-170f, 0, 148f), transformation);
            transformation2 = ConceptosGlobales.GetInstance().GenerateTransformation(scale, new TGCVector3(0, 0, 0), new TGCVector3(-170f, 0, 142f));
            portal2 = new Portal(new TGCVector3(-170f, 0, 142f), transformation2);
            this.banio.AddElements(new BidirectionalPortal(portal, portal2, new TGCVector3(0, 0, -1)));
            this.habitacion.AddElements(new BidirectionalPortal(portal2, portal, new TGCVector3(0, 0, 1)));

            //portal que va desde abajo del placard hacia arriba del placard
            transformation = ConceptosGlobales.GetInstance().GenerateTransformation(scale, new TGCVector3(0, 0, 0), new TGCVector3(-202f, 0, -159f));
            portal = new Portal(new TGCVector3(-202f, 0, -159f), transformation);
            targetPosition = new TGCVector3(-210, 90, -156);
            this.habitacion.AddElements(new UnidirectionalPortal(portal, targetPosition, new TGCVector3(0, 0, 1)));

            //portal que va desde abajo de la cama hacia arriba de la cama
            transformation = ConceptosGlobales.GetInstance().GenerateTransformation(scale, new TGCVector3(0, 0, 0), new TGCVector3(-35f, 0, -173f));
            portal = new Portal(new TGCVector3(-35f, 0, -173f), transformation);
            targetPosition = new TGCVector3(34, 32, -159);
            this.habitacion.AddElements(new UnidirectionalPortal(portal, targetPosition, new TGCVector3(-1, 0, 0)));

        }

        private SceneElement GiveMeAnObject(string ruta, TGCMatrix transformacion)
        {
            TgcScene tgcScene = new TgcSceneLoader().loadSceneFromFile(ConceptosGlobales.GetInstance().GetMediaDir() + ruta);
            SceneElement nuevoObjeto = new SceneElement(tgcScene.Meshes, transformacion);
            return nuevoObjeto;

        }

        private SceneRepeatedElement GiveMeAnObject(string ruta, List<TGCMatrix> transformations)
        {
            TgcScene tgcScene = new TgcSceneLoader().loadSceneFromFile(ConceptosGlobales.GetInstance().GetMediaDir() + ruta);
            SceneRepeatedElement nuevoObjeto = new SceneRepeatedElement(tgcScene.Meshes, transformations);
            return nuevoObjeto;
        }

        private TGCMatrix GenerateTransformation(TGCVector3 escala, TGCVector3 rotacion, TGCVector3 traslado)
        {
            TGCMatrix matrixEscalado = TGCMatrix.Scaling(escala);
            TGCMatrix matrixRotacion = TGCMatrix.RotationYawPitchRoll(rotacion.Y, rotacion.X, rotacion.Z);
            TGCMatrix matrixTraslacion = TGCMatrix.Translation(traslado);
            TGCMatrix transformacion = matrixEscalado * matrixRotacion * matrixTraslacion;
            return transformacion;
        }

        public void Render()
        {
            this.scene.Render();
            this.VehicleUbication().Render();
        }

        public void Dispose()
        {
            this.scene.Dispose();
            this.habitacion.Dispose();
            this.cocina.Dispose();
            this.banio.Dispose();

        }
        public void remove(Collidable objeto)
        {
            VehicleUbication().remove(objeto);
        }

        /*public Camioneta calculateCollisions(Camioneta auto)
        {
            //bool collide = false;
            TgcMesh collider = null;
            auto.mesh.BoundingBox.setRenderColor(Color.Yellow);
            foreach (Objeto objeto in objetosEscenario)
            {
                if ((collider = objeto.TestColision(auto.mesh)) != null)
                {
                    auto.mesh.BoundingBox.setRenderColor(Color.Red);
                    collider.BoundingBox.setRenderColor(Color.Red);
                    auto.SetVelocidadActual(-auto.GetVelocidadActual() * 4);
                    auto.SetEstado(new Backward(auto));
                }

                else
                {
                    objeto.SetColorBoundingBox(Color.Yellow);
                }

            }

            return auto;
        }*/
    }
}
