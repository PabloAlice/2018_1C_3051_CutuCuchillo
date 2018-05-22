using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;

namespace TGC.Group.Model
{
    class Portals
    {
        private static Portals instance;
        private List<Portal> portals = new List<Portal>();
        private TgcMesh portal;

        private Portals()
        {
            TgcScene scene = new TgcSceneLoader().loadSceneFromFile(ConceptosGlobales.getInstance().GetMediaDir() + "MeshCreator\\Meshes\\Otros\\Portal\\Portal-TgcScene.xml");
            this.portal = scene.Meshes[0];
            InitPortals();
        }

        
        public static Portals GetInstance()
        {
            if (instance == null)
            {
                instance = new Portals();
            }

            return instance;
        }


        /*
        public void checkCollision(Vehiculo car)
        {
            foreach(TgcMesh mesh in meshes)
            {
                mesh.Transform = transformIN;
                if(car.mesh.BoundingBox.)

                mesh.Transform = transformOUT;
            }
        }*/

        private void Move(TGCMatrix transformation)
        {
                portal.AutoTransform = false;
                portal.Transform = transformation;
                portal.BoundingBox.transform(transformation);
        }

        private void InitPortals()
        {
            TGCVector3 scale = new TGCVector3(0.2f, 0.2f, 0.2f);
            float altura = 0f;

            portals.Add(new Portal(
                scale, new TGCVector3(0,0,0), new TGCVector3(159f,altura,-162f), 
                new TGCVector3(0f,0f,0f),
                FastMath.PI));

            portals.Add(new Portal(
                scale, new TGCVector3(0, 0, 0), new TGCVector3(133f, altura, 143f),
                new TGCVector3(133f, 0f, 150f),
                0f));

            portals.Add(new Portal(
                scale, new TGCVector3(0, 0, 0), new TGCVector3(136f, altura, 148f),
                new TGCVector3(133f, 0f, 140f),
                0));

            portals.Add(new Portal(
                scale, new TGCVector3(0, 0, 0), new TGCVector3(65f, altura, 378f),
                new TGCVector3(139f, 50f, 348f),
                0));

            portals.Add(new Portal(
                scale, new TGCVector3(0, 0, 0), new TGCVector3(190f, altura, 373f),
                new TGCVector3(190f, 60f, 372f),
                0));

            portals.Add(new Portal(
                scale, new TGCVector3(0, FastMath.PI_HALF, 0), new TGCVector3(-49f, altura, 260f),
                new TGCVector3(-60f, 0f, 291f),
                0));

            portals.Add(new Portal(
                scale, new TGCVector3(0, 0, 0), new TGCVector3(-143f, altura, 148f),
                new TGCVector3(-143f, 0f, 139f),
                0));

            portals.Add(new Portal(
                scale, new TGCVector3(0, 0, 0), new TGCVector3(-142f, altura, 142f),
                new TGCVector3(-142f, 0f, 151f),
                0));

            portals.Add(new Portal(
                scale, new TGCVector3(0, 0, 0), new TGCVector3(-202f, altura, -159f),
                new TGCVector3(-202f, 70f, -157f),
                0));

            portals.Add(new Portal(
                scale, new TGCVector3(0, 0, 0), new TGCVector3(-35f, altura, -173.7f),
                new TGCVector3(-202f, 70f, -157f),
                0));
        }

        private void CheckPortals(Vehiculo car)
        {
            foreach(Portal portal in portals)
            {
                Move(portal.GetTransformation());
                if (!AreDistantPositions(this.portal.Position, car.GetPosicion()))
                {
                    if (Collide(car))
                    {
                        car.Move(portal.getTargetPosition());
                        car.Rotate(portal.getTargetRotation());
                    }
                }

            }
        }

        private Boolean Collide(Vehiculo car)
        {
            return false;
        }

        private Boolean AreDistantPositions(TGCVector3 posA, TGCVector3 posB)
        {
            float distX = Math.Abs(posA.X - posB.X);
            float distZ = Math.Abs(posA.Z - posB.Z);

            return distX > 10f || distZ > 10f ? true : false;
        }

        public void Render()
        {
            foreach(Portal portal in this.portals)
            {
                this.RenderPortal(portal);
            }
        }

        private void RenderPortal(Portal portal)
        {
                this.portal.AutoTransform = false;
                this.portal.Transform = portal.GetTransformation();
                portal.Rotate(TGCMatrix.RotationZ(0.05f));
                this.portal.Render();
        }

        public void Dispose()
        {
            this.portal.Dispose();
        }
    }
}
