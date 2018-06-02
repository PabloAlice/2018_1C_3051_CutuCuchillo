using System;
using System.Collections.Generic;
using System.Linq;
using TGC.Core.SceneLoader;
using TGC.Core.Mathematica;
using TGC.Core.Collision;
using TGC.Core.BoundingVolumes;

namespace TGC.Group.Model
{
    abstract class Weapon : Collidable, IShootable
    {
        // Scene object related methods and attributes
        private static MeshObb billet;

        protected TGCVector3 position;
        protected bool inScene;

        protected TGCMatrix rotation, scalation;
        protected String billetPath, weaponPath;
        
        public Weapon()
        {
            this.rotation = TGCMatrix.RotationYawPitchRoll(0f, 0f, 0f);
        }

        public TgcMesh GetCollidable(Vehicle car)
        {
            return billet.GetMesh();
        }
      
        public TgcBoundingAxisAlignBox GetBoundingAlignBox()
        {
            return billet.GetMesh().BoundingBox;
        }

        public void Render()
        {
            if (inScene)
            {
                UpdateRotation();

                TGCMatrix translation = TGCMatrix.Translation(position);
                billet.Transform(TGCMatrix.Scaling(0.04f, 0.04f, 0.04f) * TGCMatrix.RotationYawPitchRoll(0f, -FastMath.PI_HALF, 0f) * rotation * translation * TGCMatrix.Translation(0f, 0.7f, 0f));

                MeshObb weapon = getMeshOBB();
                weapon.Transform(scalation * rotation * translation * GetHeight());

                billet.Render();
                weapon.Render();
            }
        }

        protected abstract TGCMatrix GetHeight();

        protected void InitializeMeshes()
        {
            setMeshOBB(new MeshObb(new TgcSceneLoader().loadSceneFromFile(weaponPath).Meshes[0]));
            billet = new MeshObb(new TgcSceneLoader().loadSceneFromFile(billetPath).Meshes[0]);
        }

        private void UpdateRotation()
        {
            this.rotation = rotation * TGCMatrix.RotationYawPitchRoll(1f * GlobalConcepts.GetInstance().GetElapsedTime(), 0f, 0f);
        }

        public void HandleCollisions(Vehicle car)
        {
            if (TgcCollisionUtils.testObbObb(car.GetTGCBoundingOrientedBox(), billet.GetObb()) && inScene)
            {
                // Agregar logica para chequear si debe agregar el elemento (si le faltan municiones, si tiene ya este tipo de arma....)
                car.setWeapon(this);
                this.inScene = false;
            }
        }

        public void enableInScene()
        {
            this.inScene = true;
        }
        
        public void setPosition(TGCVector3 position)
        {
            // Osea que si le das una posicion, lo va a poner en la escena, sino (como el caso de la DefaultWeapon) no lo pone un carajo
            this.position = position;
            this.inScene = true;
        }





        // Projectile related methods and attributes

        protected List<Projectile> projectiles = new List<Projectile>();

        public void addProjectile(Projectile p)
        {
            this.projectiles.Add(p);
        }

        private bool colisionan(Projectile p, Collidable c)
        {
            this.getMeshOBB().Transform(getShotMeshTransformation(p));
            return TgcCollisionUtils.testAABBAABB(this.getMeshOBB().GetBoundingAlignBox(), c.GetBoundingAlignBox());
        }

        public void updateProjectiles()
        {
            projectiles.RemoveAll(p => p.getTimeSinceShot() > 1 || Scene.GetInstance().GetPosiblesCollidables().Exists(c => colisionan(p, c)));
        }

        public void renderProjectiles()
        {
            foreach(Projectile p in projectiles)
            {
                p.updateTimeSinceShot(GlobalConcepts.GetInstance().GetElapsedTime());
                this.getMeshOBB().Transform(getShotMeshTransformation(p));
                this.getMeshOBB().Render();
            }
        }

        public abstract MeshObb getMeshOBB();
        public abstract void setMeshOBB(MeshObb mesh);

        public bool HasRemainingProjectiles()
        {
            if(projectiles.Count() > 0)
            {
                return true;
            }
            return false;
        }

        public abstract TGCMatrix getShotMeshTransformation(Projectile p);

        public virtual void Dispose()
        {
            billet.Dispose();
            this.getMeshOBB().Dispose();
        }

    }
}
