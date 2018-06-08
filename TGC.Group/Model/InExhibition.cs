using TGC.Core.Mathematica;

namespace TGC.Group.Model
{
    class InExhibition : WeaponState
    {

        public InExhibition(Weapon weapon):base(weapon)
        {
        }

        override public void Move()
        {
            return;
        }

        override public void Update()
        {
            this.weapon.matrix.Rotate(TGCMatrix.RotationYawPitchRoll(0.5f, 0.3f, 0));
        }
        
    }
}
