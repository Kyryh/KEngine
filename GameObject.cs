using System;
using System.Collections.Generic;
using System.Linq;
using KEngine.Components;
using Microsoft.Xna.Framework;

namespace KEngine
{
    public class GameObject {

        public string Name { get; init; }

        public bool active;
        public bool IsActive {
            get {
                if (Transform.Parent == null)
                    return active;
                return active && Transform.Parent.GameObject.IsActive;
            }
        }

        public Transform Transform { get; private set; }

        public readonly Component[] components = Array.Empty<Component>();

        private bool loaded = false;

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

            Transform[] transformChildren;
            if (children != null) {
                transformChildren = new Transform[children.Length];
                for (int i = 0; i < children.Length; i++)
                {
                    transformChildren[i] = children[i].Transform;
                }
            } else {
                transformChildren = Array.Empty<Transform>();
            }

            Transform = new(
                this,
                position ?? Vector2.Zero,
                rotation,
                scale ?? Vector2.One,
                transformChildren
            );

            this.active = active;

            if (components != null) {
                this.components = new Component[components.Length];
                for (int i = 0; i < components.Length; i++) {
                    this.components[i] = components[i];
                    this.components[i].GameObject = this;
                }
            }

        }

        public void Load() {
            if (loaded)
                throw new InvalidOperationException($"GameObject {Name} already loaded");
            loaded = true;
            KGame.Instance.LoadGameObject(this);
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
                for (int i = 0; i < Transform.children.Length; i++) {
                    if (Transform.children[i].GameObject.TryGetComponent<T>(out var component, activeOnly, true))
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
                for (int i = 0; i < Transform.children.Length; i++) {
                    result.AddRange(Transform.children[i].GameObject.GetComponents<T>(activeOnly, true));
                }
            }
            return result;
        }

        public void Destroy() {
            for (int i = 0; i < components.Length; i++) {
                components[i].OnDisable();
                components[i].OnDestroy();
                KGame.Instance.RemoveComponent(components[i]);
            }
            for (int i = 0; i < Transform.children.Length; i++)
            {
                Transform.children[i].GameObject.Destroy();
            }
            KGame.Instance.RemoveGameObject(this);
        }

    }
}
