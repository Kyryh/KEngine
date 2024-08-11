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
                if (Parent == null)
                    return active;
                return active && Parent.IsActive;
            }
        }

        public GameObject Parent { get; private set; }
        public readonly GameObject[] children = Array.Empty<GameObject>();

        public Vector2 position;
        public float rotation;
        public Vector2 scale;

        private readonly Component[] components = Array.Empty<Component>();


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
                this.components = new Component[components.Length];
                for (int i = 0; i < components.Length; i++) {
                    this.components[i] = components[i];
                    this.components[i].GameObject = this;
                }
            }

            if (children != null) {
                this.children = new GameObject[children.Length];
                for (int i = 0; i < children.Length; i++) {
                    this.children[i] = children[i];
                    this.children[i].Parent = this;
                }
            }

            KGame.Instance.AddGameObject(this);
        }

        public Vector2 GlobalPosition {
            get {
                if (Parent == null)
                    return position;
                return Parent.GlobalPosition + position;
            }
        }

        public float GlobalRotation {
            get {
                if (Parent == null)
                    return rotation;
                return Parent.GlobalRotation + rotation;
            }
        }

        public Vector2 GlobalScale {
            get {
                if (Parent == null)
                    return scale;
                return Parent.GlobalScale + scale;
            }
        }


        public T GetComponent<T>(bool activeOnly = true, bool searchInChildren = false) where T : Component {
            for (int i = 0; i < components.Length; i++) {
                if (!(activeOnly || components[i].IsActive))
                    continue;

                if (components[i] is T component) {
                    return component;
                }
            }
            if (searchInChildren) {
                for (int i = 0; i < children.Length; i++) {
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
            for (int i = 0; i < components.Length; i++) {
                if (!(activeOnly || components[i].IsActive))
                    continue;

                if (components[i] is T component) {
                    result.Add(component);
                }
            }
            if (searchInChildren) {
                for (int i = 0; i < children.Length; i++) {
                    result.AddRange(children[i].GetComponents<T>(activeOnly, true));
                }
            }
            return result;
        }

        public void Destroy() {
            for (int i = 0; i < components.Length; i++) {
                components[i].OnDisable();
                components[i].OnDestroy();
            }
            for (int i = 0; i < children.Length; i++)
            {
                children[i].Destroy();
            }
            KGame.Instance.RemoveGameObject(this);
        }

    }
}
