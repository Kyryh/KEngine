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

        protected GraphicsDeviceManager graphics;
        protected SpriteBatch spriteBatch;


        protected bool reorderDrawablesBeforeDrawing;

        List<GameObject> gameObjects = new();
        List<Component> components = new();
        List<DrawableComponent> drawableComponents = new();
        
        protected KGame()
        {
            Instance = this;
            graphics = new GraphicsDeviceManager(this);
        }


        static Dictionary<string, object> assets = new();

        public static T GetContent<T>(string assetName) {
            if (assets.TryGetValue(assetName, out var asset)) {
                return (T)asset;
            }
            assets[assetName] = Instance.Content.Load<T>(assetName);
            return (T)assets[assetName];
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
                component.Update(gameTime.ElapsedGameTime.TotalSeconds);
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

            spriteBatch.Begin();
            for (int i = 0; i < drawableComponents.Count; i++)
            {
                drawableComponents[i].Draw(spriteBatch);
            }
            spriteBatch.End();
        }

    }
}
