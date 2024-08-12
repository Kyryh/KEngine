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

        public float size = 10f;


        public Matrix WorldToScreenMatrix { private set; get; }
        public Matrix ScreenToWorldMatrix { private set; get; }

        public static Camera CreateMainCamera() {
            if (MainCamera != null)
                throw new InvalidOperationException("Main camera already initialized");
            MainCamera = new Camera();
            return MainCamera;
        }


        public override void Update(float deltaTime) {
            base.Update(deltaTime);
            RecalculateMatrices();
        }
        void RecalculateMatrices() {
            var screenSize = KGame.GetScreenSize();
            ScreenToWorldMatrix = Matrix.Identity
                * Matrix.CreateTranslation(-screenSize.ToVector3() / 2)
                * Matrix.CreateScale(1, -1, 1)
                * Matrix.CreateRotationZ(-Transform.GlobalRotation)
                * Matrix.CreateScale(size / KGame.GetScreenSize().X)
                * Matrix.CreateTranslation(Transform.GlobalPosition.ToVector3())
                ;

            WorldToScreenMatrix = Matrix.Invert(ScreenToWorldMatrix);
        }
        public Vector2 WorldToScreen(Vector2 position) {
            return Vector2.Transform(position, WorldToScreenMatrix);
        }

        public Vector2 ScreenToWorld(Vector2 position) {
            return Vector2.Transform(position, ScreenToWorldMatrix);
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
