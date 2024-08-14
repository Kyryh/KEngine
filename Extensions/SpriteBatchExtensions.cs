using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEngine.Extensions {
    public static class SpriteBatchExtensions {
        private static Texture2D texture;
        private static Vector2 center = new Vector2(0.5f, 0.5f);
        private static Vector2 centerLeft = new Vector2(0f, 0.5f);
        private static Texture2D GetTexture(SpriteBatch spriteBatch) {
            
            if (texture == null) {
                texture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
                texture.SetData(new[] { Color.White });
            }
            return texture;
            
        }

        public static void DrawPoint(this SpriteBatch spriteBatch, Vector2 position, Color? color = null, float size = 5f) {
            spriteBatch.Draw(GetTexture(spriteBatch), position, null, color ?? Color.White, 0, center, size, SpriteEffects.None, 0);
        }

        public static void DrawLine(this SpriteBatch spriteBatch, Vector2 position, float rotation, Color color) {
            spriteBatch.Draw(GetTexture(spriteBatch), position, null, color, rotation, centerLeft, 1f, SpriteEffects.None, 0);
        }

    }
}
