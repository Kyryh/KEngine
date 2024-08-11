
using System;
using System.Collections.Generic;

namespace KEngine.Components {
    public abstract class Component {

        protected bool active;

        public bool Active {
            get {
                return active;
            }
            set {
                if (active != value) {
                    if (value)
                        OnEnable();
                    else
                        OnDisable();
                }
                active = value;
            }
        }

        public bool IsActive => Active && GameObject.IsActive;

        private GameObject gameObject;

        public GameObject GameObject {
            get {
                return gameObject;
            }
            set {
                if (gameObject is not null)
                    throw new InvalidOperationException("GameObject already set");
                gameObject = value;
            }
        }

        public virtual int UpdateGroup => 0;

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

        public virtual void Update(float deltaTime) {

        }

        public virtual void OnDisable() {

        }

        public virtual void OnDestroy() {
            KGame.Instance.RemoveComponent(this);
        }

    }
}
