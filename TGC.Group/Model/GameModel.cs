using TGC.Core.Direct3D;
using TGC.Core.Example;
using TGC.Core.Mathematica;
using TGC.Group.Model.Vehiculos;
using TGC.Core.Text;
using Microsoft.DirectX.Direct3D;
using System.Drawing;
using TGC.Core.Textures;
using TGC.Core.SceneLoader;
using TGC.Core.Geometry;
using TGC.Group.Model.GameModelStates;

namespace TGC.Group.Model
{
    public class GameModel : TgcExample
    {

        private GameModelState gameModelState;

        public GameModel(string mediaDir, string shadersDir) : base(mediaDir, shadersDir)
        {
            Category = Game.Default.Category;
            Name = Game.Default.Name;
            Description = Game.Default.Description;
        }

        public override void Init()
        {
            GlobalConcepts.GetInstance().SetMediaDir(this.MediaDir);
            GlobalConcepts.GetInstance().SetShaderDir(this.ShadersDir);
            //GlobalConcepts.GetInstance().SetInput(Input);
            GlobalConcepts.GetInstance().SetDispositivoDeAudio(this.DirectSound.DsDevice);
            GlobalConcepts.GetInstance().SetScreen(D3DDevice.Instance.Device);
            GlobalConcepts.GetInstance().SetFrustum(this.Frustum);
            gameModelState = new InitialMenu(this);
            gameModelState.Init();

            //this.Camara = camaraManagement;
            
            //manager = new ObjectManagement(MediaDir + "meshCreator\\meshes\\Habitacion\\Billetes\\Billete2\\Billete2-TgcScene.xml", camaraManagement);
            
        }

        public override void Update()
        {

            this.PreUpdate();
            GlobalConcepts.GetInstance().SetElapsedTime(ElapsedTime);

            //Comentado para que los sonidos funcionen correctamente
            //this.auto = Escena.getInstance().calculateCollisions(this.auto);
            this.gameModelState.Update();
            this.PostUpdate();
        }



        public override void Render()
        {
            D3DDevice.Instance.ParticlesEnabled = true;
            D3DDevice.Instance.EnableParticles();
            TexturesManager.Instance.clearAll();
            
            D3DDevice.Instance.Device.BeginScene();
            D3DDevice.Instance.Device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1.0f, 0);
            //D3DDevice.Instance.Device.RenderState.FillMode = FillMode.WireFrame;

            this.gameModelState.Render();

            this.RenderFPS();
            D3DDevice.Instance.Device.EndScene();
            D3DDevice.Instance.Device.Present();
         
        }

        public override void Dispose()
        {
            this.gameModelState.Dispose();
        }

        public void SetState(GameModelState state)
        {
            this.gameModelState = state;
        }

        public GameModelState GetState()
        {
            return this.gameModelState;
        }
    }
}