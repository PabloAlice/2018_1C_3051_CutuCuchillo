using TGC.Core.Collision;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;

namespace TGC.Group.Model
{
    class Bomb : Weapon
    {

        public Bomb(TransformationMatrix matrix, TgcMesh mesh) : base(matrix, mesh)
        {
        }

        override public void Move()
        {
            //proximamente
        }

        public override TGCVector3 GetShootRotation()
        {
            return new TGCVector3(0,0,0);
        }
    }
}
