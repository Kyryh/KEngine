using KEngine.Components;
using KEngine.Components.DrawableComponents;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEngine {
    public abstract class KGame : Game {
        public static KGame Instance { private set; get; }

        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;


        protected bool reorderDrawablesBeforeDrawing;

        List<GameObject> gameObjects = new();
        List<Component> components = new();
        List<DrawableComponent> drawableComponents = new();
        
        protected KGame()
        {
            Instance = this;
            graphics = new GraphicsDeviceManager(this);
        }

        protected override void LoadContent() {
            base.LoadContent();
            spriteBatch = new SpriteBatch(GraphicsDevice);
        }
        public void AddGameObject(GameObject gameObject) {
            gameObjects.Add(gameObject);
        }

        public void AddComponent(Component component) {
            // TODO: Don't just append, add it in the correct spot 
            components.Add(component);
        }

        public void AddDrawableComponent(DrawableComponent component) {
            // TODO: same thing as above
            drawableComponents.Add(component);
        }

        public void RemoveGameObject(GameObject gameObject) {
            gameObjects.Remove(gameObject);
        }
        public void RemoveComponent(Component component) {
            components.Remove(component);
        }
        public void RemoveDrawableComponent(DrawableComponent component) {
            drawableComponents.Remove(component);
        }

        protected override void Update(GameTime gameTime) {
            base.Update(gameTime);

            foreach (var component in components) {
                component.Update(gameTime.ElapsedGameTime.Seconds);
            }
        }

        protected virtual int DrawableComparer(DrawableComponent a, DrawableComponent b) {
            return 0;
        }

        protected override void Draw(GameTime gameTime) {
            base.Draw(gameTime);
            if (reorderDrawablesBeforeDrawing) {
                drawableComponents.Sort(DrawableComparer);
            }
            for (int i = 0; i < drawableComponents.Count; i++)
            {
                drawableComponents[i].Draw(spriteBatch);
            }
        }

    }
}
