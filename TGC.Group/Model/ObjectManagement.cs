using TGC.Core.SceneLoader;
using TGC.Core.Mathematica;
using TGC.Core.Input;
using Microsoft.DirectX.DirectInput;
using TGC.Core.Text;
using System.Drawing;

namespace TGC.Group.Model
{
    class ObjectManagement
    {
        private TgcMesh mesh;
        private TGCMatrix traslation, rotation, escalade;
        private ThirdPersonCamera camara;
        private float epsilon = 0.1f;
        private TgcText2D texto;

        public ObjectManagement(string pathToMesh, ThirdPersonCamera camara)
        {
            this.CreateMesh(pathToMesh);
            this.camara = camara;
        }

        private void CreateMesh(string pathToMesh)
        {
            TgcSceneLoader loader = new TgcSceneLoader();
            TgcScene scene = loader.loadSceneFromFile(pathToMesh);
            this.mesh = scene.Meshes[0];
            this.mesh.AutoTransform = false;
            this.rotation = TGCMatrix.RotationYawPitchRoll(0, 0, 0);
            this.escalade = TGCMatrix.Scaling(new TGCVector3(0.2f,0.2f,0.2f));
            this.traslation = TGCMatrix.Translation(0,0,0);
        }

        private void GenerarTexto()
        {
            string dialogo = "Posicion = ({0} | {1} | {2})";
            dialogo = string.Format(dialogo, this.GetPosition().X, this.GetPosition().Y, this.GetPosition().Z);
            texto = Text.newText(dialogo, 120, 300);
            texto.Color = Color.Blue;
        }

        private void substractEpsilon()
        {
            epsilon -= 0.01f;
            epsilon = this.Maximun(epsilon, 0);
        }

        private float Maximun(float a, float b)
        {
            return (a < b) ? b : a;
        }

        public void Action(TgcD3dInput input)
        {
            if (input.keyDown(Key.NumPad4))
            {
                this.camara.rotateY(-0.005f);
            }
            if (input.keyDown(Key.NumPad6))
            {
                this.camara.rotateY(0.005f);
            }

            if (input.keyDown(Key.RightArrow))
            {
                this.camara.OffsetHeight += 0.05f;
            }
            if (input.keyDown(Key.LeftArrow))
            {
                this.camara.OffsetHeight -= 0.05f;
            }

            if (input.keyDown(Key.UpArrow))
            {
                this.camara.OffsetForward += 0.05f;
            }
            if (input.keyDown(Key.DownArrow))
            {
                this.camara.OffsetForward -= 0.05f;
            }

            if (input.keyDown(Key.NumPadPlus))
            {
                epsilon += 0.01f;
            }
            if (input.keyDown(Key.NumPadMinus))
            {
                this.substractEpsilon();
            }
            if (input.keyDown(Key.I))
            {
                this.MoveForward();
            }
            if (input.keyDown(Key.K))
            {
                this.MoveBackward();
            }
            if (input.keyDown(Key.J))
            {
                this.MoveLeft();
            }
            if (input.keyDown(Key.L))
            {
                this.MoveRight();
            }
            if (input.keyDown(Key.U))
            {
                this.RotateY();
            }
            if (input.keyDown(Key.O))
            {
                this.MoveUp();
            }
            if (input.keyDown(Key.P))
            {
                this.MoveDown();
            }


            this.UpdateCamera();
        }

        private void RotateY()
        {
            this.rotation = TGCMatrix.RotationY(epsilon * 0.8f) * this.rotation;
        }

        private void MoveUp()
        {
            this.traslation = TGCMatrix.Translation(0, epsilon, 0) * this.traslation;
        }

        private void MoveDown()
        {
            this.traslation = TGCMatrix.Translation(0, -epsilon, 0) * this.traslation;
        }

        private void MoveForward()
        {
            this.traslation = TGCMatrix.Translation(0,0,epsilon) * this.traslation;
        }

        private void MoveBackward()
        {
            this.traslation = TGCMatrix.Translation(0, 0, -epsilon) * this.traslation;
        }

        private void MoveLeft()
        {
            this.traslation = TGCMatrix.Translation(-epsilon, 0, 0) * this.traslation;
        }

        private void MoveRight()
        {
            this.traslation = TGCMatrix.Translation(epsilon, 0, 0) * this.traslation;
        }

        private TGCVector3 GetPosition()
        {
            return TGCVector3.transform(new TGCVector3(0, 0, 0), this.rotation * this.traslation);
        }

        private void UpdateCamera()
        {
            this.camara.Target = this.GetPosition() + new TGCVector3(0,0,30f);
        }

        public void Transform()
        {
            TGCMatrix transformation = this.escalade * this.rotation * this.traslation;
            this.mesh.Transform = transformation;
            this.mesh.BoundingBox.transform(transformation);
            this.GenerarTexto();
        }

        public void Render()
        {
            this.mesh.Render();
            this.mesh.BoundingBox.Render();
            this.texto.render();
        }

    }
}
