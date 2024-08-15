using Microsoft.Xna.Framework;
using KEngine.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEngine {
    public class Transform {

        public delegate void OnPositionChangedHandler(Vector2 oldPosition, Vector2 newPosition);
        public delegate void OnRotationChangedHandler(float oldRotation, float newRotation);
        public delegate void OnScaleChangedHandler(Vector2 oldScale, Vector2 newScale);
        public GameObject GameObject { get; private set; }

        private Vector2 position;
        private float rotation;
        private Vector2 scale;

        public event OnPositionChangedHandler OnPositionChanged;
        public event OnRotationChangedHandler OnRotationChanged;
        public event OnScaleChangedHandler OnScaleChanged;

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

        internal Matrix LocalMatrix {
            get {
                if (localMatrixDirty) {
                    var scaleMatrix = Matrix.CreateScale(Scale.ToVector3());
                    var rotationMatrix = Matrix.CreateRotationZ(-Rotation);
                    var translationMatrix = Matrix.CreateTranslation(Position.ToVector3());
                    Matrix.Multiply(ref scaleMatrix, ref rotationMatrix, out localMatrix);
                    Matrix.Multiply(ref localMatrix, ref translationMatrix, out localMatrix);
                }
                return localMatrix;
            }
        }

        internal Matrix GlobalMatrix {
            get {
                if (globalMatrixDirty) {
                    if (Parent != null) {
                        var localMatrix = LocalMatrix;
                        var parentGlobalMatrix = Parent.GlobalMatrix;
                        Matrix.Multiply(ref localMatrix, ref parentGlobalMatrix, out globalMatrix);
                    } else {
                        globalMatrix = LocalMatrix;
                    }
                }
                return globalMatrix;
            }
        }
        public Vector2 Position {
            get {
                return position;
            }
            set {
                if (position == value)
                    return;
                var oldValue = position;
                SetMatricesDirty(false);
                position = value;
                OnPositionChanged?.Invoke(oldValue, value);
            }
        }

        public float Rotation {
            get {
                return rotation;
            }
            set {
                if (rotation == value)
                    return;
                var oldValue = rotation;
                SetMatricesDirty(false);
                rotation = value;
                OnRotationChanged?.Invoke(oldValue, value);
            }
        }

        public Vector2 Scale {
            get {
                return scale;
            }
            set {
                if (scale == value)
                    return;
                var oldValue = scale;
                SetMatricesDirty(false);
                scale = value;
                OnScaleChanged?.Invoke(oldValue, value);
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
