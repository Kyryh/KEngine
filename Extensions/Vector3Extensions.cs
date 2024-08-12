using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEngine.Extensions {
    public static class Vector3Extensions {
        public static Vector2 ToVector2(this Vector3 v) {
            return new Vector2(v.X, v.Y);
        }
    }
}
