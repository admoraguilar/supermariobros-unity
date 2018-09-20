using UnityEngine;
using System;
using System.Collections.Generic;


public class EnumStack<T> where T : struct, IComparable, IFormattable, IConvertible {
    public event Action<T> OnEnter = delegate { };
    public event Action<T> OnStay = delegate { };
    public event Action<T> OnExit = delegate { };

    public int Count { get { return enums.Count; } }

    private Stack<T> enums = new Stack<T>();


    public T Peek() {
        return enums.Count > 0 ? enums.Peek() : default(T);
    }

    public T[] GetValues() {
        return enums.Count > 0 ? enums.ToArray() : default(T[]);
    }

    public bool IsEnum(T @enum) {
        return enums.Count > 0 ? enums.Peek().Equals(@enum) : false;
    }

    public void SetEnum(T @enum) {
        if(enums.Peek().Equals(@enum)) return;

        while(enums.Count > 0) {
            PopEnum();
        }

        PushEnum(@enum);
    }

    public void PushEnum(T @enum) {
        if(enums.Peek().Equals(@enum)) return;

        if(enums.Count > 0) {
            OnExit(enums.Peek());
        }

        enums.Push(@enum);
        OnEnter(@enum);
    }

    public T PopEnum() {
        if(enums.Count <= 0) return default(T);

        T @enum = default(T);

        if(enums.Count > 0) {
            OnExit(enums.Pop());
        }

        if(enums.Count > 0) {
            OnEnter(enums.Peek());
        }

        return @enum;
    }

    public void Update() {
        if(enums.Count > 0) {
            OnStay(enums.Peek());
        }
    }
}
