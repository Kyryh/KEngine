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

        HashSet<GameObject> gameObjects = new();
        SortedSet<Component> components = new(new Component.UpdatePriorityComparer());
        Dictionary<string, HashSet<DrawableComponent>> drawableComponents = new();

        protected string[] drawingLayers = new string[] {
            "Default"
        };

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

            foreach (var component in components) {
                component.Update(gameTime.ElapsedGameTime.TotalSeconds);
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
