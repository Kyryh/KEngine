using KEngine.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEngine.Components.Colliders {
    public class BoxCollider : Collider {
        public float Width { get; init; } = 1f;
        public float Height { get; init; } = 1f;
        public Vector2 Offset { get; init; } = Vector2.Zero;
        public float Rotation { get; init; } = 0f;

        private bool recalculateVertices = true;
        private bool recalculateAxes = true;
        private readonly Vector2[] vertices = new Vector2[4];
        private readonly Vector2[] globalVertices = new Vector2[4];
        public override Vector2[] Vertices {
            get {
                if (recalculateVertices) {
                    recalculateVertices = false;
                    var globalMatrix = Transform.GlobalMatrix;
                    Vector2.Transform(ref vertices[0], ref globalMatrix, out globalVertices[0]);
                    Vector2.Transform(ref vertices[1], ref globalMatrix, out globalVertices[1]);
                    Vector2.Transform(ref vertices[2], ref globalMatrix, out globalVertices[2]);
                    Vector2.Transform(ref vertices[3], ref globalMatrix, out globalVertices[3]);
                }
                return globalVertices;
            }
        }

        private Vector2[] axes = new Vector2[2];
        public override Vector2[] Axes {
            get {
                if (recalculateAxes) {
                    recalculateAxes = false;
                    var rotationMatrix = Matrix.CreateRotationZ(-Transform.GlobalRotation);
                    var axis = GameConstants.Vector2.Up;

                    Vector2.Transform(ref axis, ref rotationMatrix, out axes[0]);
                    Vector2.Normalize(ref axes[0], out axes[0]);

                    axes[1].X = axes[0].Y;
                    axes[1].Y = -axes[0].X;
                }
                return axes;
            }
        }

        public override void Initialize() {
            base.Initialize();
            vertices[0] = new Vector2(Width / 2, Height / 2);
            vertices[1] = new Vector2(Width / 2, -Height / 2);
            vertices[2] = new Vector2(-Width / 2, -Height / 2);
            vertices[3] = new Vector2(-Width / 2, Height / 2);

            var rotationMatrix = Matrix.CreateRotationZ(Rotation);
            var translationMatrix = Matrix.CreateTranslation(Offset.ToVector3());
            Matrix.Multiply(ref rotationMatrix, ref translationMatrix, out var matrix);
            for (int i = 0; i < vertices.Length; i++) {
                Vector2.Transform(ref vertices[i], ref matrix, out vertices[i]);
            }

            Transform.OnPositionChanged += (_, _) => RecalculateNeeded();
            Transform.OnRotationChanged += (_, _) => RecalculateNeeded();
            Transform.OnScaleChanged += (_, _) => RecalculateNeeded();


        }

        void RecalculateNeeded() {
            recalculateVertices = true;
            recalculateAxes = true;
        }
        public override void DebugDraw(SpriteBatch spriteBatch) {
            var vertices = Vertices;
            spriteBatch.DrawLine(Camera.MainCamera.WorldToScreen(vertices[0]), Camera.MainCamera.WorldToScreen(vertices[1]), color: Color.LightGreen);
            spriteBatch.DrawLine(Camera.MainCamera.WorldToScreen(vertices[1]), Camera.MainCamera.WorldToScreen(vertices[2]), color: Color.LightGreen);
            spriteBatch.DrawLine(Camera.MainCamera.WorldToScreen(vertices[2]), Camera.MainCamera.WorldToScreen(vertices[3]), color: Color.LightGreen);
            spriteBatch.DrawLine(Camera.MainCamera.WorldToScreen(vertices[3]), Camera.MainCamera.WorldToScreen(vertices[0]), color: Color.LightGreen);
            spriteBatch.DrawLine(Camera.MainCamera.WorldToScreen(vertices[0]), Camera.MainCamera.WorldToScreen(vertices[2]), color: Color.LightGreen);
            spriteBatch.DrawLine(Camera.MainCamera.WorldToScreen(vertices[1]), Camera.MainCamera.WorldToScreen(vertices[3]), color: Color.LightGreen);
        }
    }
}
