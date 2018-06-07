using TGC.Core.SceneLoader;
using TGC.Core.Mathematica;
using TGC.Core.BoundingVolumes;

namespace TGC.Group.Model
{
    abstract class Weapon
    {
        protected TgcMesh mesh;
        protected TgcBoundingSphere sphere;
        protected TransformationMatrix matrix = new TransformationMatrix();

        public Weapon(TGCMatrix InitialPosition, TGCMatrix Scaling, TgcMesh mesh)
        {
            this.matrix.Translate(InitialPosition);
            this.matrix.Scale(Scaling);
            this.matrix.Rotate(TGCMatrix.RotationYawPitchRoll(0, 0, 0));
            this.mesh = mesh;
            this.mesh.AutoTransform = false;
            TGCMatrix m = this.matrix.GetTransformation();
            this.mesh.Transform = m;
            this.mesh.BoundingBox.transform(m);
            this.sphere = TgcBoundingSphere.computeFromPoints(this.mesh.BoundingBox.computeCorners()).toClass();
        }

        protected void Transform()
        {
            TGCMatrix m = this.matrix.GetTransformation();
            this.mesh.Transform = m;
            this.sphere.setCenter(TGCVector3.transform(new TGCVector3(0,0,0), m));

        }
        abstract public void Move();
    }
}
