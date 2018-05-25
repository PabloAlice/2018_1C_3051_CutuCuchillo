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
        public Misile(TGCMatrix translate) : base(translate)
        {

            this.scalation = TGCMatrix.Scaling(0.04f, 0.04f, 0.04f);
            this.weaponPath = ConceptosGlobales.GetInstance().GetMediaDir() + "MeshCreator\\Meshes\\Otros\\Weapons\\Misil\\Misil-TgcScene.xml";
            this.billetPath = ConceptosGlobales.GetInstance().GetMediaDir() + "MeshCreator\\Meshes\\Habitacion\\Billetes\\BilleteAmiguero\\BilleteAmiguero-TgcScene.xml";

            InitializeMeshes();
        }

        protected override TGCMatrix getHeight()
        {
            return TGCMatrix.Translation(0f, 2.2f, 0f);
        }
    }
}
