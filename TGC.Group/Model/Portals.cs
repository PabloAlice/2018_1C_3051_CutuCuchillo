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
        private TgcScene portal;
        String mediaDir = ConceptosGlobales.getInstance().GetMediaDir();

        private Portals()
        {
            this.portal = new TgcSceneLoader().loadSceneFromFile(mediaDir + "MeshCreator\\Meshes\\Otros\\Portal\\Portal-TgcScene.xml");
            initPortals();
        }

        
        public static Portals getInstance()
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

        private void move(TGCMatrix transformation)
        {
            foreach (TgcMesh mesh in this.portal.Meshes)
            {
                mesh.AutoTransform = false;
                mesh.Transform = transformation;
                mesh.BoundingBox.transform(transformation);
            }
        }

        private void initPortals()
        {
            portals.Add(new Portal(
                ConceptosGlobales.getInstance().GenerateTransformation(new TGCVector3(0.3f,0.3f,0.3f), new TGCVector3(0,0,0), new TGCVector3(159f,0f,-162f)), 
                new TGCVector3(0f,0f,0f),
                FastMath.PI));

            portals.Add(new Portal(
                ConceptosGlobales.getInstance().GenerateTransformation(new TGCVector3(0.3f, 0.3f, 0.3f), new TGCVector3(0, 0, 0), new TGCVector3(133f, 0f, 143f)),
                new TGCVector3(133f, 0f, 150f),
                0f));

            portals.Add(new Portal(
                ConceptosGlobales.getInstance().GenerateTransformation(new TGCVector3(0.3f, 0.3f, 0.3f), new TGCVector3(0, 0, 0), new TGCVector3(136f, 0f, 148f)),
                new TGCVector3(133f, 0f, 140f),
                0));

            portals.Add(new Portal(
                ConceptosGlobales.getInstance().GenerateTransformation(new TGCVector3(0.3f, 0.3f, 0.3f), new TGCVector3(0, 0, 0), new TGCVector3(65f, 0f, 378f)),
                new TGCVector3(139f, 50f, 348f),
                0));

            portals.Add(new Portal(
                ConceptosGlobales.getInstance().GenerateTransformation(new TGCVector3(0.3f, 0.3f, 0.3f), new TGCVector3(0, 0, 0), new TGCVector3(190f, 0f, 373f)),
                new TGCVector3(190f, 60f, 372f),
                0));

            portals.Add(new Portal(
                ConceptosGlobales.getInstance().GenerateTransformation(new TGCVector3(0.3f, 0.3f, 0.3f), new TGCVector3(0, FastMath.PI_HALF, 0), new TGCVector3(-49f, 0f, 260f)),
                new TGCVector3(-60f, 0f, 291f),
                0));

            portals.Add(new Portal(
                ConceptosGlobales.getInstance().GenerateTransformation(new TGCVector3(0.3f, 0.3f, 0.3f), new TGCVector3(0, 0, 0), new TGCVector3(-143f, 0f, 148f)),
                new TGCVector3(-143f, 0f, 139f),
                0));

            portals.Add(new Portal(
                ConceptosGlobales.getInstance().GenerateTransformation(new TGCVector3(0.3f, 0.3f, 0.3f), new TGCVector3(0, 0, 0), new TGCVector3(-142f, 0f, 142f)),
                new TGCVector3(-142f, 0f, 151f),
                0));

            portals.Add(new Portal(
                ConceptosGlobales.getInstance().GenerateTransformation(new TGCVector3(0.3f, 0.3f, 0.3f), new TGCVector3(0, 0, 0), new TGCVector3(-202f, 0f, -159f)),
                new TGCVector3(-202f, 70f, -157f),
                0));

            portals.Add(new Portal(
                ConceptosGlobales.getInstance().GenerateTransformation(new TGCVector3(0.3f, 0.3f, 0.3f), new TGCVector3(0, 0, 0), new TGCVector3(-35f, 0f, -173.7f)),
                new TGCVector3(-202f, 70f, -157f),
                0));
        }

        private void checkPortals(Vehiculo car)
        {
            foreach(Portal portal in portals)
            {
                move(portal.getPosition());
                if (!areDistantPositions(this.portal.Meshes[0].Position, car.GetPosicion()))
                {
                    if (collide(car))
                    {
                        car.Move(portal.getTargetPosition());
                        car.Rotate(portal.getTargetRotation());
                    }
                }

            }
        }

        private Boolean collide(Vehiculo car)
        {
            return false;
        }

        private Boolean areDistantPositions(TGCVector3 posA, TGCVector3 posB)
        {
            float distX = Math.Abs(posA.X - posB.X);
            float distZ = Math.Abs(posA.Z - posB.Z);

            return distX > 10f || distZ > 10f ? true : false;
        }

        public void render()
        {
            foreach(Portal portal in portals)
            {
                renderPortal(portal);
            }
        }

        private void renderPortal(Portal portal)
        {
            foreach (TgcMesh mesh in this.portal.Meshes)
            {
                mesh.AutoTransform = false;
                mesh.Transform = portal.getPosition();
                mesh.Render();
            }
        }

        public void dispose()
        {
            this.portal.DisposeAll();
        }
    }
}
