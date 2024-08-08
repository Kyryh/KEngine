using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEngine.Components {
    public abstract class DrawableComponent : Component {
        public virtual int DrawingPriority => 0;
        protected DrawableComponent()
        {
            KGame.Instance.AddDrawableComponent(this);
        }
    }
}
