using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.SceneLoader;
using TGC.Core.Mathematica;

namespace TGC.Group.Model
{
    abstract class Weapon : Collidable
    {
        private TgcMesh billet;
        private TgcMesh weapon;
        protected TGCMatrix translation, rotation, scalation;
        protected String billetPath, weaponPath; 

        public Weapon(TGCMatrix translate)
        {
            this.rotation = TGCMatrix.RotationYawPitchRoll(0f, 0f, 0f);
            this.translation = translate;
        }

        public void Render()
        {
            Update();
            
            billet.Transform = TGCMatrix.Scaling(0.04f, 0.04f, 0.04f) * TGCMatrix.RotationYawPitchRoll(0f, -FastMath.PI_HALF, 0f) * rotation * translation * TGCMatrix.Translation(0f, 0.7f, 0f);
            weapon.Transform = scalation * rotation * translation * getHeight();

            billet.Render();
            weapon.Render();

        }

        protected abstract TGCMatrix getHeight();

        protected virtual void InitializeMeshes()
        {
            weapon = new TgcSceneLoader().loadSceneFromFile(weaponPath).Meshes[0];
            weapon.AutoTransform = false;
            billet = new TgcSceneLoader().loadSceneFromFile(billetPath).Meshes[0];
            billet.AutoTransform = false;
        }

        private void Update()
        {
            this.rotation = rotation * TGCMatrix.RotationYawPitchRoll(1f * ConceptosGlobales.GetInstance().GetElapsedTime(), 0f, 0f);
        }

        public void HandleCollisions(Vehiculo car)
        {
            return;
        }

        public void Dispose()
        {
            billet.Dispose();
            weapon.Dispose();
        }
    }
}
