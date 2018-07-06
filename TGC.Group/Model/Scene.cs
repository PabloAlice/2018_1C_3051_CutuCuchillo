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
            this.auto.HandleCollision(this.AI);
            this.VehicleUbication(this.auto).HandleCollisions(this.auto);
            this.VehicleUbication(this.AI).HandleCollisions(this.AI);
            
        }

        public void HandleCollisions(ThirdPersonCamera camera)
        {
            List<Collidable> posibleCollidables = CameraUbication(camera).GetPosiblesCollidables(camera.GetPosition());
            foreach (Collidable element in posibleCollidables)
            {
                element.HandleCollision(camera);
            }
        }

        private bool IsBetween(TGCVector3 interes, TGCVector3 pmin, TGCVector3 pmax)
        {
            return interes.X > pmin.X && interes.X < pmax.X && interes.Z > pmin.Z && interes.Z < pmax.Z;
        }

        private bool IsIn(Section seccion, TGCVector3 position)
        {
            return IsBetween(position, seccion.GetPuntoMinimo(), seccion.GetPuntoMaximo());
            
        }

        public List<Weapon> GetWeapons(Vehicle car)
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
            return this.NearestSection(position);
            
        }

        private Section CameraUbication(ThirdPersonCamera camera)
        {
            TGCVector3 position = camera.GetPosition();
            if (this.IsIn(this.cocina, position))
            {
                return this.cocina;
            }
            else if (this.IsIn(this.habitacion, position))
            {
                return this.habitacion;
            }
            else if (this.IsIn(this.banio, position))
            {
                return this.banio;
            }
            return this.NearestSection(position);

        }

        private Section NearestSection(TGCVector3 position)
        {
            List<Section> sections = new List<Section>();
            sections.Add(this.habitacion);
            sections.Add(this.cocina);
            sections.Add(this.banio);
            sections.Sort((x1,x2) => this.distance(x1.Center(), position).CompareTo(this.distance(x2.Center(), position)));
            return sections[0];
        }

        private float distance(TGCVector3 p1, TGCVector3 p2)
        {
            TGCVector3 vector = p2 - p1;
            return TGCVector3.Length(vector);
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

        public List<Collidable> GetPlanes()
        {
            return this.VehicleUbication(this.auto).GetPlanes();
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

            weapon = this.GimeMeASingleMesh("MeshCreator\\Meshes\\Otros\\Weapons\\Misil\\Misil-TgcScene.xml");
            initMatrix = new TransformationMatrix();
            initMatrix.SetScalation(TGCMatrix.Scaling(0.02f, 0.02f, 0.02f));
            initMatrix.SetRotation(TGCMatrix.RotationYawPitchRoll(0, 0, 0));
            initMatrix.SetTranslation(TGCMatrix.Translation(new TGCVector3(200f, 0.35f, 40)));
            this.habitacion.AddElement(new Misile(initMatrix, weapon));

            weapon = this.GimeMeASingleMesh("MeshCreator\\Meshes\\Otros\\Weapons\\BolaHielo\\BolaHielo-TgcScene.xml");
            initMatrix = new TransformationMatrix();
            initMatrix.SetScalation(TGCMatrix.Scaling(0.02f, 0.02f, 0.02f));
            initMatrix.SetRotation(TGCMatrix.RotationYawPitchRoll(0, 0, 0));
            initMatrix.SetTranslation(TGCMatrix.Translation(new TGCVector3(0f, 0.35f, -50)));
            this.habitacion.AddElement(new BolaHielo(initMatrix, weapon));

            weapon = this.GimeMeASingleMesh("MeshCreator\\Meshes\\Otros\\Weapons\\BolaHielo\\BolaHielo-TgcScene.xml");
            initMatrix = new TransformationMatrix();
            initMatrix.SetScalation(TGCMatrix.Scaling(0.02f, 0.02f, 0.02f));
            initMatrix.SetRotation(TGCMatrix.RotationYawPitchRoll(0, 0, 0));
            initMatrix.SetTranslation(TGCMatrix.Translation(new TGCVector3(40f, 0.35f, -100)));
            this.habitacion.AddElement(new BolaHielo(initMatrix, weapon));

            weapon = this.GimeMeASingleMesh("MeshCreator\\Meshes\\Otros\\Weapons\\BolaHielo\\BolaHielo-TgcScene.xml");
            initMatrix = new TransformationMatrix();
            initMatrix.SetScalation(TGCMatrix.Scaling(0.02f, 0.02f, 0.02f));
            initMatrix.SetRotation(TGCMatrix.RotationYawPitchRoll(0, 0, 0));
            initMatrix.SetTranslation(TGCMatrix.Translation(new TGCVector3(-145f, 0.35f, 90)));
            this.habitacion.AddElement(new BolaHielo(initMatrix, weapon));

            weapon = this.GimeMeASingleMesh("MeshCreator\\Meshes\\Otros\\Weapons\\Misil\\Misil-TgcScene.xml");
            initMatrix = new TransformationMatrix();
            initMatrix.SetScalation(TGCMatrix.Scaling(0.02f, 0.02f, 0.02f));
            initMatrix.SetRotation(TGCMatrix.RotationYawPitchRoll(0, 0, 0));
            initMatrix.SetTranslation(TGCMatrix.Translation(new TGCVector3(-41f, 0.35f, -120)));
            this.habitacion.AddElement(new Misile(initMatrix, weapon));

            weapon = this.GimeMeASingleMesh("MeshCreator\\Meshes\\Otros\\Weapons\\Misil\\Misil-TgcScene.xml");
            initMatrix = new TransformationMatrix();
            initMatrix.SetScalation(TGCMatrix.Scaling(0.02f, 0.02f, 0.02f));
            initMatrix.SetRotation(TGCMatrix.RotationYawPitchRoll(0, 0, 0));
            initMatrix.SetTranslation(TGCMatrix.Translation(new TGCVector3(209f, 43.35f, -161)));
            this.habitacion.AddElement(new Misile(initMatrix, weapon));

            weapon = this.GimeMeASingleMesh("MeshCreator\\Meshes\\Otros\\Weapons\\Misil\\Misil-TgcScene.xml");
            initMatrix = new TransformationMatrix();
            initMatrix.SetScalation(TGCMatrix.Scaling(0.02f, 0.02f, 0.02f));
            initMatrix.SetRotation(TGCMatrix.RotationYawPitchRoll(0, 0, 0));
            initMatrix.SetTranslation(TGCMatrix.Translation(new TGCVector3(84f, 0.35f, 185)));
            this.cocina.AddElement(new Misile(initMatrix, weapon));

            weapon = this.GimeMeASingleMesh("MeshCreator\\Meshes\\Otros\\Weapons\\Misil\\Misil-TgcScene.xml");
            initMatrix = new TransformationMatrix();
            initMatrix.SetScalation(TGCMatrix.Scaling(0.02f, 0.02f, 0.02f));
            initMatrix.SetRotation(TGCMatrix.RotationYawPitchRoll(0, 0, 0));
            initMatrix.SetTranslation(TGCMatrix.Translation(new TGCVector3(74f, 39.55f, 334)));
            this.cocina.AddElement(new Misile(initMatrix, weapon));

            weapon = this.GimeMeASingleMesh("MeshCreator\\Meshes\\Otros\\Weapons\\BolaHielo\\BolaHielo-TgcScene.xml");
            initMatrix = new TransformationMatrix();
            initMatrix.SetScalation(TGCMatrix.Scaling(0.02f, 0.02f, 0.02f));
            initMatrix.SetRotation(TGCMatrix.RotationYawPitchRoll(0, 0, 0));
            initMatrix.SetTranslation(TGCMatrix.Translation(new TGCVector3(134.5f, 39.25f, 0)));
            this.cocina.AddElement(new BolaHielo(initMatrix, weapon));

            weapon = this.GimeMeASingleMesh("MeshCreator\\Meshes\\Otros\\Weapons\\BolaHielo\\BolaHielo-TgcScene.xml");
            initMatrix = new TransformationMatrix();
            initMatrix.SetScalation(TGCMatrix.Scaling(0.02f, 0.02f, 0.02f));
            initMatrix.SetRotation(TGCMatrix.RotationYawPitchRoll(0, 0, 0));
            initMatrix.SetTranslation(TGCMatrix.Translation(new TGCVector3(200f, 142.85f, 333)));
            this.cocina.AddElement(new BolaHielo(initMatrix, weapon));

            weapon = this.GimeMeASingleMesh("MeshCreator\\Meshes\\Otros\\Weapons\\BolaHielo\\BolaHielo-TgcScene.xml");
            initMatrix = new TransformationMatrix();
            initMatrix.SetScalation(TGCMatrix.Scaling(0.02f, 0.02f, 0.02f));
            initMatrix.SetRotation(TGCMatrix.RotationYawPitchRoll(0, 0, 0));
            initMatrix.SetTranslation(TGCMatrix.Translation(new TGCVector3(-75, 0.35f, 284)));
            this.banio.AddElement(new BolaHielo(initMatrix, weapon));

            weapon = this.GimeMeASingleMesh("MeshCreator\\Meshes\\Otros\\Weapons\\Misil\\Misil-TgcScene.xml");
            initMatrix = new TransformationMatrix();
            initMatrix.SetScalation(TGCMatrix.Scaling(0.02f, 0.02f, 0.02f));
            initMatrix.SetRotation(TGCMatrix.RotationYawPitchRoll(0, 0, 0));
            initMatrix.SetTranslation(TGCMatrix.Translation(new TGCVector3(-206, 0.35f, 232f)));
            this.cocina.AddElement(new Misile(initMatrix, weapon));

            Life life;

            initMatrix = new TransformationMatrix();
            initMatrix.SetScalation(TGCMatrix.Scaling(0.025f, 0.025f, 0.025f));
            initMatrix.SetRotation(TGCMatrix.RotationYawPitchRoll(0, 0, 0));
            initMatrix.SetTranslation(TGCMatrix.Translation(new TGCVector3(10, 0.5f, 0f)));
            life = new Life(initMatrix.GetTransformation());
            this.habitacion.AddElement(life);

            TgcScene escena = new TgcSceneLoader().loadSceneFromFile(GlobalConcepts.GetInstance().GetMediaDir() + "Texturas\\Habitacion\\escenaMesheada-TgcScene.xml");
            escena.Meshes.ForEach((mesh) => {
                var adj = new int[mesh.D3dMesh.NumberFaces * 3];
                mesh.D3dMesh.GenerateAdjacency(0, adj);
                mesh.D3dMesh.ComputeNormals(adj);
            });
            //Habitacion
            //piso
            this.habitacion.AddElement(new Plane(new TGCVector3(-221,0,-174), new TGCVector3(227, 180, 145), new TGCVector3(0,1,0), "Habitacion\\Paredes\\1.jpg", 35, 35), true);
            //pared izquierda
            this.habitacion.AddElement(new Plane(new TGCVector3(-221, 0, -174), new TGCVector3(-221, 180, 145), new TGCVector3(1, 0, 0), "Habitacion\\Paredes\\2.jpg", 1, 1), true);
            //pared trasera
            this.habitacion.AddElement(new Plane(new TGCVector3(-221, 0, -174), new TGCVector3(227, 180, 0), new TGCVector3(0, 0, 1), "Habitacion\\Paredes\\3.jpg", 1, 1), true);
            //pared derecha
            this.habitacion.AddElement(new Plane(new TGCVector3(227, 0, -174), new TGCVector3(227, 180, 145), new TGCVector3(-1, 0, 0), "Habitacion\\Paredes\\2.jpg", 1, 1), true);
            //pared frontal
            this.habitacion.AddElement(new Plane(new TGCVector3(-221, 0, 145), new TGCVector3(227, 180, 145), new TGCVector3(0, 0, -1), "Habitacion\\Paredes\\2.jpg", 1, 1), true);
            //techo
            this.habitacion.AddElement(new Plane(new TGCVector3(-221, 180, -174), new TGCVector3(227, 180, 145), new TGCVector3(0, -1, 0), "Habitacion\\Paredes\\4.jpg", 1, 1), true);

            //cocina
            //piso
            this.cocina.AddElement(new Plane(new TGCVector3(-52, 0, 145), new TGCVector3(227, 180, 380), new TGCVector3(0, 1, 0), "Cocina\\Paredes\\1.jpg", 10, 10), true);
            //pared trasera
            this.cocina.AddElement(new Plane(new TGCVector3(-52, 0, 145), new TGCVector3(227, 180, 145), new TGCVector3(0, 0, 1), "Cocina\\Paredes\\2.jpg", 10, 10), true);
            //pared derecha
            this.cocina.AddElement(new Plane(new TGCVector3(227, 0, 145), new TGCVector3(227, 180, 380), new TGCVector3(-1, 0, 0), "Cocina\\Paredes\\2.jpg", 10, 10), true);
            //pared Frontal
            this.cocina.AddElement(new Plane(new TGCVector3(-52, 0, 380), new TGCVector3(227, 180, 380), new TGCVector3(0, 0, -1), "Cocina\\Paredes\\2.jpg", 10, 10), true);
            //pared izquierda
            this.cocina.AddElement(new Plane(new TGCVector3(-52, 0, 145), new TGCVector3(-52, 180, 380), new TGCVector3(1, 0, 0), "Cocina\\Paredes\\2.jpg", 10, 10), true);
            //techo
            this.cocina.AddElement(new Plane(new TGCVector3(-52, 180, 145), new TGCVector3(227, 180, 380), new TGCVector3(0, -1, 0), "Cocina\\Paredes\\2.jpg", 1, 1), true);

            //banio
            //pared trasera
            this.banio.AddElement(new Plane(new TGCVector3(-221, 0, 145), new TGCVector3(-52, 180, 145), new TGCVector3(0, 0, 1), "Bathroom\\Paredes\\1.jpg", 4.6f, 4.6f), true);
            //pared izquierda
            this.banio.AddElement(new Plane(new TGCVector3(-221, 0, 145), new TGCVector3(-221, 180, 300), new TGCVector3(1, 0, 0), "Bathroom\\Paredes\\1.jpg", 4.6f, 4.6f), true);
            //pared derecha
            this.banio.AddElement(new Plane(new TGCVector3(-52, 0, 145), new TGCVector3(-52, 180, 300), new TGCVector3(-1, 0, 0), "Bathroom\\Paredes\\1.jpg", 4.6f, 4.6f), true);
            //pared frontal
            this.banio.AddElement(new Plane(new TGCVector3(-221, 0, 300), new TGCVector3(-52, 180, 300), new TGCVector3(0, 0, -1), "Bathroom\\Paredes\\1.jpg", 4.6f, 4.6f), true);
            //piso
            this.banio.AddElement(new Plane(new TGCVector3(-221, 0, 145), new TGCVector3(-52, 180, 300), new TGCVector3(0, 1, 0), "Bathroom\\Paredes\\1.jpg", 7, 7), true);
            //techo
            this.banio.AddElement(new Plane(new TGCVector3(-52, 180, 145), new TGCVector3(-52, 180, 300), new TGCVector3(0, -1, 0), "Bathroom\\Paredes\\1.jpg", 1, 1), true);
            
            //marco derecha puerta derecha habitacion
            plane = escena.Meshes[0];
            list = new List<TgcMesh>();
            list.Add(plane);
            aux = new SceneElement(list, TGCMatrix.Scaling(new TGCVector3(1, 1, 1)) * TGCMatrix.RotationYawPitchRoll(0, 0, 0) * TGCMatrix.Translation(new TGCVector3(0, 0, 0)));
            this.habitacion.AddElement(aux, true);
            this.cocina.AddElement(aux, true);

            //marco izquierda puerta derecha habitacion
            plane = escena.Meshes[1];
            list = new List<TgcMesh>();
            list.Add(plane);
            aux = new SceneElement(list, TGCMatrix.Scaling(new TGCVector3(1, 1, 1)) * TGCMatrix.RotationYawPitchRoll(0, 0, 0) * TGCMatrix.Translation(new TGCVector3(0, 0, 0)));
            this.habitacion.AddElement(aux, true);
            this.cocina.AddElement(aux, true);
            
            //marco derecho puerta izquierda habitacion
            plane = escena.Meshes[2];
            list = new List<TgcMesh>();
            list.Add(plane);
            aux = new SceneElement(list, TGCMatrix.Scaling(new TGCVector3(1, 1, 1)) * TGCMatrix.RotationYawPitchRoll(0, 0, 0) * TGCMatrix.Translation(new TGCVector3(0, 0, 0)));
            this.habitacion.AddElement(aux, true);
            this.banio.AddElement(aux, true);

            //marco izquierda puerta izquierda habitacion
            plane = escena.Meshes[3];
            list = new List<TgcMesh>();
            list.Add(plane);
            aux = new SceneElement(list, TGCMatrix.Scaling(new TGCVector3(1, 1, 1)) * TGCMatrix.RotationYawPitchRoll(0, 0, 0) * TGCMatrix.Translation(new TGCVector3(0, 0, 0)));
            this.habitacion.AddElement(aux, true);
            this.banio.AddElement(aux, true);


            //marco derecha puerta cocina banio
            plane = escena.Meshes[4];
            list = new List<TgcMesh>();
            list.Add(plane);
            aux = new SceneElement(list, TGCMatrix.Scaling(new TGCVector3(1, 1, 1)) * TGCMatrix.RotationYawPitchRoll(0, 0, 0) * TGCMatrix.Translation(new TGCVector3(0, 0, 0)));
            this.banio.AddElement(aux, true);
            this.cocina.AddElement(aux, true);


            //marco izquierda puerta cocina banio
            plane = escena.Meshes[5];
            list = new List<TgcMesh>();
            list.Add(plane);
            aux = new SceneElement(list, TGCMatrix.Scaling(new TGCVector3(1, 1, 1)) * TGCMatrix.RotationYawPitchRoll(0, 0, 0) * TGCMatrix.Translation(new TGCVector3(0, 0, 0)));
            this.banio.AddElement(aux, true);
            this.cocina.AddElement(aux, true);

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
            this.cocina.AddElement(new SceneElement(list, GlobalConcepts.GenerateTransformation(new TGCVector3(0.25f, 0.26f, 0.25f), new TGCVector3(0, -FastMath.PI_HALF, 0), new TGCVector3(65f, 0f, 235f))));
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
            this.banio.AddElement(new SceneElement(list, GlobalConcepts.GenerateTransformation(new TGCVector3(1f, 1f, 1f), new TGCVector3(0, FastMath.PI_HALF, 0), new TGCVector3(-165, 0f, 192))));
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
            this.habitacion.AddElement(new SceneElement(list, GlobalConcepts.GenerateTransformation(new TGCVector3(0.2f, 0.2f, 0.2f), new TGCVector3(0, 0, 0), new TGCVector3(-10f, 0f, 3f))));
            this.habitacion.AddElement(new SceneElement(list, GlobalConcepts.GenerateTransformation(new TGCVector3(0.2f, 0.2f, 0.2f), new TGCVector3(0, 0f, 0), new TGCVector3(-75f, 0f, -100f))));
            this.habitacion.AddElement(new SceneElement(list, GlobalConcepts.GenerateTransformation(new TGCVector3(0.2f, 0.2f, 0.2f), new TGCVector3(0, 0f, FastMath.PI_HALF), new TGCVector3(75f, 1f, 10f))));

            list = new List<TgcMesh>();
            list = this.GiveMeAMesh("MeshCreator\\Meshes\\Habitacion\\Rastis\\Rasti-2\\Rastis-2-TgcScene.xml");
            this.habitacion.AddElement(new SceneElement(list, GlobalConcepts.GenerateTransformation(new TGCVector3(0.2f, 0.2f, 0.2f), new TGCVector3(0, 0f, 0f), new TGCVector3(45f, 0f, 20f))));
            this.habitacion.AddElement(new SceneElement(list, GlobalConcepts.GenerateTransformation(new TGCVector3(0.2f, 0.2f, 0.2f), new TGCVector3(0, 0f, 0f), new TGCVector3(40f, 0f, -20f))));
            this.habitacion.AddElement(new SceneElement(list, GlobalConcepts.GenerateTransformation(new TGCVector3(0.2f, 0.2f, 0.2f), new TGCVector3(0, 0f, 0f), new TGCVector3(-50f, 0f, 20f))));

            list = new List<TgcMesh>();
            list = this.GiveMeAMesh("MeshCreator\\Meshes\\Habitacion\\Rastis\\Rasti-4\\Rastis-4-TgcScene.xml");
            this.habitacion.AddElement(new SceneElement(list, GlobalConcepts.GenerateTransformation(new TGCVector3(0.2f, 0.2f, 0.2f), new TGCVector3(0, 0f, 0f), new TGCVector3(22f, 0f, 0f))));
            this.habitacion.AddElement(new SceneElement(list, GlobalConcepts.GenerateTransformation(new TGCVector3(0.2f, 0.2f, 0.2f), new TGCVector3(0, 0f, 0f), new TGCVector3(-100f, 0f, 45f))));
            this.habitacion.AddElement(new SceneElement(list, GlobalConcepts.GenerateTransformation(new TGCVector3(0.2f, 0.2f, 0.2f), new TGCVector3(0, 0f, 0f), new TGCVector3(100f, 0f, 50f))));

            list = new List<TgcMesh>();
            list = this.GiveMeAMesh("MeshCreator\\Meshes\\Habitacion\\Rastis\\Rasti-6\\Rastis-6-TgcScene.xml");
            this.habitacion.AddElement(new SceneElement(list, GlobalConcepts.GenerateTransformation(new TGCVector3(0.2f, 0.2f, 0.2f), new TGCVector3(0, 0f, 0f), new TGCVector3(0f, 0f, -50f))));
            this.habitacion.AddElement(new SceneElement(list, GlobalConcepts.GenerateTransformation(new TGCVector3(0.2f, 0.2f, 0.2f), new TGCVector3(0, 0f, 0f), new TGCVector3(-120f, 0f, -10f))));
            this.habitacion.AddElement(new SceneElement(list, GlobalConcepts.GenerateTransformation(new TGCVector3(0.2f, 0.2f, 0.2f), new TGCVector3(0, 0f, 0f), new TGCVector3(-200f, 0f, -200f))));

            list = new List<TgcMesh>();
            list = this.GiveMeAMesh("MeshCreator\\Meshes\\Habitacion\\Toma\\Toma-TgcScene.xml");
            this.habitacion.AddElement(new SceneElement(list, GlobalConcepts.GenerateTransformation(new TGCVector3(0.5f, 0.5f, 0.5f), new TGCVector3(0, 0f, FastMath.PI_HALF), new TGCVector3(-110f, 15f, 144f))));
            this.habitacion.AddElement(new SceneElement(list, GlobalConcepts.GenerateTransformation(new TGCVector3(0.5f, 0.5f, 0.5f), new TGCVector3(0, 0f, FastMath.PI_HALF), new TGCVector3(60f, 15f, 144f))));

            list = new List<TgcMesh>();
            list = this.GiveMeAMesh("MeshCreator\\Meshes\\Habitacion\\Pelota\\Pelota-TgcScene.xml");
            this.habitacion.AddElement(new SceneElement(list, GlobalConcepts.GenerateTransformation(new TGCVector3(0.4f, 0.4f, 0.4f), new TGCVector3(0, 0f, 0), new TGCVector3(153f, 0f, 85f))));
            list = new List<TgcMesh>();
            list = this.GiveMeAMesh("MeshCreator\\Meshes\\Habitacion\\Notebook\\Notebook-TgcScene.xml");
            this.habitacion.AddElement(new SceneElement(list, GlobalConcepts.GenerateTransformation(new TGCVector3(0.5f, 0.5f, 0.5f), new TGCVector3(0, 0, 0), new TGCVector3(174f, 42.5f, -99f))));
            list = new List<TgcMesh>();
            list = this.GiveMeAMesh("MeshCreator\\Meshes\\Habitacion\\Libros\\Arquitectura\\Arquitectura-TgcScene.xml");
            this.habitacion.AddElement(new SceneElement(list, GlobalConcepts.GenerateTransformation(new TGCVector3(0.5f, 0.3f, 0.5f), new TGCVector3(0, 0, 0), new TGCVector3(178f, 42.5f, -139f))));
            list = new List<TgcMesh>();
            list = this.GiveMeAMesh("MeshCreator\\Meshes\\Habitacion\\Cargador\\Cargador-TgcScene.xml");
            this.habitacion.AddElement(new SceneElement(list, GlobalConcepts.GenerateTransformation(new TGCVector3(0.2f, 0.2f, 0.2f), new TGCVector3(0, 0, 0), new TGCVector3(197f, 42.5f, -70f))));
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
            this.habitacion.AddElement(new SceneElement(list, GlobalConcepts.GenerateTransformation(new TGCVector3(0.3f, 0.3f, 0.3f), new TGCVector3(0, FastMath.PI_HALF, 0), new TGCVector3(208f, 43.8f, -130f))));


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
            this.cocina.AddElement(new UnidirectionalPortal(portal, targetPosition, new TGCVector3(1, 0, 0), unidirectional));

            //portal que va de abajo del mueble de la cocina, hacia arriba del mueblePared (unidireccional)
            transformation = GlobalConcepts.GenerateTransformation(scale, new TGCVector3(0, FastMath.PI_HALF, 0), new TGCVector3(224f, 0, 375.5f));
            portal = new Portal(new TGCVector3(224f, 0, 375.5f), transformation);
            targetPosition = new TGCVector3(215, 170, 363);
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
