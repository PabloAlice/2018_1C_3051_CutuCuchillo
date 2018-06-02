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

        protected List<TGCVector3> positions;
        private List<bool> isVisible;

        protected TGCMatrix rotation, scalation;
        protected String billetPath, weaponPath;
        
        public Weapon()
        {
            this.rotation = TGCMatrix.RotationYawPitchRoll(0f, 0f, 0f);
        }

        public TgcMesh GetCollidable(Vehicle car)
        {
            //return billet.GetMesh();
            return null;
        }

        public void addSceneWeapon(TGCVector3 position)
        {
            this.positions.Add(position);
            this.isVisible.Add(true);
        }
        
        public TgcBoundingAxisAlignBox GetBoundingAlignBox()
        {
            return billet.GetMesh().BoundingBox;
        }

        public void Render()
        {
            UpdateRotation();

            for(int i = 0; i < positions.Count(); i++)
            {
                if (isVisible[i])
                {
                    MeshObb weapon = getMeshOBB();

                    TGCMatrix translation = TGCMatrix.Translation(positions[i]);

                    billet.Transform(TGCMatrix.Scaling(0.04f, 0.04f, 0.04f) * TGCMatrix.RotationYawPitchRoll(0f, -FastMath.PI_HALF, 0f) * rotation * translation * TGCMatrix.Translation(0f, 0.7f, 0f));
                    weapon.Transform(scalation * rotation * translation * GetHeight());

                    billet.Render();
                    weapon.Render();
                }
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
            for (int i = 0; i < positions.Count(); i++)
            {
                if (TgcCollisionUtils.testObbObb(car.GetTGCBoundingOrientedBox(), billet.GetObb()) && isVisible[i])
                {
                    // Agregar logica para chequear si debe agregar el elemento (si le faltan municiones, si tiene ya este tipo de arma....)
                    car.addWeapon(this);
                    this.isVisible[i] = false;
                }
            }
        }
        


        // Projectile related methods and attributes

        protected List<Projectile> projectiles = new List<Projectile>();

        public void addProjectile(Projectile p)
        {
            this.projectiles.Add(p);
        }

        public void renderProjectiles()
        {
            foreach(Projectile p in projectiles)
            {
                p.updateTimeSinceShot(GlobalConcepts.GetInstance().GetElapsedTime());
                this.getMeshOBB().Transform(getShotMeshPosition(p));
                this.getMeshOBB().Render();
            }
        }

        public abstract MeshObb getMeshOBB();
        public abstract void setMeshOBB(MeshObb mesh);

        public abstract TGCMatrix getShotMeshPosition(Projectile p);

        public virtual void Dispose()
        {
            billet.Dispose();
        }

    }
}
