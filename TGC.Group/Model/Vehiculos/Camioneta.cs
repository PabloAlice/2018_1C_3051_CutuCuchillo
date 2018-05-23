using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.SceneLoader;
using TGC.Core.Mathematica;

namespace TGC.Group.Model.Vehiculos
{
    class Camioneta : Vehiculo
    {
        public Camioneta(CamaraEnTerceraPersona camara, TGCVector3 posicionInicial, SoundsManager soundsManager) : base(camara, posicionInicial, soundsManager)
        {
            //creo los meshes de las ruedas y luego agrego cada object Rueda(mesh,posicion) a la lista de ruedas;
            TgcSceneLoader loader = new TgcSceneLoader();
            TgcMesh ruedaIzquierda = loader.loadSceneFromFile(ConceptosGlobales.GetInstance().GetMediaDir() + "meshCreator\\meshes\\Vehiculos\\Camioneta\\Rueda\\Rueda-TgcScene.xml").Meshes[0];
            TgcMesh ruedaDerecha = ruedaIzquierda.clone("ruedaDerecha");
            TgcMesh ruedaTraseraIzquierda = ruedaIzquierda.clone("ruedaTraseraIzquierda");
            TgcMesh ruedaTraseraDerecha = ruedaIzquierda.clone("ruedaTraseraDerecha");
            delanteraIzquierda = new Rueda(ruedaIzquierda, new TGCVector3(32f, 18f, -63f));
            delanteraDerecha = new Rueda(ruedaDerecha, new TGCVector3(-32f, 18f, -63f));

            ruedas.Add(new Rueda(ruedaTraseraIzquierda, new TGCVector3(32f, 18f, 55f)));
            ruedas.Add(new Rueda(ruedaTraseraDerecha, new TGCVector3(-32f, 18f, 55f)));

        }
    }
}
