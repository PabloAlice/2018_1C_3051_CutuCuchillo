﻿using TGC.Core.SceneLoader;
using TGC.Core.Mathematica;

namespace TGC.Group.Model
{
    class Misile : Weapon
    {
        public Misile(TransformationMatrix matrix, TgcMesh mesh) : base(matrix, mesh)
        {
        }

        public override void Collide(Collidable collided)
        {
            this.weaponState = new InExhibition(this);
            return;
        }

        override public void Move()
        {
            //proximamente
        }

        public override TGCVector3 GetShootRotation()
        {
            float angle = GlobalConcepts.GetInstance().AngleBetweenVectors(new TGCVector3(0,0,1), this.direction);
            return new TGCVector3(FastMath.PI_HALF, angle, 0);
        }
    }
}
