using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.SceneLoader;
using TGC.Core.Mathematica;
using TGC.Core.Collision;
using TGC.Core.BoundingVolumes;

namespace TGC.Group.Model
{
    abstract class Weapon : Collidable
    {
        private MeshObb billet;
        private MeshObb weapon;
        protected TGCMatrix translation, rotation, scalation;
        protected String billetPath, weaponPath;
        bool isVisible;

        public Weapon(TGCMatrix translate)
        {
            this.rotation = TGCMatrix.RotationYawPitchRoll(0f, 0f, 0f);
            this.translation = translate;
            isVisible = true;
        }

        public TgcBoundingAxisAlignBox GetBoundingAlignBox()
        {
            return this.billet.getMesh().BoundingBox;
        }

        public void Render()
        {
            if (!isVisible)
            {
                return;
            }

            Update();
            
            billet.Transform(TGCMatrix.Scaling(0.04f, 0.04f, 0.04f) * TGCMatrix.RotationYawPitchRoll(0f, -FastMath.PI_HALF, 0f) * rotation * translation * TGCMatrix.Translation(0f, 0.7f, 0f));
            weapon.Transform(scalation * rotation * translation * GetHeight());

            billet.Render();
            weapon.Render();
        }

        public TGCVector3 GetPosition()
        {
            return TGCVector3.transform(new TGCVector3(0,0,0), translation);
        }

        protected abstract TGCMatrix GetHeight();

        protected virtual void InitializeMeshes()
        {
            weapon = new MeshObb(new TgcSceneLoader().loadSceneFromFile(weaponPath).Meshes[0]);
            billet = new MeshObb(new TgcSceneLoader().loadSceneFromFile(billetPath).Meshes[0]);
        }

        private void Update()
        {
            this.rotation = rotation * TGCMatrix.RotationYawPitchRoll(1f * ConceptosGlobales.GetInstance().GetElapsedTime(), 0f, 0f);
        }

        public void HandleCollisions(Vehiculo car)
        {
            if (TgcCollisionUtils.testObbObb(car.GetTGCBoundingOrientedBox(), billet.getObb().GetBoundingOrientedBox()) && isVisible)
            {
                car.addWeapon(this);
                this.Collide(car);
                this.isVisible = false;
            }
        }

        
        protected abstract void Collide(Vehiculo car);

        public void Dispose()
        {
            billet.Dispose();
            weapon.Dispose();
        }
    }
}
