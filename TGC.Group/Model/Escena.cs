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
    class Escena
    {
        private static Escena instance;
        private Vehiculo auto;
        private String mediaDir = "";

        private SceneElement scene;
        private Seccion cocina, banio, habitacion;

        private Escena()
        {
            this.cocina = new Seccion(new TGCVector3(-52, 0, 145), new TGCVector3(226, 0, 380));
            this.habitacion = new Seccion(new TGCVector3(-221,0,-174), new TGCVector3(226,0,145));
            this.banio = new Seccion(new TGCVector3(-221, 0, 145), new TGCVector3(-52, 0, 300));

        }

        public static Escena getInstance()
        {
            if (instance == null)
            {
                instance = new Escena();
            }

            return instance;
        }

        public void SetVehiculo(Vehiculo auto)
        {
            this.auto = auto;
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

        private Seccion UbicacionVehiculo()
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

            this.mediaDir = mediaDir;

            this.scene = this.GiveMeAnObject("Texturas\\Habitacion\\escenaFinal-TgcScene.xml", generateTransformation(new TGCVector3(1, 1, 1), new TGCVector3(0, 0, 0), new TGCVector3(0, 0, 0)));
            //cocina
            this.cocina.Add(this.GiveMeAnObject("MeshCreator\\Meshes\\Cocina\\DispenserAgua\\DispenserAgua-TgcScene.xml", generateTransformation(new TGCVector3(1f, 1f, 1f), new TGCVector3(0, 0, 0), new TGCVector3(-32f, -1f, 360f))));
            this.cocina.Add(this.GiveMeAnObject("MeshCreator\\Meshes\\Cocina\\Mesa\\Mesa-TgcScene.xml", generateTransformation(new TGCVector3(1f, 1f, 1f), new TGCVector3(0, FastMath.PI_HALF, 0), new TGCVector3(94f, 0f, 335f))));
            this.cocina.Add(this.GiveMeAnObject("MeshCreator\\Meshes\\Cocina\\Heladera\\Heladera-TgcScene.xml", generateTransformation(new TGCVector3(1f, 1f, 1f), new TGCVector3(0, 0, 0), new TGCVector3(190f, -1f, 180f))));
            this.cocina.Add(this.GiveMeAnObject("MeshCreator\\Meshes\\Cocina\\Tacho\\Tacho-TgcScene.xml", generateTransformation(new TGCVector3(0.6f, 0.6f, 0.6f), new TGCVector3(0, 0, 0), new TGCVector3(-30f, -1f, 165f))));

            transformaciones = new List<TGCMatrix>();
            transformaciones.Add(generateTransformation(new TGCVector3(0.4f, 0.4f, 0.4f), new TGCVector3(0, -FastMath.QUARTER_PI, 0), new TGCVector3(116f, 0f, 295f)));
            transformaciones.Add(generateTransformation(new TGCVector3(0.4f, 0.4f, 0.4f), new TGCVector3(0, -FastMath.QUARTER_PI, 0), new TGCVector3(90f, 0f, 265f)));
            transformaciones.Add(generateTransformation(new TGCVector3(0.4f, 0.4f, 0.4f), new TGCVector3(0, -FastMath.PI_HALF, 0), new TGCVector3(140f, 0f, 335f)));
            this.cocina.Add(this.GiveMeAnObject("MeshCreator\\Meshes\\Cocina\\Silla\\silla-TgcScene.xml", transformaciones));
            this.cocina.Add(this.GiveMeAnObject("MeshCreator\\Meshes\\Cocina\\Mueble\\Mueble-TgcScene.xml", generateTransformation(new TGCVector3(1f, 1f, 0.5f), new TGCVector3(0, 0, 0), new TGCVector3(10f, -1f, 357f))));

            transformaciones = new List<TGCMatrix>();
            transformaciones.Add(generateTransformation(new TGCVector3(0.15f, 0.15f, 0.15f), new TGCVector3(0, 0, 0), new TGCVector3(110f, 23.85f, 290f)));
            transformaciones.Add(generateTransformation(new TGCVector3(0.15f, 0.15f, 0.15f), new TGCVector3(0, -FastMath.QUARTER_PI, 0), new TGCVector3(110f, 25.1f, 290f)));
            this.cocina.Add(this.GiveMeAnObject("MeshCreator\\Meshes\\Habitacion\\Libros\\Comun\\Comun-TgcScene.xml", transformaciones));
            this.cocina.Add(this.GiveMeAnObject("MeshCreator\\Meshes\\Habitacion\\Libros\\Arquitectura\\Arquitectura-TgcScene.xml", generateTransformation(new TGCVector3(0.15f, 0.15f, 0.15f), new TGCVector3(0, -FastMath.PI_HALF, 0), new TGCVector3(110f, 26.35f, 290f))));
            this.cocina.Add(this.GiveMeAnObject("MeshCreator\\Meshes\\Habitacion\\Caja\\Caja-TgcScene.xml", generateTransformation(new TGCVector3(0.25f, 0.26f, 0.25f), new TGCVector3(0, -FastMath.QUARTER_PI, 0), new TGCVector3(65f, 0f, 235f))));
            this.cocina.Add(this.GiveMeAnObject("MeshCreator\\Meshes\\Cocina\\Mueble\\Mueble-TgcScene.xml", generateTransformation(new TGCVector3(1f, 1f, 0.5f), new TGCVector3(0, FastMath.PI_HALF, 0), new TGCVector3(202f, 0f, 342f))));
            this.cocina.Add(this.GiveMeAnObject("MeshCreator\\Meshes\\Habitacion\\Toma\\Toma-TgcScene.xml", generateTransformation(new TGCVector3(0.5f, 0.5f, 0.5f), new TGCVector3(0, 0f, FastMath.PI_HALF), new TGCVector3(-110f, 15f, 144f))));

            //banio
            this.banio.Add(this.GiveMeAnObject("MeshCreator\\Meshes\\Bathroom\\Jabon\\Jabon-TgcScene.xml", generateTransformation(new TGCVector3(0.7f, 0.7f, 0.7f), new TGCVector3(0, 0, 0), new TGCVector3(-75f, 0f, 265f))));
            this.banio.Add(this.GiveMeAnObject("MeshCreator\\Meshes\\Bathroom\\Bathtub\\Bathtub-TgcScene.xml", generateTransformation(new TGCVector3(1.5f, 1.8f, 1.5f), new TGCVector3(0, 0, 0), new TGCVector3(-164f, 0f, 270f))));
            this.banio.Add(this.GiveMeAnObject("MeshCreator\\Meshes\\Bathroom\\InodoroCuadrado\\InodoroCuadrado-TgcScene.xml",generateTransformation(new TGCVector3(2f, 3f, 3f), new TGCVector3(0, 0, 0), new TGCVector3(-85, 0f, 175))));
            this.banio.Add(this.GiveMeAnObject("MeshCreator\\Meshes\\Bathroom\\Cepillo\\Cepillo-TgcScene.xml", generateTransformation(new TGCVector3(1.4f, 1.4f, 1.4f), new TGCVector3(0, FastMath.QUARTER_PI, 0), new TGCVector3(-125, 0f, 170f))));
            this.banio.Add(this.GiveMeAnObject("MeshCreator\\Meshes\\Bathroom\\Esponja\\Esponja-TgcScene.xml", generateTransformation(new TGCVector3(1f, 1f, 1f), new TGCVector3(0, FastMath.QUARTER_PI, 0), new TGCVector3(-165, 0f, 190))));
            this.banio.Add(this.GiveMeAnObject("MeshCreator\\Meshes\\Habitacion\\Banqueta\\Banqueta-TgcScene.xml", generateTransformation(new TGCVector3(0.4f, 0.35f, 0.4f), new TGCVector3(0, 0, 0), new TGCVector3(-170, 0f, 215))));
            this.banio.Add(this.GiveMeAnObject("MeshCreator\\Meshes\\Bathroom\\Espejo\\Espejo-TgcScene.xml", generateTransformation(new TGCVector3(1f, 1f, 1f), new TGCVector3(0, 0, 0), new TGCVector3(-90f, 0f, 298f))));

            //habitacion
            this.habitacion.Add(this.GiveMeAnObject("MeshCreator\\Meshes\\Habitacion\\Cama\\Cama-TgcScene.xml", generateTransformation(new TGCVector3(1, 1, 1), new TGCVector3(0, FastMath.PI, 0), new TGCVector3(-36f, 0, -124f))));
            this.habitacion.Add(this.GiveMeAnObject("MeshCreator\\Meshes\\Habitacion\\MesaDeLuz\\MesaDeLuz-TgcScene.xml", generateTransformation(new TGCVector3(1, 1, 1), new TGCVector3(0, FastMath.PI, 0), new TGCVector3(22f, 0, -158f))));
            this.habitacion.Add(this.GiveMeAnObject("MeshCreator\\Meshes\\Habitacion\\Placard\\Placard-TgcScene.xml", generateTransformation(new TGCVector3(1, 1, 1), new TGCVector3(0, -FastMath.PI_HALF, 0), new TGCVector3(-205f, 0, -122f))));
            this.habitacion.Add(this.GiveMeAnObject("MeshCreator\\Meshes\\Habitacion\\Armario\\Armario-TgcScene.xml", generateTransformation(new TGCVector3(1, 1, 1), new TGCVector3(0, -FastMath.PI_HALF, 0), new TGCVector3(-30f, 0, 110f))));
            this.habitacion.Add(this.GiveMeAnObject("MeshCreator\\Meshes\\Habitacion\\Escritorio\\Escritorio-TgcScene.xml", generateTransformation(new TGCVector3(1, 1, 1), new TGCVector3(0, 0, 0), new TGCVector3(183f, 0f, -107f))));
            this.habitacion.Add(this.GiveMeAnObject("MeshCreator\\Meshes\\Habitacion\\SillaEscritorio\\SillaEscritorio-TgcScene.xml", generateTransformation(new TGCVector3(0.5f, 0.3f, 0.5f), new TGCVector3(0, 0, 0), new TGCVector3(151f, -1f, -101f))));
            this.habitacion.Add(this.GiveMeAnObject("MeshCreator\\Meshes\\Habitacion\\CajaZapatillas\\CajaZapatillas-TgcScene.xml", generateTransformation(new TGCVector3(0.5f, 0.5f, 0.5f), new TGCVector3(0, FastMath.PI_HALF, 0), new TGCVector3(100f, 0f, -147f))));
            this.habitacion.Add(this.GiveMeAnObject("MeshCreator\\Meshes\\Habitacion\\Sillon\\Sillon-TgcScene.xml", generateTransformation(new TGCVector3(1f, 1f, 1f), new TGCVector3(0f, FastMath.PI+FastMath.PI_HALF, 0f), new TGCVector3(-180f, -0.5f, 20f))));
            this.habitacion.Add(this.GiveMeAnObject("MeshCreator\\Meshes\\Habitacion\\Escoba\\Escoba-TgcScene.xml", generateTransformation(new TGCVector3(1.3f, 1.3f, 1.3f), new TGCVector3(FastMath.PI_HALF, 0, 0), new TGCVector3(-105f, 1f, -60f))));
            this.habitacion.Add(this.GiveMeAnObject("MeshCreator\\Meshes\\Habitacion\\Rubik\\Rubik-TgcScene.xml", generateTransformation(new TGCVector3(0.4f, 0.4f, 0.4f), new TGCVector3(0, 0, 0), new TGCVector3(100f, 0f, -100f))));

            transformaciones = new List<TGCMatrix>();
            transformaciones.Add(generateTransformation(new TGCVector3(0.2f, 0.2f, 0.2f), new TGCVector3(0, 0, FastMath.PI_HALF), new TGCVector3(-10f, 1f, 3f)));
            transformaciones.Add(generateTransformation(new TGCVector3(0.2f, 0.2f, 0.2f), new TGCVector3(0, 1f, 0), new TGCVector3(-75f, 0f, -100f)));
            transformaciones.Add(generateTransformation(new TGCVector3(0.2f, 0.2f, 0.2f), new TGCVector3(0, 0f, FastMath.PI_HALF), new TGCVector3(75f, 1f, 10f)));
            this.habitacion.Add(this.GiveMeAnObject("MeshCreator\\Meshes\\Habitacion\\Rastis\\Rasti-1\\Rastis-1-TgcScene.xml", transformaciones));

            transformaciones = new List<TGCMatrix>();
            transformaciones.Add(generateTransformation(new TGCVector3(0.2f, 0.2f, 0.2f), new TGCVector3(0, 1f, 0f), new TGCVector3(45f, 0f, 20f)));
            transformaciones.Add(generateTransformation(new TGCVector3(0.2f, 0.2f, 0.2f), new TGCVector3(0, 2f, 0f), new TGCVector3(40f, 0f, -20f)));
            transformaciones.Add(generateTransformation(new TGCVector3(0.2f, 0.2f, 0.2f), new TGCVector3(0, 3f, 0f), new TGCVector3(-50f, 0f, 20f)));
            this.habitacion.Add(this.GiveMeAnObject("MeshCreator\\Meshes\\Habitacion\\Rastis\\Rasti-2\\Rastis-2-TgcScene.xml", transformaciones));

            transformaciones = new List<TGCMatrix>();
            transformaciones.Add(generateTransformation(new TGCVector3(0.2f, 0.2f, 0.2f), new TGCVector3(0, 0f, 0f), new TGCVector3(22f, 0f, 0f)));
            transformaciones.Add(generateTransformation(new TGCVector3(0.2f, 0.2f, 0.2f), new TGCVector3(0, 2f, 0f), new TGCVector3(-100f, 0f, 45f)));
            transformaciones.Add(generateTransformation(new TGCVector3(0.2f, 0.2f, 0.2f), new TGCVector3(0, 1f, 0f), new TGCVector3(100f, 0f, 50f)));
            this.habitacion.Add(this.GiveMeAnObject("MeshCreator\\Meshes\\Habitacion\\Rastis\\Rasti-4\\Rastis-4-TgcScene.xml", transformaciones));

            transformaciones = new List<TGCMatrix>();
            transformaciones.Add(generateTransformation(new TGCVector3(0.2f, 0.2f, 0.2f), new TGCVector3(0, 1f, 0f), new TGCVector3(0f, 0f, -50f)));
            transformaciones.Add(generateTransformation(new TGCVector3(0.2f, 0.2f, 0.2f), new TGCVector3(0, 1f, 0f), new TGCVector3(-120f, 0f, -10f)));
            transformaciones.Add(generateTransformation(new TGCVector3(0.2f, 0.2f, 0.2f), new TGCVector3(0, 1f, 0f), new TGCVector3(-200f, 0f, -200f)));
            this.habitacion.Add(this.GiveMeAnObject("MeshCreator\\Meshes\\Habitacion\\Rastis\\Rasti-6\\Rastis-6-TgcScene.xml", transformaciones));

            transformaciones = new List<TGCMatrix>();
            transformaciones.Add(generateTransformation(new TGCVector3(0.5f, 0.5f, 0.5f), new TGCVector3(0, 0f, FastMath.PI_HALF), new TGCVector3(60f, 15f, 378f)));
            transformaciones.Add(generateTransformation(new TGCVector3(0.5f, 0.5f, 0.5f), new TGCVector3(0, 0f, FastMath.PI_HALF), new TGCVector3(60f, 15f, 144f)));
            this.habitacion.Add(this.GiveMeAnObject("MeshCreator\\Meshes\\Habitacion\\Toma\\Toma-TgcScene.xml", transformaciones));

            this.habitacion.Add(this.GiveMeAnObject("MeshCreator\\Meshes\\Habitacion\\Pelota\\Pelota-TgcScene.xml", generateTransformation(new TGCVector3(0.4f, 0.4f, 0.4f), new TGCVector3(0, 0f, 0), new TGCVector3(153f, 0f, 85f))));
            
            
            
            //this.toalla = this.dameMesh("MeshCreator\\Meshes\\Bathroom\\Toalla\\Toalla-TgcScene.xml", new TGCVector3(1f, 1f, 1f), new TGCVector3(0, FastMath.QUARTER_PI, 0), new TGCVector3(-90, 0f, 245));

        }

        private SceneElement GiveMeAnObject(string ruta, TGCMatrix transformacion)
        {
            TgcScene tgcScene = new TgcSceneLoader().loadSceneFromFile(mediaDir + ruta);
            SceneElement nuevoObjeto = new SceneElement(tgcScene.Meshes, transformacion);
            return nuevoObjeto;

        }

        private SceneElement GiveMeAnObject(string ruta, List<TGCMatrix> transformations)
        {
            TgcScene tgcScene = new TgcSceneLoader().loadSceneFromFile(mediaDir + ruta);
            SceneRepeatedElement nuevoObjeto = new SceneRepeatedElement(tgcScene.Meshes, transformations);
            return nuevoObjeto;
        }

        private TGCMatrix generateTransformation(TGCVector3 escala, TGCVector3 rotacion, TGCVector3 traslado)
        {
            TGCMatrix matrixEscalado = TGCMatrix.Scaling(escala);
            TGCMatrix matrixRotacionX = TGCMatrix.RotationX(rotacion.X);
            TGCMatrix matrixRotacionY = TGCMatrix.RotationY(rotacion.Y);
            TGCMatrix matrixRotacionZ = TGCMatrix.RotationZ(rotacion.Z);
            TGCMatrix matrixRotacion = matrixRotacionX * matrixRotacionY * matrixRotacionZ;
            TGCMatrix matrixTraslacion = TGCMatrix.Translation(traslado);
            TGCMatrix transformacion = matrixEscalado * matrixRotacion * matrixTraslacion;
            return transformacion;
        }

        public void Render()
        {
            this.scene.Render();
            this.UbicacionVehiculo().Render();
        }

        public void Dispose()
        {
            this.scene.Dispose();
            this.habitacion.Dispose();
            this.cocina.Dispose();
            this.banio.Dispose();
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
