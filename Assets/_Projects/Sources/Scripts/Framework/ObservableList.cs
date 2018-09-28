using UnityEngine;
using System;
using System.Collections.Generic;


public class ObservableList<T> : List<T> {
    public event Action<T> OnAdd = delegate { };
    public event Action<T> OnRemove = delegate { };


    public new void Add(T item) {
        OnAdd(item);
        base.Add(item);
    }

    public new void AddRange(IEnumerable<T> collection) {
        foreach(var element in collection) {
            OnAdd(element);
        }
        base.AddRange(collection);
    }

    public new void Remove(T item) {
        OnRemove(item);
        base.Remove(item);
    }

    public new void Clear() {
        for(int i = 0; i < Count; i++) {
            OnRemove(this[i]);
        }

        base.Clear();
    }
}
