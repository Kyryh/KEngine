using System;
using System.Collections.Generic;
using KEngine.Components;
using Microsoft.Xna.Framework;

namespace KEngine
{
    public class GameObject {

        public string Name { get; init; }

        public bool active;
        public bool IsActive {
            get {
                if (parent == null)
                    return active;
                return active && parent.IsActive;
            }
        }

        public GameObject parent;
        public List<GameObject> children;

        public Vector2 position;
        public float rotation;
        public Vector2 scale;

        private readonly List<Component> components = new();


        public GameObject(
            string name,
            Vector2? position = null,
            float rotation = 0,
            Vector2? scale = null,
            bool active = true,
            Component[] components = null,
            GameObject[] children = null
        ) {
            Name = name;

            this.position = position ?? Vector2.Zero;
            this.rotation = rotation;
            this.scale = scale ?? Vector2.One;

            this.active = active;

            if (components != null) {
                for (int i = 0; i < components.Length; i++) {
                    AddComponent(components[i]);
                }
            }
            if (children != null) {
                this.children.AddRange(children);
                foreach (var child in children)
                {
                    child.parent = this;
                }
            }

            KGame.Instance.AddGameObject(this);
        }

        public Vector2 GlobalPosition {
            get {
                if (parent == null)
                    return position;
                return parent.GlobalPosition + position;
            }
        }

        public float GlobalRotation {
            get {
                if (parent == null)
                    return rotation;
                return parent.GlobalRotation + rotation;
            }
        }

        public Vector2 GlobalScale {
            get {
                if (parent == null)
                    return scale;
                return parent.GlobalScale + scale;
            }
        }

        private void AddComponent(Component component) {
            if (components.Contains(component))
                throw new ArgumentException($"Component is already present in GameObject {Name}");
            components.Add(component);
            component.GameObject = this;
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
