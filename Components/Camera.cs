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
                return size;
            }
            set {
                if (value == size)
                    return;
                size = value;
                RecalculateMatrices();
            }
        }

        private static Matrix flipYMatrix = Matrix.CreateScale(1, -1, 1);


        private Matrix worldToScreenMatrix = Matrix.Identity;
        public Matrix WorldToScreenMatrix => worldToScreenMatrix;

        private Matrix screenToWorldMatrix = Matrix.Identity;
        public Matrix ScreenToWorldMatrix => screenToWorldMatrix;

        public static Camera CreateMainCamera() {
            if (MainCamera != null)
                throw new InvalidOperationException("Main camera already initialized");
            MainCamera = new Camera();
            return MainCamera;
        }

        public override void Initialize() {
            base.Initialize();
            RecalculateMatrices();
            Transform.OnPositionChanged += (_, _) => RecalculateMatrices();
            Transform.OnRotationChanged += (_, _) => RecalculateMatrices();
            Transform.OnScaleChanged += (_, _) => RecalculateMatrices();
        }
        void RecalculateMatrices() {
            var screenSize = KGame.GetScreenSize();

            var centerScreenTranslationMatrix = Matrix.CreateTranslation(-screenSize.ToVector3() / 2);
            var rotationMatrix = Matrix.CreateRotationZ(-Transform.GlobalRotation);
            var scaleMatrix = Matrix.CreateScale(size / KGame.GetScreenSize().X);
            var translationMatrix = Matrix.CreateTranslation(Transform.GlobalPosition.ToVector3());

            Matrix.Multiply(ref centerScreenTranslationMatrix, ref flipYMatrix, out screenToWorldMatrix);
            Matrix.Multiply(ref screenToWorldMatrix, ref rotationMatrix, out screenToWorldMatrix);
            Matrix.Multiply(ref screenToWorldMatrix, ref scaleMatrix, out screenToWorldMatrix);
            Matrix.Multiply(ref screenToWorldMatrix, ref translationMatrix, out screenToWorldMatrix);

            Matrix.Invert(ref screenToWorldMatrix, out worldToScreenMatrix);
        }
        public Vector2 WorldToScreen(Vector2 position) {
            Vector2.Transform(ref position, ref worldToScreenMatrix, out var result);
            return result;
        }

        public Vector2 ScreenToWorld(Vector2 position) {
            Vector2.Transform(ref position, ref screenToWorldMatrix, out var result);
            return result;
        }

        public void Draw(SpriteBatch spriteBatch, Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth) {
            spriteBatch.Draw(
                texture,
                WorldToScreen(position),
                sourceRectangle,
                color,
                rotation - Transform.GlobalRotation,
                origin,
                scale / size * KGame.GetScreenSize().X,
                effects,
                layerDepth
            );
        }


    }
}
