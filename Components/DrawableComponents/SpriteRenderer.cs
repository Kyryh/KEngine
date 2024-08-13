using Microsoft.Xna.Framework;
using KEngine.Drawing;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEngine.Components.DrawableComponents {
    public class SpriteRenderer : DrawableComponent {
        public string spriteName;
        Sprite sprite;
        public int spriteIndex;
        public bool flipX;
        public bool flipY;
        public override void Initialize() {
            base.Initialize();
            sprite = KGame.GetSprite(spriteName);
            
        }
        public override void Draw(SpriteBatch spriteBatch) {
            var effects = SpriteEffects.None;

            if (flipX)
                effects |= SpriteEffects.FlipHorizontally;
            if (flipY)
                effects |= SpriteEffects.FlipVertically;

            Camera.MainCamera.Draw(
                spriteBatch,
                sprite.Texture,
                Transform.GlobalPosition,
                sprite.GetSourceRectangle(spriteIndex),
                Color.White,
                Transform.GlobalRotation,
                sprite.Center+sprite.Offset,
                sprite.Scale*Transform.GlobalScale,
                effects,
                LayerDepth
            );
            
            base.Draw(spriteBatch);

        }
    }
}
