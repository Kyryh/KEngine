
namespace KEngine.Components {
    public abstract class Component {

        protected bool active;

        public bool Active {
            get {
                return active;
            }
            set {
                active = value;
                if (value)
                    OnEnable();
                else
                    OnDisable();
            }
        }

        protected readonly GameObject gameObject;
        protected Transform Transform => gameObject.Transform;

        public virtual int UpdatePriority => 0;

        protected Component()
        {
            KGame.Instance.AddComponent(this);
        }
        public virtual void Initialize() {

        }

        public virtual void Start() {

        }

        public virtual void OnEnable() {

        }

        public virtual void Update(double deltaTime) {

        }

        public virtual void OnDisable() {

        }

        public virtual void OnDestroy() {

        }

    }
}
