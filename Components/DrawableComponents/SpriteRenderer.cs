using Microsoft.Xna.Framework;
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
        public override void Draw(SpriteBatch spriteBatch) {
            var texture = KGame.GetContent<Texture2D>(spriteName);
            spriteBatch.Draw(
                texture,
                GameObject.GlobalPosition,
                null,
                Color.White,
                GameObject.GlobalRotation,
                new Vector2(texture.Width/2f, texture.Height/2f),
                GameObject.GlobalScale,
                SpriteEffects.None,
                LayerDepth
            );
            
            base.Draw(spriteBatch);

        }
    }
}
