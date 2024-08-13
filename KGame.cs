using KEngine.Components;
using KEngine.Components.DrawableComponents;
using KEngine.Drawing;
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

        Queue<Component> componentsToInitialize = new();
        LinkedList<Component> componentsToStart = new();

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
        static Dictionary<string, Sprite> sprites = new();

        public static T GetContent<T>(string assetName) {
            if (assets.TryGetValue(assetName, out var asset)) {
                return (T)asset;
            }
            assets[assetName] = Instance.Content.Load<T>(assetName);
            return (T)assets[assetName];
        }

        protected static void InitSprites(params Sprite[] sprites) {
            foreach (var sprite in sprites)
            {
                KGame.sprites[sprite.SpriteName] = sprite;
            }
        }

        public static Sprite GetSprite(string spriteName) {
            if (sprites.TryGetValue(spriteName, out var sprite))
                return sprite;
            sprites[spriteName] = new(spriteName, true, Vector2.Zero, Vector2.One);
            return sprites[spriteName];
        }

        protected override void Initialize() {
            base.Initialize();
            foreach (var layer in drawingLayers)
            {
                drawableComponents[layer] = new();
            }
            new GameObject(
                "Main Camera",
                components: new[] {
                    Camera.CreateMainCamera()
                }
            ).Load();
        }
        protected override void LoadContent() {
            base.LoadContent();
            spriteBatch = new SpriteBatch(GraphicsDevice);
        }
        internal void LoadGameObject(GameObject gameObject) {
            gameObjects.Add(gameObject);
            foreach (var component in gameObject.components)
            {
                AddComponent(component);
            }
            foreach (var child in gameObject.Transform.children)
            {
                LoadGameObject(child.GameObject);
            }
        }

        internal void AddComponent(Component component) {
            for (int i = 0; i < components.Count; i++)
            {
                if (components[i].UpdateGroup > component.UpdateGroup) {
                    components.Insert(i, component);
                    return;
                }
            }
            components.Add(component);
            componentsToInitialize.Enqueue(component);
            componentsToStart.AddLast(component);
        }

        internal void AddDrawableComponent(DrawableComponent component) {
            drawableComponents[component.drawingLayer].Add(component);
        }

        internal void RemoveGameObject(GameObject gameObject) {
            gameObjects.Remove(gameObject);
        }
        internal void RemoveComponent(Component component) {
            components.Remove(component);
        }
        internal void RemoveDrawableComponent(DrawableComponent component) {
            drawableComponents[component.drawingLayer].Remove(component);
        }

        protected override void Update(GameTime gameTime) {
            base.Update(gameTime);
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            for (int i = 0; i < componentsToInitialize.Count; i++)
            {
                componentsToInitialize.Dequeue().Initialize();
            }

            var node = componentsToStart.First;
            while (node != null) {
                var next = node.Next;
                if (node.Value.IsActive) {
                    node.Value.Start();
                    componentsToStart.Remove(node);
                }
                node = next;
            }

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
