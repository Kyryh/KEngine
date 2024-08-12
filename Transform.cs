using Microsoft.Xna.Framework;
using KEngine.Extensions;
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

        Matrix localMatrix;
        Matrix globalMatrix;

        bool localMatrixDirty = true;
        bool globalMatrixDirty = true;

        private void SetMatricesDirty(bool worldOnly) {
            localMatrixDirty |= worldOnly;
            foreach (var child in children)
            {
                child.SetMatricesDirty(true);
            }
        }

        Matrix LocalMatrix {
            get {
                if (localMatrixDirty) {
                    localMatrix = Matrix.Identity
                        * Matrix.CreateScale(Scale.ToVector3())
                        * Matrix.CreateRotationZ(-Rotation)
                        * Matrix.CreateTranslation(Position.ToVector3())
                        ;
                }
                return localMatrix;
            }
        }

        Matrix GlobalMatrix {
            get {
                if (globalMatrixDirty) {
                    if (Parent != null)
                        globalMatrix = LocalMatrix * Parent.GlobalMatrix;
                    else
                        globalMatrix = LocalMatrix;
                }
                return globalMatrix;
            }
        }
        public Vector2 Position {
            get {
                return position;
            }
            set {
                SetMatricesDirty(false);
                position = value;
            }
        }

        public float Rotation {
            get {
                return rotation;
            }
            set {
                SetMatricesDirty(false);
                rotation = value;
            }
        }

        public Vector2 Scale {
            get {
                return scale;
            }
            set {
                SetMatricesDirty(false);
                scale = value;
            }
        }

        public Vector2 GlobalPosition {
            get {
                return GlobalMatrix.Translation.ToVector2();
            }
        }

        public float GlobalRotation {
            get {
                return (float)Math.Atan2(GlobalMatrix.M21, GlobalMatrix.M11);
            }
        }

        public Vector2 GlobalScale {
            get {
                GlobalMatrix.Decompose(out var scale, out var _, out var _);
                return scale.ToVector2();
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
