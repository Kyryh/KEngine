using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEngine {
    public class Transform {

        public GameObject GameObject { get; private set; }

        private Vector2 position;
        private float rotation;
        private Vector2 scale;

        public Vector2 Position {
            get {
                return position;
            }
            set {
                position = value;
            }
        }

        public float Rotation {
            get {
                return rotation;
            }
            set {
                rotation = value;
            }
        }

        public Vector2 Scale {
            get {
                return scale;
            }
            set {
                scale = value;
            }
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
                return Parent.GlobalScale * scale;
            }
        }

        public Transform Parent { get; private set; }
        public readonly Transform[] children;

        public Transform(GameObject gameObject, Vector2 position, float rotation, Vector2 scale, Transform[] children) {
            GameObject = gameObject;
            this.position = position;
            this.rotation = rotation;
            this.scale = scale;
            this.children = new Transform[children.Length];
            for (int i = 0; i < children.Length; i++) {
                this.children[i] = children[i];
                this.children[i].Parent = this;
            }
        }
    }
}
