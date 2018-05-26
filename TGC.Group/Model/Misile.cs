using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Mathematica;

namespace TGC.Group.Model
{
    class Misile : Weapon
    {
        public Misile() : base()
        {

            this.scalation = TGCMatrix.Scaling(0.04f, 0.04f, 0.04f);
            this.weaponPath = ConceptosGlobales.GetInstance().GetMediaDir() + "MeshCreator\\Meshes\\Otros\\Weapons\\Misil\\Misil-TgcScene.xml";
            this.billetPath = ConceptosGlobales.GetInstance().GetMediaDir() + "MeshCreator\\Meshes\\Habitacion\\Billetes\\BilleteAmiguero\\BilleteAmiguero-TgcScene.xml";

            InitializeMeshes();
        }

        public override void Dispose()
        {
            base.Dispose();
        }

        public override MeshObb getMeshOBB()
        {
            throw new NotImplementedException();
        }

        public override TGCMatrix getShotMeshPosition(Projectile p)
        {
            throw new NotImplementedException();
        }

        public override void setMeshOBB(MeshObb mesh)
        {
            throw new NotImplementedException();
        }

        protected override TGCMatrix GetHeight()
        {
            return TGCMatrix.Translation(0f, 2.2f, 0f);
        }

    }
}
