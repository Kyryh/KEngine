using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KEngine.Extensions;

namespace KEngine.Components {
    public class Camera : Component {
        public static Camera MainCamera { get; private set; }

        private float size = 10f;
        public float Size {
            get {
                return size / KGame.GetScreenSize().X;
            }
            set {
                size = value * KGame.GetScreenSize().X;
            }
        }

        public Matrix WorldToScreenMatrix { private set; get; }
        public Matrix ScreenToWorldMatrix { private set; get; }

        public static Camera CreateMainCamera() {
            if (MainCamera != null)
                throw new InvalidOperationException("Main camera already initialized");
            MainCamera = new Camera();
            return MainCamera;
        }


        void RecalculateMatrices() {
            var screenSize = KGame.GetScreenSize();
            ScreenToWorldMatrix = 
                  Matrix.CreateTranslation(-screenSize.ToVector3() / 2)
                * Matrix.CreateScale(1, -1, 1)
                * Matrix.CreateRotationZ(-GameObject.GlobalRotation)
                * Matrix.CreateScale(Size)
                * Matrix.CreateTranslation(GameObject.GlobalPosition.ToVector3())
                ;

            WorldToScreenMatrix = Matrix.Invert(ScreenToWorldMatrix);
        }
        public Vector2 WorldToScreen(Vector2 position) {

            RecalculateMatrices();

            
            return Vector2.Transform(position, WorldToScreenMatrix);
        }

        public Vector2 ScreenToWorld(Vector2 position) {
            position += GameObject.GlobalPosition - KGame.GetScreenSize() / 2;
            position.Y *= -1;
            return position;
        }

        public float WorldToScreen(float rotation) {
            return rotation - GameObject.GlobalRotation;
        }

        public float ScreenToWorld(float rotation) {
            return rotation + GameObject.GlobalRotation;
        }


    }
}
