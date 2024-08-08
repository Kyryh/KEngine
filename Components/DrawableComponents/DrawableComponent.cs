using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEngine.Components.DrawableComponents
{
    public abstract class DrawableComponent : Component
    {
        public int drawingPriority;
        protected DrawableComponent()
        {
            KGame.Instance.AddDrawableComponent(this);
        }

        public override void OnDestroy()
        {
            KGame.Instance.RemoveDrawableComponent(this);
            base.OnDestroy();
        }
    }
}
