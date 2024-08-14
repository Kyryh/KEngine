using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KEngine.Drawing
{
    public class Sprite
    {
        public string SpriteName { get; }
        public Texture2D Texture { get; }
        public Vector2 Offset { get; init; }
        public Vector2 Size { get; init; }
        public Vector2 Center { get; init; }
        public Vector2 Scale { get; init; }
        public virtual Rectangle? GetSourceRectangle(int index) => null;
        public Sprite(string spriteName, bool scaleTo1x1 = true, Vector2? offset = null, Vector2? scale = null)
        {
            SpriteName = spriteName;
            Texture = KGame.GetContent<Texture2D>(spriteName);
            Size = new Vector2(Texture.Width, Texture.Height);
            Center = Size * 0.5f;
            Offset = Center + (offset ?? Vector2.Zero) * Size;
            scale ??= Vector2.One;
            Scale = scaleTo1x1 ? scale.Value / Size : scale.Value;
        }
    }
}
