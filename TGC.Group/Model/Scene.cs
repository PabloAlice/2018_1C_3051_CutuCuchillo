using System;
using System.Collections.Generic;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Group.Model.Vehiculos;

namespace TGC.Group.Model
{
    class Scene
    {
        private static Scene instance;
        public Vehicle auto, AI;
        private Section cocina, banio, habitacion;
        public ThirdPersonCamera camera;

        private Scene()
        {
            this.cocina = new Section(new TGCVector3(-52, 0, 145), new TGCVector3(227, 0, 380));
            this.habitacion = new Section(new TGCVector3(-221,0,-174), new TGCVector3(227,0,145));
            this.banio = new Section(new TGCVector3(-221, 0, 145), new TGCVector3(-52, 0, 300));

        }

        public static Scene GetInstance()
        {
            if (instance == null)
            {
                instance = new Scene();
            }

            return instance;
        }

        public void SetVehiculo(Vehicle auto)
        {
            this.auto = auto;
        }

        public void HandleCollisions()
        {
            this.auto.HandleCollisions(this.AI);
            this.VehicleUbication(this.auto).HandleCollisions(this.auto);
            this.VehicleUbication(this.AI).HandleCollisions(this.AI);
            
        }

        private bool IsBetween(TGCVector3 interes, TGCVector3 pmin, TGCVector3 pmax)
        {
            return interes.X > pmin.X && interes.X < pmax.X && interes.Z > pmin.Z && interes.Z < pmax.Z;
        }

        private bool IsIn(Section seccion, TGCVector3 position)
        {
            return IsBetween(position, seccion.GetPuntoMinimo(), seccion.GetPuntoMaximo());
            
        }

        public List<Collidable> GetWeapons(Vehicle car)
        {
            return this.VehicleUbication(car).GetWeapons();
        }

        public void AddToSection(Collidable element)
        {
            TGCVector3 position = element.GetPosition();
            if(this.IsIn(this.habitacion, position))
            {
                this.habitacion.AddElement(element);
            }
            else if(this.IsIn(this.cocina, position))
            {
                this.cocina.AddElement(element);
            }
            else if(this.IsIn(this.banio, position))
            {
                this.banio.AddElement(element);
            }
            else
            {
                throw new System.Exception("El Elemento Que quizo agregar, se encuentra fuera de todas las secciones");
            }
        }

        public List<Collidable> GetPosiblesCollidables(Vehicle car)
        {
            return this.VehicleUbication(car).GetPosiblesCollidables(car.GetPosition());
        }

        public List<Collidable> GetPosiblesCollidables(Weapon weapon)
        {
            return this.WeaponUbication(weapon).GetPosiblesCollidables(weapon.GetPosition());
        }

        private Section VehicleUbication(Vehicle car)
        {
            TGCVector3 position = car.GetPosition();
            if (this.IsIn(this.cocina, position))
            {
                return this.cocina;
            }
            else if (this.IsIn(this.habitacion, position))
            {
                return this.habitacion;
            }
            else if(this.IsIn(this.banio, position))
            {
                return this.banio;
            }
            throw new Exception("Saliste del escenario");
        }

        private Section WeaponUbication(Weapon weapon)
        {
            TGCVector3 position = weapon.GetPosition();
            if (this.IsIn(this.cocina, position))
            {
                return this.cocina;
            }
            else if (this.IsIn(this.habitacion, position))
            {
                return this.habitacion;
            }
            else
            {
                return this.banio;
            }
        }

        public void SetCamera(ThirdPersonCamera camera)
        {
            this.camera = camera;
        }

        public void Init()
        {

            GlobalConcepts GlobalConcepts = GlobalConcepts.GetInstance();
            TgcMesh plane;
            List<TgcMesh> list;
            SceneElement aux;
            TransformationMatrix initMatrix;
            TgcMesh weapon;

            weapon = this.GimeMeASingleMesh("MeshCreator\\Meshes\\Otros\\Weapons\\Bomba\\Bomba-TgcScene.xml");
            initMatrix = new TransformationMatrix();
            initMatrix.SetScalation(TGCMatrix.Scaling(0.02f, 0.02f, 0.02f));
            initMatrix.SetRotation(TGCMatrix.RotationYawPitchRoll(0,0,0));
            initMatrix.SetTranslation(TGCMatrix.Translation(new TGCVector3(0, 0.35f, 0)));
            this.habitacion.AddElement(new Bomb(initMatrix, weapon));

            weapon = this.GimeMeASingleMesh("MeshCreator\\Meshes\\Otros\\Weapons\\Misil\\Misil-TgcScene.xml");
            initMatrix = new TransformationMatrix();
            initMatrix.SetScalation(TGCMatrix.Scaling(0.02f, 0.02f, 0.02f));
            initMatrix.SetRotation(TGCMatrix.RotationYawPitchRoll(0, 0, 0));
            initMatrix.SetTranslation(TGCMatrix.Translation(new TGCVector3(-120f, 0.35f, 0)));
            this.habitacion.AddElement(new Misile(initMatrix, weapon));

            weapon = this.GimeMeASingleMesh("MeshCreator\\Meshes\\Otros\\Weapons\\BolaHielo\\BolaHielo-TgcScene.xml");
            initMatrix = new TransformationMatrix();
            initMatrix.SetScalation(TGCMatrix.Scaling(0.02f, 0.02f, 0.02f));
            initMatrix.SetRotation(TGCMatrix.RotationYawPitchRoll(0, 0, 0));
            initMatrix.SetTranslation(TGCMatrix.Translation(new TGCVector3(-120f, 0.35f, -30)));
            this.habitacion.AddElement(new BolaHielo(initMatrix, weapon));

            TgcScene escena = new TgcSceneLoader().loadSceneFromFile(GlobalConcepts.GetInstance().GetMediaDir() + "Texturas\\Habitacion\\escenaMesheada-TgcScene.xml");
            escena.Meshes.ForEach((mesh) => {
                var adj = new int[mesh.D3dMesh.NumberFaces * 3];
                mesh.D3dMesh.GenerateAdjacency(0, adj);
                mesh.D3dMesh.ComputeNormals(adj);
            });
            //Habitacion
            //piso
            this.habitacion.AddElement(new Plane(new TGCVector3(-221,0,-174), new TGCVector3(227, 180, 145), new TGCVector3(0,1,0), "Habitacion\\Paredes\\1.jpg"), true);
            //pared izquierda
            this.habitacion.AddElement(new Plane(new TGCVector3(-221, 0, -174), new TGCVector3(-221, 180, 145), new TGCVector3(1, 0, 0), "Habitacion\\Paredes\\2.jpg"), true);
            //pared trasera
            this.habitacion.AddElement(new Plane(new TGCVector3(-221, 0, -174), new TGCVector3(227, 180, 0), new TGCVector3(0, 0, 1), "Habitacion\\Paredes\\3.jpg"), true);
            //pared derecha
            this.habitacion.AddElement(new Plane(new TGCVector3(227, 0, -174), new TGCVector3(227, 180, 145), new TGCVector3(-1, 0, 0), "Habitacion\\Paredes\\2.jpg"), true);
            //pared derecha de la puerta derecha
            //this.habitacion.AddElement(new Plane(new TGCVector3(227, 0, 145), new TGCVector3(170, 180, 145), new TGCVector3(0, 0, -1), "Habitacion\\Paredes\\2.jpg"), true);
            //pared frontal
            this.habitacion.AddElement(new Plane(new TGCVector3(-221, 0, 145), new TGCVector3(227, 180, 145), new TGCVector3(0, 0, -1), "Habitacion\\Paredes\\2.jpg"), true);
            //pared izquierda de la puerta izquierda
            //this.habitacion.AddElement(new Plane(new TGCVector3(-221, 0, 145), new TGCVector3(-211, 180, 145), new TGCVector3(0, 0, -1), "Habitacion\\Paredes\\2.jpg"), true);
            //techo
            this.habitacion.AddElement(new Plane(new TGCVector3(-221, 180, -174), new TGCVector3(227, 180, 145), new TGCVector3(0, -1, 0), "Habitacion\\Paredes\\4.jpg"), true);

            //cocina
            //piso
            this.cocina.AddElement(new Plane(new TGCVector3(-52, 0, 145), new TGCVector3(227, 180, 380), new TGCVector3(0, 1, 0), "Cocina\\Paredes\\1.jpg"), true);
            //pared izquierda de la puerta derecha
            //this.cocina.AddElement(new Plane(new TGCVector3(-52, 0, 145), new TGCVector3(92, 180, 145), new TGCVector3(0, 0, 1), "Cocina\\Paredes\\2.jpg"), true);
            //pared izquierda de la puerta derecha
            //this.cocina.AddElement(new Plane(new TGCVector3(170, 0, 145), new TGCVector3(227, 180, 145), new TGCVector3(0, 0, 1), "Cocina\\Paredes\\2.jpg"), true);
            //pared trasera
            this.cocina.AddElement(new Plane(new TGCVector3(-52, 0, 145), new TGCVector3(227, 180, 145), new TGCVector3(0, 0, 1), "Cocina\\Paredes\\2.jpg"), true);
            //pared derecha
            this.cocina.AddElement(new Plane(new TGCVector3(227, 0, 145), new TGCVector3(227, 180, 380), new TGCVector3(-1, 0, 0), "Cocina\\Paredes\\2.jpg"), true);
            //pared Frontal
            this.cocina.AddElement(new Plane(new TGCVector3(-52, 0, 380), new TGCVector3(227, 180, 380), new TGCVector3(0, 0, -1), "Cocina\\Paredes\\2.jpg"), true);
            //pared derecha de puerta izquierda
            //this.cocina.AddElement(new Plane(new TGCVector3(-52, 0, 380), new TGCVector3(-52, 180, 292), new TGCVector3(1, 0, 0), "Cocina\\Paredes\\2.jpg"), true);
            //pared izquierda de puerta izquierda
            //this.cocina.AddElement(new Plane(new TGCVector3(-52, 0, 229), new TGCVector3(-52, 180, 145), new TGCVector3(1, 0, 0), "Cocina\\Paredes\\2.jpg"), true);
            //pared izquierda
            this.cocina.AddElement(new Plane(new TGCVector3(-52, 0, 145), new TGCVector3(-52, 180, 380), new TGCVector3(1, 0, 0), "Cocina\\Paredes\\2.jpg"), true);
            //techo
            this.cocina.AddElement(new Plane(new TGCVector3(-52, 180, 145), new TGCVector3(227, 180, 380), new TGCVector3(0, -1, 0), "Cocina\\Paredes\\2.jpg"), true);
            /*
            //pared izquierda pieza
            plane = escena.Meshes[0];
            list = new List<TgcMesh>();
            list.Add(plane);
            this.habitacion.AddElement(new SceneElement(list, TGCMatrix.Scaling(new TGCVector3(1, 1, 1)) * TGCMatrix.RotationYawPitchRoll(0, 0, 0) * TGCMatrix.Translation(new TGCVector3(0, 0, 0))), true);

            //marco derecho puerta cocina-habitacion
            plane = escena.Meshes[1];
            list = new List<TgcMesh>();
            list.Add(plane);
            aux = new SceneElement(list, TGCMatrix.Scaling(new TGCVector3(1, 1, 1)) * TGCMatrix.RotationYawPitchRoll(0, 0, 0) * TGCMatrix.Translation(new TGCVector3(0, 0, 0)));
            this.habitacion.AddElement(aux, true);
            this.cocina.AddElement(aux, true);

            //marco izquierda puerta cocina-habitacion
            plane = escena.Meshes[2];
            list = new List<TgcMesh>();
            list.Add(plane);
            aux = new SceneElement(list, TGCMatrix.Scaling(new TGCVector3(1, 1, 1)) * TGCMatrix.RotationYawPitchRoll(0, 0, 0) * TGCMatrix.Translation(new TGCVector3(0, 0, 0)));
            this.habitacion.AddElement(aux, true);
            this.cocina.AddElement(aux, true);


            //marco puerta banio-habitacion
            plane = escena.Meshes[3];
            list = new List<TgcMesh>();
            list.Add(plane);
            aux = new SceneElement(list, TGCMatrix.Scaling(new TGCVector3(1, 1, 1)) * TGCMatrix.RotationYawPitchRoll(0, 0, 0) * TGCMatrix.Translation(new TGCVector3(0, 0, 0)));
            this.habitacion.AddElement(aux, true);
            this.banio.AddElement(aux, true);


            //marco puerta banio-habitacion
            plane = escena.Meshes[4];
            list = new List<TgcMesh>();
            list.Add(plane);
            aux = new SceneElement(list, TGCMatrix.Scaling(new TGCVector3(1, 1, 1)) * TGCMatrix.RotationYawPitchRoll(0, 0, 0) * TGCMatrix.Translation(new TGCVector3(0, 0, 0)));
            this.habitacion.AddElement(aux, true);
            this.banio.AddElement(aux, true);


            //marco banio-cocina
            plane = escena.Meshes[5];
            list = new List<TgcMesh>();
            list.Add(plane);
            aux = new SceneElement(list, TGCMatrix.Scaling(new TGCVector3(1, 1, 1)) * TGCMatrix.RotationYawPitchRoll(0, 0, 0) * TGCMatrix.Translation(new TGCVector3(0, 0, 0)));
            this.banio.AddElement(aux, true);
            this.cocina.AddElement(aux, true);


            //marco banio-cocina
            plane = escena.Meshes[6];
            list = new List<TgcMesh>();
            list.Add(plane);
            aux = new SceneElement(list, TGCMatrix.Scaling(new TGCVector3(1, 1, 1)) * TGCMatrix.RotationYawPitchRoll(0, 0, 0) * TGCMatrix.Translation(new TGCVector3(0, 0, 0)));
            this.banio.AddElement(aux, true);
            this.cocina.AddElement(aux, true);


            //piso habitacion
            plane = escena.Meshes[7];
            list = new List<TgcMesh>();
            list.Add(plane);
            this.habitacion.AddElement(new SceneElement(list, TGCMatrix.Scaling(new TGCVector3(1, 1, 1)) * TGCMatrix.RotationYawPitchRoll(0, 0, 0) * TGCMatrix.Translation(new TGCVector3(0, 0, 0))), true);
            
            //pared trasera habitacion
            plane = escena.Meshes[8];
            list = new List<TgcMesh>();
            list.Add(plane);
            this.habitacion.AddElement(new SceneElement(list, TGCMatrix.Scaling(new TGCVector3(1, 1, 1)) * TGCMatrix.RotationYawPitchRoll(0, 0, 0) * TGCMatrix.Translation(new TGCVector3(0, 0, 0))), true);


            //pared derecha habitacion
            plane = escena.Meshes[9];
            list = new List<TgcMesh>();
            list.Add(plane);
            this.habitacion.AddElement(new SceneElement(list, TGCMatrix.Scaling(new TGCVector3(1, 1, 1)) * TGCMatrix.RotationYawPitchRoll(0, 0, 0) * TGCMatrix.Translation(new TGCVector3(0, 0, 0))), true);


            //techo habitacion
            plane = escena.Meshes[10];
            list = new List<TgcMesh>();
            list.Add(plane);
            this.habitacion.AddElement(new SceneElement(list, TGCMatrix.Scaling(new TGCVector3(1, 1, 1)) * TGCMatrix.RotationYawPitchRoll(0, 0, 0) * TGCMatrix.Translation(new TGCVector3(0, 0, 0))), true);


            //piso banio
            plane = escena.Meshes[11];
            list = new List<TgcMesh>();
            list.Add(plane);
            this.banio.AddElement(new SceneElement(list, TGCMatrix.Scaling(new TGCVector3(1, 1, 1)) * TGCMatrix.RotationYawPitchRoll(0, 0, 0) * TGCMatrix.Translation(new TGCVector3(0, 0, 0))), true);


            //pared izquierda banio
            plane = escena.Meshes[12];
            list = new List<TgcMesh>();
            list.Add(plane);
            this.banio.AddElement(new SceneElement(list, TGCMatrix.Scaling(new TGCVector3(1, 1, 1)) * TGCMatrix.RotationYawPitchRoll(0, 0, 0) * TGCMatrix.Translation(new TGCVector3(0, 0, 0))), true);


            //pared frontal banio
            plane = escena.Meshes[13];
            list = new List<TgcMesh>();
            list.Add(plane);
            this.banio.AddElement(new SceneElement(list, TGCMatrix.Scaling(new TGCVector3(1, 1, 1)) * TGCMatrix.RotationYawPitchRoll(0, 0, 0) * TGCMatrix.Translation(new TGCVector3(0, 0, 0))), true);

            
            //techo banio
            plane = escena.Meshes[14];
            list = new List<TgcMesh>();
            list.Add(plane);
            this.banio.AddElement(new SceneElement(list, TGCMatrix.Scaling(new TGCVector3(1, 1, 1)) * TGCMatrix.RotationYawPitchRoll(0, 0, 0) * TGCMatrix.Translation(new TGCVector3(0, 0, 0))), true);

            
            //piso cocina
            plane = escena.Meshes[15];
            list = new List<TgcMesh>();
            list.Add(plane);
            this.cocina.AddElement(new SceneElement(list, TGCMatrix.Scaling(new TGCVector3(1, 1, 1)) * TGCMatrix.RotationYawPitchRoll(0, 0, 0) * TGCMatrix.Translation(new TGCVector3(0, 0, 0))), true);

            //pared frontal cocina
            plane = escena.Meshes[16];
            list = new List<TgcMesh>();
            list.Add(plane);
            this.cocina.AddElement(new SceneElement(list, TGCMatrix.Scaling(new TGCVector3(1, 1, 1)) * TGCMatrix.RotationYawPitchRoll(0, 0, 0) * TGCMatrix.Translation(new TGCVector3(0, 0, 0))), true);

            //pared derecha cocina
            plane = escena.Meshes[17];
            list = new List<TgcMesh>();
            list.Add(plane);
            this.cocina.AddElement(new SceneElement(list, TGCMatrix.Scaling(new TGCVector3(1, 1, 1)) * TGCMatrix.RotationYawPitchRoll(0, 0, 0) * TGCMatrix.Translation(new TGCVector3(0, 0, 0))), true);

            //pared habitacion
            plane = escena.Meshes[18];
            list = new List<TgcMesh>();
            list.Add(plane);
            this.habitacion.AddElement(new SceneElement(list, TGCMatrix.Scaling(new TGCVector3(1, 1, 1)) * TGCMatrix.RotationYawPitchRoll(0, 0, 0) * TGCMatrix.Translation(new TGCVector3(0, 0, 0))), true);


            //pared cocina
            plane = escena.Meshes[19];
            list = new List<TgcMesh>();
            list.Add(plane);
            this.cocina.AddElement(new SceneElement(list, TGCMatrix.Scaling(new TGCVector3(1, 1, 1)) * TGCMatrix.RotationYawPitchRoll(0, 0, 0) * TGCMatrix.Translation(new TGCVector3(0, 0, 0))), true);
            
            //techo cocina
            plane = escena.Meshes[20];
            list = new List<TgcMesh>();
            list.Add(plane);
            this.cocina.AddElement(new SceneElement(list, TGCMatrix.Scaling(new TGCVector3(1, 1, 1)) * TGCMatrix.RotationYawPitchRoll(0, 0, 0) * TGCMatrix.Translation(new TGCVector3(0, 0, 0))), true);

            
            //pared cocina
            plane = escena.Meshes[21];
            list = new List<TgcMesh>();
            list.Add(plane);
            this.cocina.AddElement(new SceneElement(list, TGCMatrix.Scaling(new TGCVector3(1, 1, 1)) * TGCMatrix.RotationYawPitchRoll(0, 0, 0) * TGCMatrix.Translation(new TGCVector3(0, 0, 0))), true);

            //pared frontal habitacion
            plane = escena.Meshes[22];
            list = new List<TgcMesh>();
            list.Add(plane);
            this.habitacion.AddElement(new SceneElement(list, TGCMatrix.Scaling(new TGCVector3(1, 1, 1)) * TGCMatrix.RotationYawPitchRoll(0, 0, 0) * TGCMatrix.Translation(new TGCVector3(0, 0, 0))), true);

            //pared trasera cocina
            plane = escena.Meshes[23];
            list = new List<TgcMesh>();
            list.Add(plane);
            this.cocina.AddElement(new SceneElement(list, TGCMatrix.Scaling(new TGCVector3(1, 1, 1)) * TGCMatrix.RotationYawPitchRoll(0, 0, 0) * TGCMatrix.Translation(new TGCVector3(0, 0, 0))), true);

            //pared banio
            plane = escena.Meshes[24];
            list = new List<TgcMesh>();
            list.Add(plane);
            this.banio.AddElement(new SceneElement(list, TGCMatrix.Scaling(new TGCVector3(1, 1, 1)) * TGCMatrix.RotationYawPitchRoll(0, 0, 0) * TGCMatrix.Translation(new TGCVector3(0, 0, 0))), true);

            //pared derecha banio
            plane = escena.Meshes[25];
            list = new List<TgcMesh>();
            list.Add(plane);
            this.banio.AddElement(new SceneElement(list, TGCMatrix.Scaling(new TGCVector3(1, 1, 1)) * TGCMatrix.RotationYawPitchRoll(0, 0, 0) * TGCMatrix.Translation(new TGCVector3(0, 0, 0))), true);
            
            //pared cocina
            plane = escena.Meshes[26];
            list = new List<TgcMesh>();
            list.Add(plane);
            this.cocina.AddElement(new SceneElement(list, TGCMatrix.Scaling(new TGCVector3(1, 1, 1)) * TGCMatrix.RotationYawPitchRoll(0, 0, 0) * TGCMatrix.Translation(new TGCVector3(0, 0, 0))), true);
            */
            //cocina
            list = new List<TgcMesh>();
            list = this.GiveMeAMesh("MeshCreator\\Meshes\\Cocina\\DispenserAgua\\DispenserAgua-TgcScene.xml");
            this.cocina.AddElement(new SceneElement(list, GlobalConcepts.GenerateTransformation(new TGCVector3(1f, 1f, 1f), new TGCVector3(0, 0, 0), new TGCVector3(-32f, -1f, 360f))));
            list = new List<TgcMesh>();
            list = this.GiveMeAMesh("MeshCreator\\Meshes\\Cocina\\Mesa\\Mesa-TgcScene.xml");
            this.cocina.AddElement(new SceneElement(list, GlobalConcepts.GenerateTransformation(new TGCVector3(1f, 1f, 1f), new TGCVector3(0, FastMath.PI_HALF, 0), new TGCVector3(94f, 0f, 335f))));
            list = new List<TgcMesh>();
            list = this.GiveMeAMesh("MeshCreator\\Meshes\\Cocina\\Heladera\\Heladera-TgcScene.xml");
            this.cocina.AddElement(new SceneElement(list, GlobalConcepts.GenerateTransformation(new TGCVector3(1f, 1f, 1f), new TGCVector3(0, 0, 0), new TGCVector3(191f, -1f, 184f))));
            list = new List<TgcMesh>();
            list = this.GiveMeAMesh("MeshCreator\\Meshes\\Cocina\\Tacho\\Tacho-TgcScene.xml");
            this.cocina.AddElement(new SceneElement(list, GlobalConcepts.GenerateTransformation(new TGCVector3(0.6f, 0.6f, 0.6f), new TGCVector3(0, 0, 0), new TGCVector3(-30f, -1f, 165f))));

            list = new List<TgcMesh>();
            list = this.GiveMeAMesh("MeshCreator\\Meshes\\Cocina\\Silla\\Silla-TgcScene.xml");
            this.cocina.AddElement(new SceneElement(list, GlobalConcepts.GenerateTransformation(new TGCVector3(0.4f, 0.4f, 0.4f), new TGCVector3(0, 0, 0), new TGCVector3(116f, 0f, 295f))));
            this.cocina.AddElement(new SceneElement(list, GlobalConcepts.GenerateTransformation(new TGCVector3(0.4f, 0.4f, 0.4f), new TGCVector3(0, 0, 0), new TGCVector3(90f, 0f, 265f))));
            this.cocina.AddElement(new SceneElement(list, GlobalConcepts.GenerateTransformation(new TGCVector3(0.4f, 0.4f, 0.4f), new TGCVector3(0, -FastMath.PI_HALF, 0), new TGCVector3(140f, 0f, 335f))));
            list = new List<TgcMesh>();
            list = this.GiveMeAMesh("MeshCreator\\Meshes\\Cocina\\Mueble\\Mueble-TgcScene.xml");
            this.cocina.AddElement(new SceneElement(list, GlobalConcepts.GenerateTransformation(new TGCVector3(1f, 1f, 0.5f), new TGCVector3(0, 0, 0), new TGCVector3(14f, -1f, 357f))));

            list = new List<TgcMesh>();
            list = this.GiveMeAMesh("MeshCreator\\Meshes\\Habitacion\\Libros\\Comun\\Comun-TgcScene.xml");
            this.cocina.AddElement(new SceneElement(list, GlobalConcepts.GenerateTransformation(new TGCVector3(0.15f, 0.15f, 0.15f), new TGCVector3(0, 0, 0), new TGCVector3(110f, 23.85f, 290f))));
            this.cocina.AddElement(new SceneElement(list, GlobalConcepts.GenerateTransformation(new TGCVector3(0.15f, 0.15f, 0.15f), new TGCVector3(0, 0, 0), new TGCVector3(110f, 25.1f, 290f))));

            list = new List<TgcMesh>();
            list = this.GiveMeAMesh("MeshCreator\\Meshes\\Habitacion\\Libros\\Arquitectura\\Arquitectura-TgcScene.xml");
            this.cocina.AddElement(new SceneElement(list, GlobalConcepts.GenerateTransformation(new TGCVector3(0.15f, 0.15f, 0.15f), new TGCVector3(0, -FastMath.PI_HALF, 0), new TGCVector3(110f, 26.35f, 290f))));
            list = new List<TgcMesh>();
            list = this.GiveMeAMesh("MeshCreator\\Meshes\\Habitacion\\Caja\\Caja-TgcScene.xml");
            this.cocina.AddElement(new SceneElement(list, GlobalConcepts.GenerateTransformation(new TGCVector3(0.25f, 0.26f, 0.25f), new TGCVector3(0, -FastMath.QUARTER_PI, 0), new TGCVector3(65f, 0f, 235f))));
            list = new List<TgcMesh>();
            list = this.GiveMeAMesh("MeshCreator\\Meshes\\Cocina\\Mueble\\Mueble-TgcScene.xml");
            this.cocina.AddElement(new SceneElement(list, GlobalConcepts.GenerateTransformation(new TGCVector3(1f, 1f, 0.5f), new TGCVector3(0, FastMath.PI_HALF, 0), new TGCVector3(202f, 0f, 342f))));
            list = new List<TgcMesh>();
            list = this.GiveMeAMesh("MeshCreator\\Meshes\\Habitacion\\Toma\\Toma-TgcScene.xml");
            this.cocina.AddElement(new SceneElement(list, GlobalConcepts.GenerateTransformation(new TGCVector3(0.5f, 0.5f, 0.5f), new TGCVector3(0, 0f, FastMath.PI_HALF), new TGCVector3(60f, 15f, 378f))));

            list = new List<TgcMesh>();
            list = this.GiveMeAMesh("MeshCreator\\Meshes\\Cocina\\CopaMadera\\CopaMadera-TgcScene.xml");
            this.cocina.AddElement(new SceneElement(list, GlobalConcepts.GenerateTransformation(new TGCVector3(0.07f, 0.07f, 0.07f), new TGCVector3(0, 0, 0), new TGCVector3(94f, 43.5f, 340f))));
            this.cocina.AddElement(new SceneElement(list, GlobalConcepts.GenerateTransformation(new TGCVector3(0.07f, 0.07f, 0.07f), new TGCVector3(0, FastMath.PI, 0), new TGCVector3(70f, 43.5f, 315f))));
            this.cocina.AddElement(new SceneElement(list, GlobalConcepts.GenerateTransformation(new TGCVector3(0.07f, 0.07f, 0.07f), new TGCVector3(0, FastMath.PI_HALF, 0), new TGCVector3(125f, 43.5f, 355f))));
            this.cocina.AddElement(new SceneElement(list, GlobalConcepts.GenerateTransformation(new TGCVector3(0.07f, 0.07f, 0.07f), new TGCVector3(0, -FastMath.PI_HALF, 0), new TGCVector3(130f, 43.5f, 325f))));

            list = new List<TgcMesh>();
            list = this.GiveMeAMesh("MeshCreator\\Meshes\\Cocina\\Tenedor\\Tenedor-TgcScene.xml");
            this.cocina.AddElement(new SceneElement(list, GlobalConcepts.GenerateTransformation(new TGCVector3(0.07f, 0.07f, 0.07f), new TGCVector3(0, 0, 0), new TGCVector3(120f, 39.3f, 312f))));
            this.cocina.AddElement(new SceneElement(list, GlobalConcepts.GenerateTransformation(new TGCVector3(0.07f, 0.07f, 0.07f), new TGCVector3(0, -FastMath.PI_HALF, 0), new TGCVector3(50f, 39.3f, 350f))));
            this.cocina.AddElement(new SceneElement(list, GlobalConcepts.GenerateTransformation(new TGCVector3(0.07f, 0.07f, 0.07f), new TGCVector3(0, FastMath.PI, 0), new TGCVector3(80f, 39.3f, 340f))));
            this.cocina.AddElement(new SceneElement(list, GlobalConcepts.GenerateTransformation(new TGCVector3(0.07f, 0.07f, 0.07f), new TGCVector3(0, -FastMath.PI, 0), new TGCVector3(130f, 39.3f, 330f))));
            this.cocina.AddElement(new SceneElement(list, GlobalConcepts.GenerateTransformation(new TGCVector3(0.07f, 0.07f, 0.07f), new TGCVector3(0, 0, 0), new TGCVector3(105f, 39.3f, 327f))));
            this.cocina.AddElement(new SceneElement(list, GlobalConcepts.GenerateTransformation(new TGCVector3(0.07f, 0.07f, 0.07f), new TGCVector3(0, FastMath.PI_HALF, 0), new TGCVector3(67f, 39.3f, 328f))));

            list = new List<TgcMesh>();
            list = this.GiveMeAMesh("MeshCreator\\Meshes\\Cocina\\Esponja\\Esponja-TgcScene.xml");
            this.cocina.AddElement(new SceneElement(list, GlobalConcepts.GenerateTransformation(new TGCVector3(0.2f, 0.2f, 0.2f), new TGCVector3(0, 0, 0), new TGCVector3(94f, 39.3f, 325f))));
            list = new List<TgcMesh>();
            list = this.GiveMeAMesh("MeshCreator\\Meshes\\Cocina\\Mueble2\\Mueble2-TgcScene.xml");
            this.cocina.AddElement(new SceneElement(list, GlobalConcepts.GenerateTransformation(new TGCVector3(1f, 1f, 0.5f), new TGCVector3(0, FastMath.PI_HALF, 0), new TGCVector3(202f, 0f, 270f))));

            list = new List<TgcMesh>();
            list = this.GiveMeAMesh("MeshCreator\\Meshes\\Cocina\\MueblePared\\MueblePared-TgcScene.xml");
            this.cocina.AddElement(new SceneElement(list, GlobalConcepts.GenerateTransformation(new TGCVector3(0.65f, 0.65f, 0.65f), new TGCVector3(0, -FastMath.PI_HALF, 0), new TGCVector3(202f, 100f, 270f))));
            this.cocina.AddElement(new SceneElement(list, GlobalConcepts.GenerateTransformation(new TGCVector3(0.65f, 0.65f, 0.65f), new TGCVector3(0, -FastMath.PI_HALF, 0), new TGCVector3(202f, 100f, 342f))));

            list = new List<TgcMesh>();
            list = this.GiveMeAMesh("MeshCreator\\Meshes\\Cocina\\Latas\\Pepsi\\Pepsi-TgcScene.xml");
            this.cocina.AddElement(new SceneElement(list, GlobalConcepts.GenerateTransformation(new TGCVector3(0.25f, 0.25f, 0.25f), new TGCVector3(0, 0, 0), new TGCVector3(3f, 0f, 310f))));
            this.cocina.AddElement(new SceneElement(list, GlobalConcepts.GenerateTransformation(new TGCVector3(0.25f, 0.25f, 0.25f), new TGCVector3(0, 0, 0), new TGCVector3(109f, 0f, 324f))));
            this.cocina.AddElement(new SceneElement(list, GlobalConcepts.GenerateTransformation(new TGCVector3(0.25f, 0.25f, 0.25f), new TGCVector3(0, 0, 0), new TGCVector3(85f, 0f, 241f))));


            list = new List<TgcMesh>();
            list = this.GiveMeAMesh("MeshCreator\\Meshes\\Cocina\\Latas\\Fanta\\Fanta-TgcScene.xml");
            this.cocina.AddElement(new SceneElement(list, GlobalConcepts.GenerateTransformation(new TGCVector3(0.25f, 0.25f, 0.25f), new TGCVector3(0, 0, 0), new TGCVector3(60f, 0f, 344f))));
            this.cocina.AddElement(new SceneElement(list, GlobalConcepts.GenerateTransformation(new TGCVector3(0.25f, 0.25f, 0.25f), new TGCVector3(0, 0, 0), new TGCVector3(58f, 0f, 293f))));
            this.cocina.AddElement(new SceneElement(list, GlobalConcepts.GenerateTransformation(new TGCVector3(0.25f, 0.25f, 0.25f), new TGCVector3(0, 0, 0), new TGCVector3(114f, 0f, 282f))));

            list = new List<TgcMesh>();
            list = this.GiveMeAMesh("MeshCreator\\Meshes\\Cocina\\Latas\\CocaCola\\CocaCola-TgcScene.xml");
            this.cocina.AddElement(new SceneElement(list, GlobalConcepts.GenerateTransformation(new TGCVector3(0.25f, 0.25f, 0.25f), new TGCVector3(0, 0, 0), new TGCVector3(177f, 0f, 341f))));
            this.cocina.AddElement(new SceneElement(list, GlobalConcepts.GenerateTransformation(new TGCVector3(0.25f, 0.25f, 0.25f), new TGCVector3(0, 0, 0), new TGCVector3(-43f, 0f, 315f))));
            this.cocina.AddElement(new SceneElement(list, GlobalConcepts.GenerateTransformation(new TGCVector3(0.25f, 0.25f, 0.25f), new TGCVector3(0, 0, 0), new TGCVector3(77f, 0f, 154f))));

            list = new List<TgcMesh>();
            list = this.GiveMeAMesh("MeshCreator\\Meshes\\Cocina\\Latas\\Frijoles\\Frijoles-TgcScene.xml");
            this.cocina.AddElement(new SceneElement(list, GlobalConcepts.GenerateTransformation(new TGCVector3(0.25f, 0.25f, 0.25f), new TGCVector3(0, 0, 0), new TGCVector3(-48f, 0f, 174f))));
            this.cocina.AddElement(new SceneElement(list, GlobalConcepts.GenerateTransformation(new TGCVector3(0.25f, 0.25f, 0.25f), new TGCVector3(0, 0, 0), new TGCVector3(127f, 0f, 254f))));

            list = new List<TgcMesh>();
            list = this.GiveMeAMesh("MeshCreator\\Meshes\\Cocina\\Microondas\\Microondas-TgcScene.xml");
            this.cocina.AddElement(new SceneElement(list, GlobalConcepts.GenerateTransformation(new TGCVector3(0.5f, 0.5f, 0.5f), new TGCVector3(0, FastMath.PI_HALF + FastMath.PI, 0), new TGCVector3(10f, 63f, 344f))));



            //banio
            list = new List<TgcMesh>();
            list = this.GiveMeAMesh("MeshCreator\\Meshes\\Bathroom\\Jabon\\Jabon-TgcScene.xml");
            this.banio.AddElement(new SceneElement(list, GlobalConcepts.GenerateTransformation(new TGCVector3(0.7f, 0.7f, 0.7f), new TGCVector3(0, 0, 0), new TGCVector3(-105f, 0f, 265f))));
            list = new List<TgcMesh>();
            list = this.GiveMeAMesh("MeshCreator\\Meshes\\Bathroom\\Bathtub\\Bathtub-TgcScene.xml");
            this.banio.AddElement(new SceneElement(list, GlobalConcepts.GenerateTransformation(new TGCVector3(1.5f, 1.8f, 1.5f), new TGCVector3(0, 0, 0), new TGCVector3(-164f, 0f, 270f))));
            list = new List<TgcMesh>();
            list = this.GiveMeAMesh("MeshCreator\\Meshes\\Bathroom\\InodoroCuadrado\\InodoroCuadrado-TgcScene.xml");
            this.banio.AddElement(new SceneElement(list, GlobalConcepts.GenerateTransformation(new TGCVector3(2f, 3f, 3f), new TGCVector3(0, 0, 0), new TGCVector3(-85, 0f, 175))));
            list = new List<TgcMesh>();
            list = this.GiveMeAMesh("MeshCreator\\Meshes\\Bathroom\\Cepillo\\Cepillo-TgcScene.xml");
            this.banio.AddElement(new SceneElement(list, GlobalConcepts.GenerateTransformation(new TGCVector3(1.4f, 1.4f, 1.4f), new TGCVector3(0, 0, 0), new TGCVector3(-125, 0f, 170f))));
            list = new List<TgcMesh>();
            list = this.GiveMeAMesh("MeshCreator\\Meshes\\Bathroom\\Esponja\\Esponja-TgcScene.xml");
            this.banio.AddElement(new SceneElement(list, GlobalConcepts.GenerateTransformation(new TGCVector3(1f, 1f, 1f), new TGCVector3(0, FastMath.QUARTER_PI, 0), new TGCVector3(-165, 0f, 192))));
            list = new List<TgcMesh>();
            list = this.GiveMeAMesh("MeshCreator\\Meshes\\Habitacion\\Banqueta\\Banqueta-TgcScene.xml");
            this.banio.AddElement(new SceneElement(list, GlobalConcepts.GenerateTransformation(new TGCVector3(0.4f, 0.35f, 0.4f), new TGCVector3(0, 0, 0), new TGCVector3(-170, 0f, 217))));
            list = new List<TgcMesh>();
            list = this.GiveMeAMesh("MeshCreator\\Meshes\\Bathroom\\Espejo\\Espejo-TgcScene.xml");
            this.banio.AddElement(new SceneElement(list, GlobalConcepts.GenerateTransformation(new TGCVector3(1f, 1f, 1f), new TGCVector3(0, 0, 0), new TGCVector3(-90f, 0f, 298f))));
            list = new List<TgcMesh>();
            list = this.GiveMeAMesh("MeshCreator\\Meshes\\Bathroom\\TuboMetal\\TuboMetal-TgcScene.xml");
            this.banio.AddElement(new SceneElement(list, GlobalConcepts.GenerateTransformation(new TGCVector3(0.1f, 0.1f, 0.1f), new TGCVector3(0, -FastMath.PI_HALF, 0), new TGCVector3(-215f, 3.7f, 193f))));
            list = new List<TgcMesh>();
            list = this.GiveMeAMesh("MeshCreator\\Meshes\\Bathroom\\TuboMetal2\\TuboMetal2-TgcScene.xml");
            this.banio.AddElement(new SceneElement(list, GlobalConcepts.GenerateTransformation(new TGCVector3(0.1f, 0.1f, 0.1f), new TGCVector3(0, 0, 0), new TGCVector3(-89f, 3.2f, 207f))));
            list = new List<TgcMesh>();
            list = this.GiveMeAMesh("MeshCreator\\Meshes\\Bathroom\\TuboMetal3\\TuboMetal3-TgcScene.xml");
            this.banio.AddElement(new SceneElement(list, GlobalConcepts.GenerateTransformation(new TGCVector3(0.1f, 0.1f, 0.1f), new TGCVector3(0, 0, 0), new TGCVector3(-108f, 11f, 294f))));

            //habitacion



            list = new List<TgcMesh>();
            list = this.GiveMeAMesh("MeshCreator\\Meshes\\Habitacion\\Cama\\Cama-TgcScene.xml");
            this.habitacion.AddElement(new SceneElement(list, GlobalConcepts.GenerateTransformation(new TGCVector3(1, 1, 1), new TGCVector3(0, FastMath.PI, 0), new TGCVector3(-36f, 0, -124f))));
            list = new List<TgcMesh>();
            list = this.GiveMeAMesh("MeshCreator\\Meshes\\Habitacion\\MesaDeLuz\\MesaDeLuz-TgcScene.xml");
            this.habitacion.AddElement(new SceneElement(list, GlobalConcepts.GenerateTransformation(new TGCVector3(1, 1, 1), new TGCVector3(0, FastMath.PI, 0), new TGCVector3(22f, 0, -158f))));
            list = new List<TgcMesh>();
            list = this.GiveMeAMesh("MeshCreator\\Meshes\\Habitacion\\Placard\\Placard-TgcScene.xml");
            this.habitacion.AddElement(new SceneElement(list, GlobalConcepts.GenerateTransformation(new TGCVector3(1.2f, 1.5f, 1), new TGCVector3(0, -FastMath.PI_HALF, 0), new TGCVector3(-200f, 0, -103f))));
            list = new List<TgcMesh>();
            list = this.GiveMeAMesh("MeshCreator\\Meshes\\Habitacion\\Armario\\Armario-TgcScene.xml");
            this.habitacion.AddElement(new SceneElement(list, GlobalConcepts.GenerateTransformation(new TGCVector3(1, 1, 1), new TGCVector3(0, -FastMath.PI_HALF, 0), new TGCVector3(-30f, -0.5f, 110f))));
            list = new List<TgcMesh>();
            list = this.GiveMeAMesh("MeshCreator\\Meshes\\Habitacion\\Escritorio\\Escritorio-TgcScene.xml");
            this.habitacion.AddElement(new SceneElement(list, GlobalConcepts.GenerateTransformation(new TGCVector3(1, 1, 1), new TGCVector3(0, 0, 0), new TGCVector3(183f, 0f, -107f))));
            list = new List<TgcMesh>();
            list = this.GiveMeAMesh("MeshCreator\\Meshes\\Cocina\\Silla\\Silla-TgcScene.xml");
            this.habitacion.AddElement(new SceneElement(list, GlobalConcepts.GenerateTransformation(new TGCVector3(0.6f, 0.6f, 0.6f), new TGCVector3(0, FastMath.PI_HALF, 0), new TGCVector3(151f, -1f, -101f))));
            list = new List<TgcMesh>();
            list = this.GiveMeAMesh("MeshCreator\\Meshes\\Habitacion\\CajaZapatillas\\CajaZapatillas-TgcScene.xml");
            this.habitacion.AddElement(new SceneElement(list, GlobalConcepts.GenerateTransformation(new TGCVector3(0.5f, 0.5f, 0.5f), new TGCVector3(0, FastMath.PI_HALF, 0), new TGCVector3(100f, 0f, -147f))));
            list = new List<TgcMesh>();
            list = this.GiveMeAMesh("MeshCreator\\Meshes\\Habitacion\\Sillon\\Sillon-TgcScene.xml");
            this.habitacion.AddElement(new SceneElement(list, GlobalConcepts.GenerateTransformation(new TGCVector3(1f, 1f, 1f), new TGCVector3(0f, FastMath.PI + FastMath.PI_HALF, 0f), new TGCVector3(-180f, -0.5f, 20f))));
            list = new List<TgcMesh>();
            list = this.GiveMeAMesh("MeshCreator\\Meshes\\Habitacion\\Escoba\\Escoba-TgcScene.xml");
            this.habitacion.AddElement(new SceneElement(list, GlobalConcepts.GenerateTransformation(new TGCVector3(1.3f, 1.3f, 1.3f), new TGCVector3(FastMath.PI_HALF, 0, 0), new TGCVector3(-105f, 1f, -60f))));
            list = new List<TgcMesh>();
            list = this.GiveMeAMesh("MeshCreator\\Meshes\\Habitacion\\Rubik\\Rubik-TgcScene.xml");
            this.habitacion.AddElement(new SceneElement(list, GlobalConcepts.GenerateTransformation(new TGCVector3(0.4f, 0.4f, 0.4f), new TGCVector3(0, 0, 0), new TGCVector3(100f, 0f, -100f))));

            list = new List<TgcMesh>();
            list = this.GiveMeAMesh("MeshCreator\\Meshes\\Habitacion\\Rastis\\Rasti-1\\Rastis-1-TgcScene.xml");
            this.habitacion.AddElement(new SceneElement(list, GlobalConcepts.GenerateTransformation(new TGCVector3(0.2f, 0.2f, 0.2f), new TGCVector3(0, 0, FastMath.PI_HALF), new TGCVector3(-10f, 1f, 3f))));
            this.habitacion.AddElement(new SceneElement(list, GlobalConcepts.GenerateTransformation(new TGCVector3(0.2f, 0.2f, 0.2f), new TGCVector3(0, 1f, 0), new TGCVector3(-75f, 0f, -100f))));
            this.habitacion.AddElement(new SceneElement(list, GlobalConcepts.GenerateTransformation(new TGCVector3(0.2f, 0.2f, 0.2f), new TGCVector3(0, 0f, FastMath.PI_HALF), new TGCVector3(75f, 1f, 10f))));

            list = new List<TgcMesh>();
            list = this.GiveMeAMesh("MeshCreator\\Meshes\\Habitacion\\Rastis\\Rasti-2\\Rastis-2-TgcScene.xml");
            this.habitacion.AddElement(new SceneElement(list, GlobalConcepts.GenerateTransformation(new TGCVector3(0.2f, 0.2f, 0.2f), new TGCVector3(0, 1f, 0f), new TGCVector3(45f, 0f, 20f))));
            this.habitacion.AddElement(new SceneElement(list, GlobalConcepts.GenerateTransformation(new TGCVector3(0.2f, 0.2f, 0.2f), new TGCVector3(0, 2f, 0f), new TGCVector3(40f, 0f, -20f))));
            this.habitacion.AddElement(new SceneElement(list, GlobalConcepts.GenerateTransformation(new TGCVector3(0.2f, 0.2f, 0.2f), new TGCVector3(0, 3f, 0f), new TGCVector3(-50f, 0f, 20f))));

            list = new List<TgcMesh>();
            list = this.GiveMeAMesh("MeshCreator\\Meshes\\Habitacion\\Rastis\\Rasti-4\\Rastis-4-TgcScene.xml");
            this.habitacion.AddElement(new SceneElement(list, GlobalConcepts.GenerateTransformation(new TGCVector3(0.2f, 0.2f, 0.2f), new TGCVector3(0, 0f, 0f), new TGCVector3(22f, 0f, 0f))));
            this.habitacion.AddElement(new SceneElement(list, GlobalConcepts.GenerateTransformation(new TGCVector3(0.2f, 0.2f, 0.2f), new TGCVector3(0, 2f, 0f), new TGCVector3(-100f, 0f, 45f))));
            this.habitacion.AddElement(new SceneElement(list, GlobalConcepts.GenerateTransformation(new TGCVector3(0.2f, 0.2f, 0.2f), new TGCVector3(0, 1f, 0f), new TGCVector3(100f, 0f, 50f))));

            list = new List<TgcMesh>();
            list = this.GiveMeAMesh("MeshCreator\\Meshes\\Habitacion\\Rastis\\Rasti-6\\Rastis-6-TgcScene.xml");
            this.habitacion.AddElement(new SceneElement(list, GlobalConcepts.GenerateTransformation(new TGCVector3(0.2f, 0.2f, 0.2f), new TGCVector3(0, 1f, 0f), new TGCVector3(0f, 0f, -50f))));
            this.habitacion.AddElement(new SceneElement(list, GlobalConcepts.GenerateTransformation(new TGCVector3(0.2f, 0.2f, 0.2f), new TGCVector3(0, 1f, 0f), new TGCVector3(-120f, 0f, -10f))));
            this.habitacion.AddElement(new SceneElement(list, GlobalConcepts.GenerateTransformation(new TGCVector3(0.2f, 0.2f, 0.2f), new TGCVector3(0, 1f, 0f), new TGCVector3(-200f, 0f, -200f))));

            list = new List<TgcMesh>();
            list = this.GiveMeAMesh("MeshCreator\\Meshes\\Habitacion\\Toma\\Toma-TgcScene.xml");
            this.habitacion.AddElement(new SceneElement(list, GlobalConcepts.GenerateTransformation(new TGCVector3(0.5f, 0.5f, 0.5f), new TGCVector3(0, 0f, FastMath.PI_HALF), new TGCVector3(-110f, 15f, 144f))));
            this.habitacion.AddElement(new SceneElement(list, GlobalConcepts.GenerateTransformation(new TGCVector3(0.5f, 0.5f, 0.5f), new TGCVector3(0, 0f, FastMath.PI_HALF), new TGCVector3(60f, 15f, 144f))));

            list = new List<TgcMesh>();
            list = this.GiveMeAMesh("MeshCreator\\Meshes\\Habitacion\\Pelota\\Pelota-TgcScene.xml");
            this.habitacion.AddElement(new SceneElement(list, GlobalConcepts.GenerateTransformation(new TGCVector3(0.4f, 0.4f, 0.4f), new TGCVector3(0, 0f, 0), new TGCVector3(153f, 0f, 85f))));
            list = new List<TgcMesh>();
            list = this.GiveMeAMesh("MeshCreator\\Meshes\\Habitacion\\Notebook\\Notebook-TgcScene.xml");
            this.habitacion.AddElement(new SceneElement(list, GlobalConcepts.GenerateTransformation(new TGCVector3(0.5f, 0.5f, 0.5f), new TGCVector3(0, 0, 0), new TGCVector3(174f, 44f, -99f))));
            list = new List<TgcMesh>();
            list = this.GiveMeAMesh("MeshCreator\\Meshes\\Habitacion\\Libros\\Arquitectura\\Arquitectura-TgcScene.xml");
            this.habitacion.AddElement(new SceneElement(list, GlobalConcepts.GenerateTransformation(new TGCVector3(0.5f, 0.3f, 0.5f), new TGCVector3(0, 0, 0), new TGCVector3(178f, 44f, -139f))));
            list = new List<TgcMesh>();
            list = this.GiveMeAMesh("MeshCreator\\Meshes\\Habitacion\\Cargador\\Cargador-TgcScene.xml");
            this.habitacion.AddElement(new SceneElement(list, GlobalConcepts.GenerateTransformation(new TGCVector3(0.2f, 0.2f, 0.2f), new TGCVector3(0, 0, 0), new TGCVector3(197f, 44f, -70f))));
            list = new List<TgcMesh>();
            list = this.GiveMeAMesh("MeshCreator\\Meshes\\Habitacion\\Banqueta\\Banqueta-TgcScene.xml");
            this.habitacion.AddElement(new SceneElement(list, GlobalConcepts.GenerateTransformation(new TGCVector3(0.5f, 0.5f, 0.5f), new TGCVector3(0, 0, 0), new TGCVector3(182f, 0f, -13f))));
            list = new List<TgcMesh>();
            list = this.GiveMeAMesh("MeshCreator\\Meshes\\Habitacion\\Celular\\Celular-TgcScene.xml");
            this.habitacion.AddElement(new SceneElement(list, GlobalConcepts.GenerateTransformation(new TGCVector3(0.1f, 0.1f, 0.1f), new TGCVector3(0, 0, 0), new TGCVector3(24f, 32.2f, -154f))));
            list = new List<TgcMesh>();
            list = this.GiveMeAMesh("MeshCreator\\Meshes\\Habitacion\\CajaPizza\\CajaPizza-TgcScene.xml");
            this.habitacion.AddElement(new SceneElement(list, GlobalConcepts.GenerateTransformation(new TGCVector3(0.5f, 0.5f, 0.5f), new TGCVector3(0, 0, 0), new TGCVector3(-173f, 28f, 15f))));
            list = new List<TgcMesh>();
            list = this.GiveMeAMesh("MeshCreator\\Meshes\\Habitacion\\Lapiz\\Lapiz-TgcScene.xml");
            this.habitacion.AddElement(new SceneElement(list, GlobalConcepts.GenerateTransformation(new TGCVector3(0.3f, 0.3f, 0.3f), new TGCVector3(0, FastMath.PI_HALF, 0), new TGCVector3(208f, 45f, -130f))));


            //puertas
            list = new List<TgcMesh>();
            list = this.GiveMeAMesh("MeshCreator\\Meshes\\Otros\\Puerta\\Puerta-TgcScene.xml");
            this.habitacion.AddElement(new SceneElement(list, GlobalConcepts.GenerateTransformation(new TGCVector3(1f, 2.26f, 1.88f), new TGCVector3(0, FastMath.PI_HALF, 0), new TGCVector3(-173.6f, 0, 145))));
            this.habitacion.AddElement(new SceneElement(list, GlobalConcepts.GenerateTransformation(new TGCVector3(1f, 2.26f, 2f), new TGCVector3(0, -FastMath.PI_HALF, 0), new TGCVector3(131.5f, 0, 145.5f))));

            this.cocina.AddElement(new SceneElement(list, GlobalConcepts.GenerateTransformation(new TGCVector3(1f, 2.26f, 2f), new TGCVector3(0, -FastMath.PI_HALF, 0), new TGCVector3(131.5f, 0, 145.5f))));
            this.cocina.AddElement(new SceneElement(list, GlobalConcepts.GenerateTransformation(new TGCVector3(1f, 2.26f, 1.6f), new TGCVector3(0, 0, 0), new TGCVector3(-52.5f, 0, 260f))));


            this.banio.AddElement(new SceneElement(list, GlobalConcepts.GenerateTransformation(new TGCVector3(1f, 2.26f, 1.6f), new TGCVector3(0, 0, 0), new TGCVector3(-52.5f, 0, 260f))));
            this.banio.AddElement(new SceneElement(list, GlobalConcepts.GenerateTransformation(new TGCVector3(1f, 2.26f, 1.88f), new TGCVector3(0, FastMath.PI_HALF, 0), new TGCVector3(-173.6f, 0, 145))));

            //portales
            TgcMesh bidirectional, unidirectional;
            bidirectional = this.GimeMeASingleMesh("MeshCreator\\Meshes\\Otros\\Portal\\Bidirectional\\Portal-TgcScene.xml");
            unidirectional = this.GimeMeASingleMesh("MeshCreator\\Meshes\\Otros\\Portal\\Unidirectional\\Portal-TgcScene.xml");
            TGCVector3 scale = new TGCVector3(0.2f, 0.2f, 0.2f);
            TGCMatrix transformation, transformation2;
            Portal portal, portal2;
            TGCVector3 targetPosition;

            //portal que va abajo del escritorio y que se dirige arriba del escritorio
            transformation = GlobalConcepts.GenerateTransformation(scale, new TGCVector3(0, 0, 0), new TGCVector3(159f, 0, -162f));
            portal = new Portal(new TGCVector3(159f, 0, -162f), transformation);
            targetPosition = new TGCVector3(154,45,-160);
            this.habitacion.AddElement(new UnidirectionalPortal(portal, targetPosition, new TGCVector3(1,0,0), unidirectional));

            //portal que conecta la habitacion con la cocina (bidireccional)
            transformation = GlobalConcepts.GenerateTransformation(scale, new TGCVector3(0, 0, 0), new TGCVector3(133f, 0, 143f));
            portal = new Portal(new TGCVector3(133f, 0, 143f), transformation);
            transformation2 = GlobalConcepts.GenerateTransformation(scale, new TGCVector3(0, 0, 0), new TGCVector3(133f, 0, 149f));
            portal2 = new Portal(new TGCVector3(133f, 0, 149f), transformation2);
            this.habitacion.AddElement(new BidirectionalPortal(portal, portal2, new TGCVector3(0,0,1), bidirectional));
            this.cocina.AddElement(new BidirectionalPortal(portal2, portal, new TGCVector3(0, 0, -1), bidirectional));

            //portal que va de abajo de la mesa, hacia la arriba de la mesa (unidireccional)
            transformation = GlobalConcepts.GenerateTransformation(scale, new TGCVector3(0, 0, 0), new TGCVector3(65f, 0, 378f));
            portal = new Portal(new TGCVector3(65f, 0, 378f), transformation);
            targetPosition = new TGCVector3(52, 40, 358);
            this.cocina.AddElement(new UnidirectionalPortal(portal, targetPosition, new TGCVector3(-1, 0, 0), unidirectional));

            //portal que va de abajo del mueble de la cocina, hacia arriba del mueblePared (unidireccional)
            transformation = GlobalConcepts.GenerateTransformation(scale, new TGCVector3(0, FastMath.PI_HALF, 0), new TGCVector3(224f, 0, 375.5f));
            portal = new Portal(new TGCVector3(224f, 0, 375.5f), transformation);
            targetPosition = new TGCVector3(215, 145, 363);
            this.cocina.AddElement(new UnidirectionalPortal(portal, targetPosition, new TGCVector3(0, 0, -1), unidirectional));

            //portal que va de abajo del mueble de la cocina, hacia arriba del mueblecomun (unidireccional)
            transformation = GlobalConcepts.GenerateTransformation(scale, new TGCVector3(0, 0, 0), new TGCVector3(197f, 0, 312f));
            portal = new Portal(new TGCVector3(197f, 0, 312f), transformation);
            targetPosition = new TGCVector3(210, 65, 365);
            this.cocina.AddElement(new UnidirectionalPortal(portal, targetPosition, new TGCVector3(0, 0, -1), unidirectional));

            //portal que conecta el baño con la cocina (bidireccional)
            transformation = GlobalConcepts.GenerateTransformation(scale, new TGCVector3(0, FastMath.PI_HALF, 0), new TGCVector3(-48f, 0, 258f));
            portal = new Portal(new TGCVector3(-48f, 0, 258f), transformation);
            transformation2 = GlobalConcepts.GenerateTransformation(scale, new TGCVector3(0, FastMath.PI_HALF, 0), new TGCVector3(-55f, 0, 258f));
            portal2 = new Portal(new TGCVector3(-55f, 0, 258f), transformation2);
            this.cocina.AddElement(new BidirectionalPortal(portal, portal2, new TGCVector3(-1,0,0), bidirectional));
            this.banio.AddElement(new BidirectionalPortal(portal2, portal, new TGCVector3(1, 0, 0), bidirectional));


            //portal que conecta el baño con la habitacion
            transformation = GlobalConcepts.GenerateTransformation(scale, new TGCVector3(0, 0, 0), new TGCVector3(-170f, 0, 148f));
            portal = new Portal(new TGCVector3(-170f, 0, 148f), transformation);
            transformation2 = GlobalConcepts.GenerateTransformation(scale, new TGCVector3(0, 0, 0), new TGCVector3(-170f, 0, 142f));
            portal2 = new Portal(new TGCVector3(-170f, 0, 142f), transformation2);
            this.banio.AddElement(new BidirectionalPortal(portal, portal2, new TGCVector3(0, 0, -1), bidirectional));
            this.habitacion.AddElement(new BidirectionalPortal(portal2, portal, new TGCVector3(0, 0, 1), bidirectional));

            //portal que va desde abajo del placard hacia arriba del placard
            transformation = GlobalConcepts.GenerateTransformation(scale, new TGCVector3(0, 0, 0), new TGCVector3(-202f, 0, -159f));
            portal = new Portal(new TGCVector3(-202f, 0, -166f), transformation);
            targetPosition = new TGCVector3(-210, 100, -150);
            this.habitacion.AddElement(new UnidirectionalPortal(portal, targetPosition, new TGCVector3(0, 0, 1), unidirectional));

            //portal que va desde abajo de la cama hacia arriba de la cama
            transformation = GlobalConcepts.GenerateTransformation(scale, new TGCVector3(0, 0, 0), new TGCVector3(-35f, 0, -173f));
            portal = new Portal(new TGCVector3(-35f, 0, -173f), transformation);
            targetPosition = new TGCVector3(32, 40, -160);
            this.habitacion.AddElement(new UnidirectionalPortal(portal, targetPosition, new TGCVector3(-1, 0, 0), unidirectional));
            
        }

        private List<TgcMesh> GiveMeAMesh(string ruta)
        {
            TgcScene tgcScene = new TgcSceneLoader().loadSceneFromFile(GlobalConcepts.GetInstance().GetMediaDir() + ruta);
            return tgcScene.Meshes;
            tgcScene.Meshes.ForEach((mesh) => {
                var adj = new int[mesh.D3dMesh.NumberFaces * 3];
                mesh.D3dMesh.GenerateAdjacency(0, adj);
                mesh.D3dMesh.ComputeNormals(adj);
            });

        }
        private TgcMesh GimeMeASingleMesh(string ruta)
        {
            TgcScene tgcScene = new TgcSceneLoader().loadSceneFromFile(GlobalConcepts.GetInstance().GetMediaDir() + ruta);
            //tgcScene.Meshes[0].D3dMesh.ComputeNormals();
            return tgcScene.Meshes[0];

        }


        public void Render()
        {
            this.VehicleUbication(this.auto).Render();
        }

        public void RenderRoom()
        {
            this.habitacion.Render();
        }

        public void Dispose()
        {
            this.habitacion.Dispose();
            this.cocina.Dispose();
            this.banio.Dispose();

        }

    }
}
