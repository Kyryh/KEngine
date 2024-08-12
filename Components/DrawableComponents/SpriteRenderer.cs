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
        public override void Initialize() {
            base.Initialize();
            sprite = KGame.GetSprite(spriteName);
            
        }
        public override void Draw(SpriteBatch spriteBatch) {
            Camera.MainCamera.Draw(
                spriteBatch,
                sprite.Texture,
                Transform.GlobalPosition,
                null,
                Color.White,
                Transform.GlobalRotation,
                sprite.Center+sprite.Offset,
                sprite.Scale*Transform.GlobalScale,
                SpriteEffects.None,
                LayerDepth
            );
            
            base.Draw(spriteBatch);

        }
    }
}
