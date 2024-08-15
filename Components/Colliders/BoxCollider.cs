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
        private Vector2[] vertices = new Vector2[4];
        public override Vector2[] Vertices {
            get {
                var globalMatrix = Transform.GlobalMatrix;
                var result = new Vector2[4];
                Vector2.Transform(ref vertices[0], ref globalMatrix, out result[0]);
                Vector2.Transform(ref vertices[1], ref globalMatrix, out result[1]);
                Vector2.Transform(ref vertices[2], ref globalMatrix, out result[2]);
                Vector2.Transform(ref vertices[3], ref globalMatrix, out result[3]);
                return result;
            }
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
