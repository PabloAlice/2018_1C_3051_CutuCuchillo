using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.SceneLoader;
using TGC.Core.Mathematica;

namespace TGC.Group.Model
{
    class Weapons
    {
        private TgcMesh billet;
        private TgcMesh weapon;
        private TGCMatrix traslation, rotation, scalation;

        public Weapons(string billetPath, string weaponPath)
        {
            this.billet = this.CreateMesh(billetPath);
            this.weapon = this.CreateMesh(weaponPath);
        }

        public void SetTransformation(TGCMatrix scale, TGCMatrix rotate, TGCMatrix translate)
        {
            this.traslation = translate;
            this.rotation = rotate;
            this.scalation = scale;
        }

        private TgcMesh CreateMesh(string path)
        {
            TgcSceneLoader loader = new TgcSceneLoader();
            TgcScene scene = loader.loadSceneFromFile(path);
            TgcMesh mesh = scene.Meshes[0];
            mesh.AutoTransform = false;
            this.rotation = TGCMatrix.RotationYawPitchRoll(0, 0, 0);
            this.scalation = TGCMatrix.Scaling(new TGCVector3(1,1,1));
            this.traslation = TGCMatrix.Translation(new TGCVector3(0,0,0));
            return mesh;
        }
    }
}
