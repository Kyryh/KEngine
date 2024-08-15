using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEngine.Extensions {
    public static class Vector2Extensions {
        public static Vector3 ToVector3(this Vector2 v, float z = 0) {
            return new Vector3(v.X, v.Y, z);
        }

        public static float GetRotation(this Vector2 v) {
            return (float)Math.Atan2(v.Y, v.X);
        }

        public static Vector2 Normalized(this Vector2 v) {
            if (v == Vector2.Zero)
                return v;
            v.Normalize();
            return v;
        }
    }
}
