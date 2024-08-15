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
        public float width = 1f;
        public float height = 1f;
        public Vector2 offset = Vector2.Zero;
        public IEnumerable<Vector2> Vertices {
            get {
                yield return Vector2.Transform(offset+new Vector2(width/2, height/2), Transform.GlobalMatrix);
                yield return Vector2.Transform(offset +new Vector2(-width/2, height/2), Transform.GlobalMatrix);
                yield return Vector2.Transform(offset +new Vector2(width/2, -height/2), Transform.GlobalMatrix);
                yield return Vector2.Transform(offset +new Vector2(-width/2, -height/2), Transform.GlobalMatrix);
            }
        }

        public override void DebugDraw(SpriteBatch spriteBatch) {
            var vertices = Vertices.ToList();
            spriteBatch.DrawLine(Camera.MainCamera.WorldToScreen(vertices[0]), Camera.MainCamera.WorldToScreen(vertices[1]), color: Color.LightGreen);
            spriteBatch.DrawLine(Camera.MainCamera.WorldToScreen(vertices[1]), Camera.MainCamera.WorldToScreen(vertices[2]), color: Color.LightGreen);
            spriteBatch.DrawLine(Camera.MainCamera.WorldToScreen(vertices[2]), Camera.MainCamera.WorldToScreen(vertices[3]), color: Color.LightGreen);
            spriteBatch.DrawLine(Camera.MainCamera.WorldToScreen(vertices[3]), Camera.MainCamera.WorldToScreen(vertices[0]), color: Color.LightGreen);
            spriteBatch.DrawLine(Camera.MainCamera.WorldToScreen(vertices[0]), Camera.MainCamera.WorldToScreen(vertices[2]), color: Color.LightGreen);
            spriteBatch.DrawLine(Camera.MainCamera.WorldToScreen(vertices[1]), Camera.MainCamera.WorldToScreen(vertices[3]), color: Color.LightGreen);
        }
    }
}
