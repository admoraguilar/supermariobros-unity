using UnityEngine;


namespace WishfulDroplet {
    namespace Extensions {
        public static class ComponentExtensions {
            public static bool HasComponent<T>(this Component component) {
                return component.GetComponentsInChildren<T>(true) != null;
            }
        }
    }
}

