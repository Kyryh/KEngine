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
        public bool FlipX {
            get {
                return flippedEffect.HasFlag(SpriteEffects.FlipHorizontally);
            }
            set {
                if (value)
                    flippedEffect |= SpriteEffects.FlipHorizontally;
                else
                    flippedEffect &= ~SpriteEffects.FlipHorizontally;
            }
        }
        public bool FlipY {
            get {
                return flippedEffect.HasFlag(SpriteEffects.FlipVertically);
            }
            set {
                if (value)
                    flippedEffect |= SpriteEffects.FlipVertically;
                else
                    flippedEffect &= ~SpriteEffects.FlipVertically;
            }
        }

        SpriteEffects flippedEffect = SpriteEffects.None;
        public override void Initialize() {
            base.Initialize();
            sprite = KGame.GetSprite(spriteName);
            
        }
        public override void Draw(SpriteBatch spriteBatch) {
            Camera.MainCamera.Draw(
                spriteBatch,
                sprite.Texture,
                Transform.GlobalPosition,
                sprite.GetSourceRectangle(spriteIndex),
                Color.White,
                Transform.GlobalRotation,
                sprite.Center+sprite.Offset,
                sprite.Scale*Transform.GlobalScale,
                flippedEffect,
                LayerDepth
            );
            
            base.Draw(spriteBatch);

        }
    }
}
