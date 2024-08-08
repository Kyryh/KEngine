using System;
using System.Collections.Generic;
using KEngine.Components;
using Microsoft.Xna.Framework;

namespace KEngine
{
    public class GameObject {

        public string Name { get; init; }
        public GameObject parent;
        public List<GameObject> children;
        public Transform Transform { get; }
        private readonly List<Component> components = new();


        public GameObject(
            string name,
            Vector2? position = null,
            float rotation = 0,
            Vector2? scale = null
        ) {
            Name = name;
            Transform = new Transform() {
                position = position ?? Vector2.Zero,
                rotation = rotation,
                scale = scale ?? Vector2.One
            };

            KGame.Instance.AddGameObject(this);
        }
        public void AddComponent<T>() where T : Component, new() {
            AddComponent(new T());
        }

        private void AddComponent(Component component) {
            if (components.Contains(component))
                throw new ArgumentException($"Component is already present in GameObject {Name}");
            components.Add(component);
            component.Initialize();
        }

        public T GetComponent<T>(bool activeOnly = true, bool searchInChildren = false) where T : Component {
            for (int i = 0; i < components.Count; i++) {
                if (!(activeOnly || components[i].Active))
                    continue;

                if (components[i] is T component) {
                    return component;
                }
            }
            if (searchInChildren) {
                for (int i = 0; i < children.Count; i++) {
                    if (children[i].TryGetComponent<T>(out var component, activeOnly, true))
                        return component;
                }
            }

            return null;
        }

        public bool TryGetComponent<T>(out T component, bool activeOnly = true, bool searchInChildren = false) where T : Component {
            component = GetComponent<T>(activeOnly, searchInChildren);
            return component != null;
        }

        public List<T> GetComponents<T>(bool activeOnly = true, bool searchInChildren = false) where T : Component {
            List<T> result = new();
            for (int i = 0; i < components.Count; i++) {
                if (!(activeOnly || components[i].Active))
                    continue;

                if (components[i] is T component) {
                    result.Add(component);
                }
            }
            if (searchInChildren) {
                for (int i = 0; i < children.Count; i++) {
                    result.AddRange(children[i].GetComponents<T>(activeOnly, true));
                }
            }
            return result;
        }

        public void Destroy() {
            for (int i = 0; i < components.Count; i++) {
                components[i].OnDisable();
                components[i].OnDestroy();
            }
            for (int i = 0; i < children.Count; i++)
            {
                children[i].Destroy();
            }
            KGame.Instance.RemoveGameObject(this);
        }

    }
}
