using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEngine.Drawing {
    public class SpriteSheet : Sprite {
        public int Columns { get; }
        public int Rows { get; }
        public Rectangle this[int index] {
            get {
                return this[index%Columns, index/Columns];
            }
        }

        public Rectangle this[int column, int row] {
            get {
                Vector2 position = new Vector2(Size.X*column, Size.Y*row);
                return new Rectangle((int)position.X, (int)position.Y, (int)Size.X, (int)Size.Y);
            }
        }
        public override Rectangle? GetSourceRectangle(int index) => this[index];
        public SpriteSheet(string spriteName, int columns, int rows, bool scaleTo1x1 = true, Vector2? offset = null, Vector2? scale = null) : base(spriteName, scaleTo1x1, offset, scale) {
            Size /= new Vector2(columns, rows);

            Columns = columns;
            Rows = rows;

            Offset = (offset ?? Vector2.Zero) * Size;
            Center = Size * 0.5f;
            scale ??= Vector2.One;
            Scale = scaleTo1x1 ? scale.Value / Size : scale.Value;
        }

        public SpriteSheet(string spriteName, Vector2 spriteSize, bool scaleTo1x1 = true, Vector2? offset = null, Vector2? scale = null) : base(spriteName, scaleTo1x1, offset, scale) {
            Size = spriteSize;

            Columns = (int)(Texture.Width / Size.X);
            Rows = (int)(Texture.Height / Size.Y);

            Offset = (offset ?? Vector2.Zero) * Size;
            Center = Size * 0.5f;
            scale ??= Vector2.One;
            Scale = scaleTo1x1 ? scale.Value / Size : scale.Value;
        }
    }
}
