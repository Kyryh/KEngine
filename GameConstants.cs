using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEngine {
    public static class GameConstants {
        public static Vector2Constants Vector2 { get; } = new();
    }
    public class Vector2Constants {
        Vector2 right = new(1, 0);
        Vector2 left = new(-1, 0);
        Vector2 up = new(0, 1);
        Vector2 down = new(0, -1);

        public Vector2 Right => right;
        public Vector2 Left => left;
        public Vector2 Up => up;
        public Vector2 Down => down;
    }
}
