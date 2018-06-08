using TGC.Core.SceneLoader;
using TGC.Core.Mathematica;
using TGC.Core.BoundingVolumes;

namespace TGC.Group.Model
{
    abstract class Weapon
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

        public void Transform()
        {
            TGCMatrix m = this.matrix.GetTransformation();
            this.mesh.Transform = m;
            this.sphere.setCenter(TGCVector3.transform(new TGCVector3(0,0,0), m));

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
