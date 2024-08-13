using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEngine.Components.DrawableComponents {
    public class YSortedSpriteRenderer : SpriteRenderer {
        protected override float LayerDepth => GameObject.Transform.GlobalPosition.Y/float.MaxValue;
    }
}
