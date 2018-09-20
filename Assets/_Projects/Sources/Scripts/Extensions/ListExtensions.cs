using UnityEngine;
using System.Collections.Generic;


public static class ListExtensions {
    public static void AddToFront<T>(this List<T> list, T item) {
        if(list.Count > 0) {
            T front = list[0];
            list[0] = item;
            list.Add(front);
        } else {
            list.Add(item);
        }
    }
}
