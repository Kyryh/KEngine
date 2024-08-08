using Microsoft.Xna.Framework;


namespace KEngine.Components
{
    public class Transform : Component {

        public Vector2 position = Vector2.Zero;
        public float rotation = 0f;
        public Vector2 scale = Vector2.One;

        public Vector2 GlobalPosition {
            get {
                if (gameObject.parent == null)
                    return position;
                return gameObject.parent.Transform.GlobalPosition + position;
            }
        }

        public float GlobalRotation {
            get {
                if (gameObject.parent == null)
                    return rotation;
                return gameObject.parent.Transform.GlobalRotation + rotation;
            }
        }

        public Vector2 GlobalScale {
            get {
                if (gameObject.parent == null)
                    return scale;
                return gameObject.parent.Transform.GlobalScale + scale;
            }
        }
    }
}
