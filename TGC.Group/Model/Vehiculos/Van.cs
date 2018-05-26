using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.SceneLoader;
using TGC.Core.Mathematica;

namespace TGC.Group.Model.Vehiculos
{
    class Van : Vehiculo
    {
        public Van(ThirdPersonCamera camara, TGCVector3 posicionInicial, SoundsManager soundsManager) : base(camara, posicionInicial, soundsManager)
        {
            //creo los meshes de las ruedas y luego agrego cada object Rueda(mesh,posicion) a la lista de ruedas;
            TgcSceneLoader loader = new TgcSceneLoader();
            TgcMesh ruedaIzquierda = loader.loadSceneFromFile(GlobalConcepts.GetInstance().GetMediaDir() + "meshCreator\\meshes\\Vehiculos\\Camioneta\\Rueda\\Rueda-TgcScene.xml").Meshes[0];
            TgcMesh ruedaDerecha = ruedaIzquierda.clone("ruedaDerecha");
            TgcMesh ruedaTraseraIzquierda = ruedaIzquierda.clone("ruedaTraseraIzquierda");
            TgcMesh ruedaTraseraDerecha = ruedaIzquierda.clone("ruedaTraseraDerecha");
            delanteraIzquierda = new Wheel(ruedaIzquierda, new TGCVector3(35f, 15f, 63f));
            delanteraDerecha = new Wheel(ruedaDerecha, new TGCVector3(-35f, 15f, 63f));

            ruedas.Add(new Wheel(ruedaTraseraIzquierda, new TGCVector3(38f, 18f, -61f)));
            ruedas.Add(new Wheel(ruedaTraseraDerecha, new TGCVector3(-34f, 18f, -61f)));

        }
    }
}
