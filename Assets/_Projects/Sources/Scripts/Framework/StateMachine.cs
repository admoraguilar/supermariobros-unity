using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


[Serializable]
public class StateMachine<T> {
    public State<T> CurrentState { get { return states.Count > 0 ? states.Peek() as State<T> : null; } }
    public State<T>[] States { get { return states.ToArray(); } }
    public int StateCount { get { return states.Count; } }

    private Stack<State<T>> states = new Stack<State<T>>();
    private T owner;


    public void SetState(State<T> State) {
        if(CurrentState == State) return;

        while(states.Count > 0) {
            PopState();
        }

        PushState(State);
    }

    public void Init(T owner) {
        this.owner = owner;
    }

    public void PushState(State<T> State) {
        if(CurrentState == State) return;

        if(states.Count > 0) {
            CurrentState.OnExit(owner);
        }

        states.Push(State);

        State<T> temp = states.Peek();
        temp.OnEnter(owner);
    }

    public State<T> PopState() {
        State<T> state = null;

        if(states.Count > 0) {
            CurrentState.OnExit(owner);
            state = states.Pop();
        }

        if(states.Count > 0) {
            CurrentState.OnEnter(owner);
        }

        return state;
    }

    public void Update() {
        if(CurrentState != null) {
            CurrentState.OnUpdate(owner);
        }
    }

    public void FixedUpdate() {
        if(CurrentState != null) {
            CurrentState.OnFixedUpdate(owner);
        }
    }

    public void LateUpdate() {
        if(CurrentState != null) {
            CurrentState.OnLateUpdate(owner);
        }
    }
}

[Serializable]
public abstract class State<T> {
    public abstract void OnEnter(T owner);
    public abstract void OnExit(T owner);

    public virtual void OnUpdate(T owner) { }
    public virtual void OnFixedUpdate(T owner) { }
    public virtual void OnLateUpdate(T owner) { }
}
