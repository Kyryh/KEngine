using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KEngine {
    public class Sprite {
        public string SpriteName { get; }
        public Texture2D Texture { get; }
        public Vector2 Offset { get; }
        public Vector2 Size { get; }
        public Vector2 Center { get; }
        public Vector2 Scale { get; }
        public Sprite(string spriteName, bool scaleTo1x1 = true, Vector2? offset = null, Vector2? scale = null) {
            SpriteName = spriteName;
            Texture = KGame.GetContent<Texture2D>(spriteName);
            Size = new Vector2(Texture.Width, Texture.Height);
            Offset = (offset ?? Vector2.Zero) * Size;
            Center = Size * 0.5f;
            scale ??= Vector2.One;
            Scale = scaleTo1x1 ? scale.Value / Size : scale.Value;
        }
    }
}
