using KEngine.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEngine.Components.Colliders {
    public class CircleCollider : Collider {
        public float Radius { get; init; } = 0.5f;
        public Vector2 Offset { get; init; }

        private bool recalculateCenter = true;
        private bool recalculateRadius = true;
        private Vector2 center;
        public Vector2 Center {
            get {
                if (recalculateCenter) {
                    var offset = Offset;
                    var globalMatrix = Transform.GlobalMatrix;
                    Vector2.Transform(ref offset, ref globalMatrix, out center);
                }
                return center;
            }
        }
        private float actualRadius;
        public float ActualRadius {
            get {
                if (recalculateRadius) {
                    var scale = Transform.GlobalScale;
                    actualRadius = Radius * (scale.X+scale.Y)/2;
                }
                return actualRadius;
            }
        }
        public override Vector2[] Vertices => null;

        public override void Axes(Collider other, out Vector2[] axes) {
            axes = new Vector2[1];

            float sqrDistanceToNearestVertex = float.PositiveInfinity;
            var center = Center;

            // Finds the nearest vertex to the circle
            foreach (var vertex in other.Vertices) {
                var currentAxis = vertex - center;
                var distance = currentAxis.LengthSquared();
                if (distance < sqrDistanceToNearestVertex) {
                    // Stores the direction to the nearest vertex
                    // And uses that as an axis
                    sqrDistanceToNearestVertex = distance;
                    axes[0] = currentAxis;
                }
            }

            // Normalizes the new axis since it's still a distance
            Vector2.Normalize(ref axes[0], out axes[0]);
        }

        public override void Initialize() {
            base.Initialize();
            Transform.OnPositionChanged += (_, _) => { recalculateCenter = true; };
            Transform.OnRotationChanged += (_, _) => { recalculateCenter = true; };
            Transform.OnScaleChanged += (_, _) => {
                recalculateCenter = true;
                recalculateRadius = true;
            };
        }

        public bool Contains(Vector2 point) {
            return (Center - point).LengthSquared() <= ActualRadius*ActualRadius;
        }

        protected override (float, float) ProjectToAxis(ref Vector2 axis) {

            var center = Center;

            Vector2.Dot(ref center, ref axis, out var centerDistance);

            // Calculates circle's projection by offsetting its center left and right
            float min = centerDistance - ActualRadius;
            float max = centerDistance + ActualRadius;

            return (min, max);
        }

        public override void DebugDraw(SpriteBatch spriteBatch) {
            base.DebugDraw(spriteBatch);
            List<Vector2> points = new();
            for (float i = 0; i < 2; i += 0.1f) {
                points.Add(Vector2.Transform(GameConstants.Vector2.Up * ActualRadius, Matrix.CreateRotationZ(i * MathF.PI)));
            }
            for (int i = 0; i < points.Count-1; i++) {
                spriteBatch.DrawLine(Camera.MainCamera.WorldToScreen(points[i]+Center), Camera.MainCamera.WorldToScreen(points[i + 1]+Center), color: Color.LightGreen);
            }
            spriteBatch.DrawLine(Camera.MainCamera.WorldToScreen(points[^1] + Center), Camera.MainCamera.WorldToScreen(points[0] + Center), color: Color.LightGreen);

        }
    }
}
