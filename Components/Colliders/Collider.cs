using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEngine.Components.Colliders {
    public abstract class Collider : Component {
        public override void Initialize() {
            base.Initialize();
            KGame.Instance.AddCollider(this);
        }

        public override void OnDestroy() {
            base.OnDestroy();
            KGame.Instance.RemoveCollider(this);
        }

        public virtual void DebugDraw(SpriteBatch spriteBatch) {

        }
    }
}
