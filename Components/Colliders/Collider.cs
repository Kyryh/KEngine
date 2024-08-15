using KEngine.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static KEngine.Components.Colliders.Collider;

namespace KEngine.Components.Colliders {
    public abstract class Collider : Component {
        public bool Static { get; init; }
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

        public static bool CheckCollision(Collider colA, Collider colB, out HitInfo hitInfo, SpriteBatch spriteBatch = null) {
            // SAT (Separating Axis Theorem) algorithm
            // https://www.sevenson.com.au/programming/sat/

            if (colA.Static && colB.Static) {
                hitInfo = default;
                return false;
            }

            if (colB is CircleCollider)
                (colA, colB) = (colB, colA);

            if (colA is CircleCollider circleA) {
                if (colB is CircleCollider circleB) {
                    return CircleOnCircleCollision(circleA, circleB, out hitInfo);
                }
                return CircleOnPolygonCollision(circleA, colB, out hitInfo, spriteBatch);
            }

            return PolygonOnPolygonCollision(colA, colB, out hitInfo);
        }

        private static bool PolygonOnPolygonCollision(Collider colA, Collider colB, out HitInfo hitInfo) {
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
            for (int i = 0; i < axes.Length; i++) {
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
                if (distanceAlongAxis > 0) {
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

        private static bool CircleOnPolygonCollision(CircleCollider circle, Collider col, out HitInfo hitInfo, SpriteBatch spriteBatch = null) {
            hitInfo = default;

            hitInfo.AContainsB = true;
            hitInfo.BContainsA = true;


            var axes = col.Axes;

            Array.Resize(ref axes, axes.Length + 1);

            float sqrDistanceToNearestVertex = float.PositiveInfinity;
            var center = circle.Center;

            // Finds the nearest vertex to the circle
            foreach (var vertex in col.Vertices) {
                var currentAxis = vertex - center;
                var distance = currentAxis.LengthSquared();
                if (distance < sqrDistanceToNearestVertex) {
                    // Stores the direction to the nearest vertex
                    // And uses that as an axis
                    sqrDistanceToNearestVertex = distance;
                    axes[^1] = currentAxis;
                }
            }

            // Normalizes the new axis since it's still a distance
            Vector2.Normalize(ref axes[^1], out axes[^1]);

            // Stored encountered axes in a set
            HashSet<Vector2> axesEncountered = new();

            // Default value for hitinfo distance
            hitInfo.distance = float.NegativeInfinity;

            // Loops over all of the axes
            for (int i = 0; i < axes.Length; i++) {
                // If it was already encountered, skip it
                if (!axesEncountered.Add(axes[i]))
                    continue;

                Vector2.Dot(ref center, ref axes[i], out var centerDistance);

                // Calculates circle's projection by offsetting its center left and right
                float minA = centerDistance - circle.ActualRadius;
                float maxA = centerDistance + circle.ActualRadius;

                float minB = float.PositiveInfinity;
                float maxB = float.NegativeInfinity;

                // Same thing as before, but with the second collider
                for (int j = 0; j < col.Vertices.Length; j++) {
                    Vector2.Dot(ref col.Vertices[j], ref axes[i], out float distance);
                    if (distance < minB)
                        minB = distance;
                    if (distance > maxB)
                        maxB = distance;
                }

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
                if (distanceAlongAxis > 0) {
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

            hitInfo.colliderA = circle;
            hitInfo.colliderB = col;
            return true;
        }
        private static bool CircleOnCircleCollision(CircleCollider circleA, CircleCollider circleB, out HitInfo hitInfo) {
            var radiusSum = circleA.ActualRadius + circleB.ActualRadius;
            hitInfo = default;
            var x = circleA.Transform.GlobalPosition.X - circleB.Transform.GlobalPosition.X;
            var y = circleA.Transform.GlobalPosition.Y - circleB.Transform.GlobalPosition.Y;
            var distanceSqr = x * x + y * y;
            if (distanceSqr > radiusSum * radiusSum) {
                return false;
            }


            hitInfo.colliderA = circleA;
            hitInfo.colliderB = circleB;

            hitInfo.direction = circleB.Transform.GlobalPosition - circleA.Transform.GlobalPosition;


            Vector2.Normalize(ref hitInfo.direction, out hitInfo.direction);

            var distance = MathF.Sqrt(distanceSqr);


            hitInfo.AContainsB = circleB.ActualRadius <= circleA.ActualRadius && distance <= circleB.ActualRadius - circleA.ActualRadius;
            hitInfo.BContainsA = circleA.ActualRadius <= circleB.ActualRadius && distance <= circleA.ActualRadius - circleB.ActualRadius;

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
