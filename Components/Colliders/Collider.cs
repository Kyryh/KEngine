using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEngine.Components.Colliders {
    public abstract class Collider : Component {
        public abstract Vector2[] Vertices { get; }
        public abstract Vector2[] Axes { get; }
        public override void Initialize() {
            base.Initialize();
            KGame.Instance.AddCollider(this);
        }

        public override void OnDestroy() {
            base.OnDestroy();
            KGame.Instance.RemoveCollider(this);
        }

        public static bool CheckCollision(Collider colA, Collider colB, out HitInfo hitInfo) {
            // SAT (Separating Axis Theorem) algorithm
            // https://www.sevenson.com.au/programming/sat/

            hitInfo = default;
            hitInfo.AContainsB = true;
            hitInfo.BContainsA = true;

            var axesA = colA.Axes;
            var axesB = colB.Axes;

            // Gets all of the axes from colliderA and colliderB combined
            var axes = new Vector2[axesA.Length + axesB.Length];
            Array.Copy(axesA, 0, axes, 0, axesA.Length);
            Array.Copy(axesB, 0, axes, axesA.Length, axesB.Length);

            // Stored encountered axes in a set
            HashSet<Vector2> axesEncountered = new();

            // Default value for hitinfo distance
            hitInfo.distance = float.NegativeInfinity;

            // Loops over all of the axes
            for(int i = 0; i < axes.Length; i++) {
                // If it was already encountered, skip it
                if (!axesEncountered.Add(axes[i]))
                    continue;

                float minA = float.PositiveInfinity;
                float maxA = float.NegativeInfinity;

                // Projects each vertex of the first collider onto the axis
                // And stores the left-most and the right-most ones
                for (int j = 0; j < colA.Vertices.Length; j++) {
                    Vector2.Dot(ref colA.Vertices[j], ref axes[i], out float distance);
                    if (distance < minA)
                        minA = distance;
                    if (distance > maxA)
                        maxA = distance;
                }

                float minB = float.PositiveInfinity;
                float maxB = float.NegativeInfinity;

                // Same thing as before, but with the second collider
                for (int j = 0; j < colB.Vertices.Length; j++) {
                    Vector2.Dot(ref colB.Vertices[j], ref axes[i], out float distance);
                    if (distance < minB)
                        minB = distance;
                    if (distance > maxB)
                        maxB = distance;
                }

                // Checks if the vertex projections overlap
                var distanceAlongAxis = MathF.Max(minA - maxB, minB - maxA);

                // If they don't, the polygons aren't touching
                if (distanceAlongAxis > 0) {
                    return false;
                }

                // Check how much the vertex projections are overlapping
                // And stores the smallest overlap with its distance
                if (distanceAlongAxis > hitInfo.distance) {
                    hitInfo.distance = distanceAlongAxis;
                    hitInfo.direction = axes[i];
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
