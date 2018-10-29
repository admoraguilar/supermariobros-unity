using System.Collections.Generic;


namespace WishfulDroplet.Extensions {
    public static class ListExtensions {
        public static void AddToFront<T>(this List<T> list, T item) {
            list.Insert(0, item);
        }
    }
}