using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEngine.Components {
    internal class Camera : Component {
        public static Camera MainCamera { get; private set; }

        public float size;

        public static Camera CreateMainCamera() {
            if (MainCamera != null)
                throw new InvalidOperationException("Main camera already initialized");
            MainCamera = new Camera();
            return MainCamera;
        }
        public Vector2 WorldToScreen(Vector2 position) {
            position.Y *= -1;
            position += KGame.GetScreenSize()/2 - GameObject.GlobalPosition;
            return position;
        }

        public Vector2 ScreenToWorld(Vector2 position) {
            position.Y *= -1;
            position += GameObject.GlobalPosition - KGame.GetScreenSize() / 2;
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
