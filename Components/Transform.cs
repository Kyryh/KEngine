using Microsoft.Xna.Framework;


namespace KEngine.Components
{
    public class Transform : Component {

        public Vector2 position = Vector2.Zero;
        public float rotation = 0f;
        public Vector2 scale = Vector2.One;

        public Vector2 GlobalPosition {
            get {
                if (GameObject.parent == null)
                    return position;
                return GameObject.parent.Transform.GlobalPosition + position;
            }
        }

        public float GlobalRotation {
            get {
                if (GameObject.parent == null)
                    return rotation;
                return GameObject.parent.Transform.GlobalRotation + rotation;
            }
        }

        public Vector2 GlobalScale {
            get {
                if (GameObject.parent == null)
                    return scale;
                return GameObject.parent.Transform.GlobalScale + scale;
            }
        }
    }
}
