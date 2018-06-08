using TGC.Core.SceneLoader;
using TGC.Core.Mathematica;

namespace TGC.Group.Model.Vehiculos
{
    class Misile : Weapon
    {
        public Misile(TransformationMatrix matrix, TgcMesh mesh) : base(matrix, mesh)
        {
        }

        public override void Collide(Collidable collided)
        {
            //TODO por ahora
            return;
        }

        override public void Move()
        {
            //proximamente
        }

        public override TGCVector3 GetShootRotation()
        {
            return new TGCVector3(0, 0, 0);
        }
    }
}
