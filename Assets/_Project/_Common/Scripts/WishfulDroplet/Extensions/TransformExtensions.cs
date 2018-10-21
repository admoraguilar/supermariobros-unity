using UnityEngine;


namespace WishfulDroplet {
    namespace Extensions {
        public static class TransformExtensions {
            public static Transform FindDeepChild(this Transform transform, string path, SearchType type) {
                switch(type) {
                    case SearchType.BreadthFirst: {
                            var result = transform.Find(path);
                            if(result != null) return result;
                            foreach(Transform child in transform) {
                                result = child.FindDeepChild(path, SearchType.BreadthFirst);
                                if(result != null) return result;
                            }
                            return null;
                        }
                    case SearchType.DepthFirst: {
                            foreach(Transform child in transform) {
                                if(child.name == path) return child;
                                var result = child.FindDeepChild(path, SearchType.DepthFirst);
                                if(result != null) return result;
                            }
                            return null;
                        }
                    default: return null;
                }
            }


            public enum SearchType {
                BreadthFirst,
                DepthFirst
            }
        }
    }
}

