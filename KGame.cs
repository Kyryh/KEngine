using KEngine.Components;
using KEngine.Components.DrawableComponents;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace KEngine {
    public abstract class KGame : Game {
        public static KGame Instance { private set; get; }

        protected GraphicsDeviceManager graphics;
        protected SpriteBatch spriteBatch;

        List<GameObject> gameObjects = new();
        List<Component> components = new();
        Dictionary<string, List<DrawableComponent>> drawableComponents = new();

        protected string[] drawingLayers = new string[] {
            "Default"
        };

        protected KGame()
        {
            Instance = this;
            graphics = new GraphicsDeviceManager(this);
        }

        public static Vector2 GetScreenSize() {
            return new Vector2(Instance.graphics.PreferredBackBufferWidth, Instance.graphics.PreferredBackBufferHeight);
        }

        static Dictionary<string, object> assets = new();

        public static T GetContent<T>(string assetName) {
            if (assets.TryGetValue(assetName, out var asset)) {
                return (T)asset;
            }
            assets[assetName] = Instance.Content.Load<T>(assetName);
            return (T)assets[assetName];
        }

        protected override void Initialize() {
            base.Initialize();
            foreach (var layer in drawingLayers)
            {
                drawableComponents[layer] = new();
            }
        }
        protected override void LoadContent() {
            base.LoadContent();
            spriteBatch = new SpriteBatch(GraphicsDevice);
        }
        public void AddGameObject(GameObject gameObject) {
            gameObjects.Add(gameObject);
        }

        public void AddComponent(Component component) {
            for (int i = 0; i < components.Count; i++)
            {
                if (components[i].UpdateGroup > component.UpdateGroup) {
                    components.Insert(i, component);
                    return;
                }
            }
            components.Add(component);
        }

        public void AddDrawableComponent(DrawableComponent component) {
            drawableComponents[component.drawingLayer].Add(component);
        }

        public void RemoveGameObject(GameObject gameObject) {
            gameObjects.Remove(gameObject);
        }
        public void RemoveComponent(Component component) {
            components.Remove(component);
        }
        public void RemoveDrawableComponent(DrawableComponent component) {
            drawableComponents[component.drawingLayer].Remove(component);
        }

        protected override void Update(GameTime gameTime) {
            base.Update(gameTime);
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            foreach (var component in components) {
                component.Update(deltaTime);
            }
        }

        protected override void Draw(GameTime gameTime) {
            base.Draw(gameTime);

            for (int i = 0; i < drawingLayers.Length; i++)
            {
                var layer = drawingLayers[i];
                spriteBatch.Begin();
                foreach (var component in drawableComponents[layer])
                {
                    component.Draw(spriteBatch);
                }
                spriteBatch.End();
            }
        }

    }
}
