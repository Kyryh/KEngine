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

        public override Vector2[] Axes => null;

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
