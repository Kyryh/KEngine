﻿using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEngine.Components.DrawableComponents
{
    public abstract class DrawableComponent : Component
    {
        public string drawingLayer = "Default";
        protected virtual float LayerDepth => 0f;
        public override void Initialize() {
            base.Initialize();
            KGame.Instance.AddDrawableComponent(this);
        }

        public virtual void Draw(SpriteBatch spriteBatch) {

        }
        public override void OnDestroy()
        {
            KGame.Instance.RemoveDrawableComponent(this);
            base.OnDestroy();
        }
    }
}
