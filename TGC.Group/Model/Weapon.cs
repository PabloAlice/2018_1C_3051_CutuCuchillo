using TGC.Core.SceneLoader;
using TGC.Core.Mathematica;
using TGC.Core.BoundingVolumes;
using TGC.Core.Collision;

namespace TGC.Group.Model
{
    abstract class Weapon : Collidable
    {
        public TgcMesh mesh;
        public TgcBoundingSphere sphere;
        public WeaponState weaponState;
        public TransformationMatrix initialTransformation = new TransformationMatrix();
        public TransformationMatrix matrix = new TransformationMatrix();

        public Weapon(TransformationMatrix matrix, TgcMesh mesh)
        {
            this.initialTransformation = matrix;
            this.matrix = this.initialTransformation;
            this.mesh = mesh;
            this.mesh.AutoTransform = false;
            TGCMatrix m = this.matrix.GetTransformation();
            this.mesh.Transform = m;
            this.mesh.BoundingBox.transform(m);
            this.sphere = TgcBoundingSphere.computeFromPoints(this.mesh.BoundingBox.computeCorners()).toClass();
            this.weaponState = new InExhibition(this);
            
        }

        abstract public TGCVector3 GetShootRotation();

        public bool IsColliding(Weapon weapon)
        {
            return false;
        }

        public TGCVector3 GetPosition()
        {
            return TGCVector3.transform(new TGCVector3(0,0,0), this.matrix.GetTransformation());
        }

        public TgcMesh GetCollidable(Vehicle car)
        {
            return this.weaponState.GetCollidable(car);
        }

        public void Transform()
        {
            TGCMatrix m = this.matrix.GetTransformation();
            this.mesh.Transform = m;
            this.sphere.setCenter(TGCVector3.transform(new TGCVector3(0,0,0), m));

        }

        public void HandleCollisions(Vehicle car)
        {
            this.weaponState.HandleCollision(car);
        }

        public void Shoot(Vehicle car)
        {
            this.weaponState.Shoot(car);
        }

        public void Render()
        {
            this.weaponState.Render();
        }

        public void Dispose()
        {
            this.weaponState.Dispose();
        }

        abstract public void Move();

        //abstract public void Shoot();
    }
}
