using KEngine.Components;
using KEngine.Components.Colliders;
using KEngine.Components.DrawableComponents;
using KEngine.Drawing;
using KEngine.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace KEngine {
    public abstract class KGame : Game {
        public static KGame Instance { private set; get; }

        protected GraphicsDeviceManager graphics;
        protected SpriteBatch spriteBatch;

        List<GameObject> gameObjects = new();
        List<Component> components = new();
        Dictionary<string, List<DrawableComponent>> drawableComponents = new();
        List<Collider> colliders = new();

        Queue<Component> componentsToInitialize = new();
        LinkedList<Component> componentsToStart = new();

        protected bool debugDrawGameObjectsPosition = false;
        protected bool debugDrawColliders = false;

        protected string[] drawingLayers = new string[] {
            "Default"
        };
        private Dictionary<string, DrawingLayerSettings> drawingLayerSettings = new();

        public delegate void SceneLoader();

        private OrderedDictionary scenes = new();

        protected void SetDrawingLayersSettings(DrawingLayerSettings defaultSettings, Dictionary<string, DrawingLayerSettings> layersSettings) {
            foreach (var layer in drawingLayers) {
                if (!layersSettings.TryGetValue(layer, out var settings)) {
                    settings = defaultSettings;
                }
                drawingLayerSettings[layer] = new DrawingLayerSettings(
                    settings.SortMode ?? defaultSettings.SortMode ?? SpriteSortMode.Deferred,
                    settings.BlendState ?? defaultSettings.BlendState,
                    settings.SamplerState ?? defaultSettings.SamplerState,
                    settings.DepthStencilState ?? defaultSettings.DepthStencilState,
                    settings.RasterizerState ?? defaultSettings.RasterizerState,
                    settings.Effect ?? defaultSettings.Effect
                );
            }

        }

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
                },
                dontDestroyOnLoad: true
            ).Load();
            LoadScene(0);
        }
        public void LoadScene(int index) {
            LoadScene((SceneLoader)scenes[index]);
        }
        public void LoadScene(string name) {
            LoadScene((SceneLoader)scenes[name]);
        }

        protected void SetScenes(params (string, SceneLoader)[] scenes) {
            foreach (var scene in scenes)
            {
                this.scenes[scene.Item1] = scene.Item2;
            }
        }
        private void LoadScene(SceneLoader sceneLoader) {
            int i = 0;
            while (i < gameObjects.Count) {
                if (gameObjects[i].CanDestroy) {
                    gameObjects[i].Destroy();
                } else {
                    i++;
                }
            }
            gameObjects.RemoveAll(go => !go.DontDestroyOnLoad);
            sceneLoader();
        }

        protected override void LoadContent() {
            base.LoadContent();
            spriteBatch = new SpriteBatch(GraphicsDevice);
            SetDrawingLayersSettings(new DrawingLayerSettings(), new());
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

        internal void AddCollider(Collider collider) {
            colliders.Add(collider);
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

        internal void RemoveCollider(Collider collider) {
            colliders.Remove(collider);
        }

        protected override void Update(GameTime gameTime) {
            base.Update(gameTime);
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            while (componentsToInitialize.Count > 0)
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

            for (int i = 0; i < colliders.Count; i++) {
                for (int j = i+1; j < colliders.Count; j++) {
                    if (Collider.CheckCollision(colliders[i], colliders[j], out var hitInfo)) {
                        if (!hitInfo.colliderA.IsTrigger && !hitInfo.colliderB.IsTrigger) {
                            if (hitInfo.colliderA.IsStatic) {
                                hitInfo.colliderB.Transform.Position += hitInfo.direction * hitInfo.distance;
                            } else if (hitInfo.colliderB.IsStatic) {
                                hitInfo.colliderA.Transform.Position -= hitInfo.direction * hitInfo.distance;
                            } else {
                                hitInfo.colliderB.Transform.Position += hitInfo.direction * (hitInfo.distance * 0.5f);
                                hitInfo.colliderA.Transform.Position -= hitInfo.direction * (hitInfo.distance * 0.5f);
                            }
                        }
                        hitInfo.colliderA.CallOnCollision(hitInfo.colliderB);
                        hitInfo.colliderB.CallOnCollision(hitInfo.colliderA);
                    }
                }
            }

        }

        protected override void Draw(GameTime gameTime) {
            base.Draw(gameTime);

            for (int i = 0; i < drawingLayers.Length; i++)
            {
                var layer = drawingLayers[i];
                var settings = drawingLayerSettings[layer];
                spriteBatch.Begin(settings.SortMode.Value, settings.BlendState, settings.SamplerState, settings.DepthStencilState, settings.RasterizerState, settings.Effect);
                foreach (var component in drawableComponents[layer])
                {
                    component.Draw(spriteBatch);
                }
                spriteBatch.End();
            }
            spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            if (debugDrawGameObjectsPosition) {
                foreach (var gameObject in gameObjects) {
                    spriteBatch.DrawPoint(Camera.MainCamera.WorldToScreen(gameObject.Transform.GlobalPosition), Color.Gray);
                }
            }
            if (debugDrawColliders) {
                foreach (var collider in colliders) {
                    collider.DebugDraw(spriteBatch);
                }
            }
            spriteBatch.End();
        }

        protected record DrawingLayerSettings(
            SpriteSortMode? SortMode = null,
            BlendState BlendState = null,
            SamplerState SamplerState = null,
            DepthStencilState DepthStencilState = null,
            RasterizerState RasterizerState = null,
            Effect Effect = null
        );
    }
}
