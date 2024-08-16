using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace KEngine.Components.Colliders {
    public abstract class Collider : Component {
        public delegate void OnCollisionHandler(Collider other);
        public event OnCollisionHandler OnCollision;
        public bool IsStatic { get; init; }
        public bool IsTrigger { get; init; }
        public abstract Vector2[] Vertices { get; }
        public abstract void Axes(Collider other, out Vector2[] axes);
        public override void Initialize() {
            base.Initialize();
            KGame.Instance.AddCollider(this);
        }

        public override void OnDestroy() {
            base.OnDestroy();
            KGame.Instance.RemoveCollider(this);
        }

        public static void CheckCollision(Collider col, out List<HitInfo> hitInfoList) {
            hitInfoList = new();
            for (int i = 0; i < KGame.Instance.colliders.Count; i++) {
                if (!KGame.Instance.colliders[i].IsActive) continue;
                if (CheckCollision(col, KGame.Instance.colliders[i], out var hitInfo)) {
                    hitInfoList.Add(hitInfo);
                }
            }
        }
        public static bool CheckCollision(Collider colA, Collider colB, out HitInfo hitInfo) {

            if (colA.IsStatic && colB.IsStatic) {
                hitInfo = default;
                return false;
            }

            return SATCollision(colA, colB, out hitInfo);
        }

        private static bool SATCollision(Collider colA, Collider colB, out HitInfo hitInfo) {
            // SAT (Separating Axis Theorem) algorithm
            // https://www.sevenson.com.au/programming/sat/

            hitInfo = default;

            hitInfo.AContainsB = true;
            hitInfo.BContainsA = true;

            colA.Axes(colB, out var axesA);
            colB.Axes(colA, out var axesB);

            // Gets all of the axes from colliderA and colliderB combined
            var axes = new Vector2[axesA.Length + axesB.Length];
            Array.Copy(axesA, 0, axes, 0, axesA.Length);
            Array.Copy(axesB, 0, axes, axesA.Length, axesB.Length);

            // Stored encountered axes in a set
            HashSet<Vector2> axesEncountered = new();

            // Default value for hitinfo distance
            hitInfo.distance = float.NegativeInfinity;

            // Loops over all of the axes
            for (int i = 0; i < axes.Length; i++) {
                // If it was already encountered, skip it
                if (!axesEncountered.Add(axes[i]))
                    continue;

                var (minA, maxA) = colA.ProjectToAxis(ref axes[i]);

                var (minB, maxB) = colB.ProjectToAxis(ref axes[i]);

                

                var rightOverlap = minA - maxB;
                var leftOverlap = minB - maxA;

                bool flipDirection;
                float distanceAlongAxis;

                // Checks if the vertex projections overlap
                if (rightOverlap > leftOverlap) {
                    distanceAlongAxis = rightOverlap;
                    flipDirection = false;
                }
                else {
                    distanceAlongAxis = leftOverlap;
                    flipDirection = true;
                }

                // If they don't, the polygons aren't touching
                if (distanceAlongAxis >= 0) {
                    return false;
                }

                // Check how much the vertex projections are overlapping
                // And stores the smallest overlap with its distance
                if (distanceAlongAxis > hitInfo.distance) {
                    hitInfo.distance = distanceAlongAxis;
                    hitInfo.direction = axes[i];
                    if (flipDirection)
                        hitInfo.direction *= -1;
                }

                // Check if the vertex projection ranges contain each other
                // If they don't, the polygons are certain do not be contained by each other
                if (maxA < maxB || minA > minB) hitInfo.AContainsB = false;
                if (maxB < maxA || minB > minA) hitInfo.BContainsA = false;
            }

            hitInfo.colliderA = colA;
            hitInfo.colliderB = colB;
            return true;
        }

        protected virtual (float, float) ProjectToAxis(ref Vector2 axis) {
            float min = float.PositiveInfinity;
            float max = float.NegativeInfinity;

            for (int j = 0; j < Vertices.Length; j++) {
                Vector2.Dot(ref Vertices[j], ref axis, out float distance);
                if (distance < min)
                    min = distance;
                if (distance > max)
                    max = distance;
            }

            return (min, max);
        }

        internal void CallOnCollision(Collider other) {
            OnCollision?.Invoke(other);
        }

        public virtual void DebugDraw(SpriteBatch spriteBatch) {

        }

        public struct HitInfo {
            public Collider colliderA;
            public Collider colliderB;
            public float distance;
            public Vector2 direction;
            public bool AContainsB;
            public bool BContainsA;

            public bool AInsideB {
                readonly get {
                    return BContainsA;
                }
                set {
                    BContainsA = value;
                }
            }

            public bool BInsideA {
                readonly get {
                    return AContainsB;
                }
                set {
                    AContainsB = value;
                }
            }
            public override readonly string ToString() {
                return new StringBuilder("HitInfo(\n")
                    .Append("    Distance: ")
                    .Append(distance)
                    .Append("\n    Direction: ")
                    .Append(direction)
                    .Append("\n    A inside B: ")
                    .Append(AInsideB)
                    .Append("\n    B inside A: ")
                    .Append(BInsideA)
                    .Append("\n)")
                    .ToString();
            }
        }
    }
}
