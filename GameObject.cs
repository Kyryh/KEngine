using System;
using System.Collections.Generic;
using KEngine.Components;

namespace KEngine
{
    public class GameObject {

        public string Name { get; init; }
        public GameObject parent;
        public List<GameObject> children;
        public Transform Transform { get; }
        private readonly List<Component> components = new();



    }
}
