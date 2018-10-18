using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


namespace WishfulDroplet {
    using Extensions;

    [Serializable]
    public class StateController<T> {
        private Dictionary<string, StateMachine<T>> stateMachines = new Dictionary<string, StateMachine<T>>();


        public StateMachine<T> GetStateMachine(string id) {
            StateMachine<T> stateMachine = null;
            if(!stateMachines.TryGetValue(id, out stateMachine)) {
                Debug.Log(string.Format("StateMachine: {0}, doesn't exists.", id));
            }
            return stateMachine;
        }

        public StateMachine<T> AddStateMachine(StateMachine<T> stateMachine, IState<T> firstState = null) {
            return AddStateMachine(stateMachine, default(T), firstState);
        }

        public StateMachine<T> AddStateMachine(StateMachine<T> stateMachine, T owner, IState<T> firstState = null) {
            stateMachines[stateMachine.id] = stateMachine;
            stateMachine.SetOwner(owner == null ? stateMachine.owner : owner);
            stateMachine.SetState(firstState);
            return stateMachine;
        }

        public void RemoveStateMachine(string id) {
            StateMachine<T> stateMachine = GetStateMachine(id);
            if(stateMachine != null) {
                stateMachine.SetState(null);
            }
            stateMachines.Remove(id);
        }

        public void Update() {
            foreach(var stateMachine in stateMachines) {
                stateMachine.Value.Update();
            }
        }

        public void FixedUpdate() {
            foreach(var stateMachine in stateMachines) {
                stateMachine.Value.FixedUpdate();
            }
        }

        public void LateUpdate() {
            foreach(var stateMachine in stateMachines) {
                stateMachine.Value.LateUpdate();
            }
        }
    }


    [Serializable]
    public class StateMachine<T> {
        public T owner { get; protected set; }

        public IState<T> currentState { get { return stateStack.Count > 0 ? stateStack.Peek() as IState<T> : null; } }
        public IState<T>[] states { get { return stateStack.ToArray(); } }        
        public int stateCount { get { return stateStack.Count; } }

        public string id;

        private Stack<IState<T>> stateStack = new Stack<IState<T>>();


        public StateMachine(string id) {
            this.id = id;
        }

        public void SetState(IState<T> state) {
            if(currentState == state) return;

            while(stateStack.Count > 0) {
                PopState();
            }

            PushState(state);
        }

        public void SetOwner(T owner) {
            this.owner = owner;
        }

        public void PushState(IState<T> state) {
            if(currentState == state) return;

            if(stateStack.Count > 0) {
                currentState.DoExit(owner);
            }

            stateStack.Push(state);

            IState<T> temp = stateStack.Peek();
            temp.DoEnter(owner);
        }

        public IState<T> PopState() {
            IState<T> state = null;

            if(stateStack.Count > 0) {
                currentState.DoExit(owner);
                state = stateStack.Pop();
            }

            if(stateStack.Count > 0) {
                currentState.DoEnter(owner);
            }

            return state;
        }

        public void Update() {
            if(currentState != null) {
                currentState.DoUpdate(owner);
            }
        }

        public void FixedUpdate() {
            if(currentState != null) {
                currentState.DoFixedUpdate(owner);
            }
        }

        public void LateUpdate() {
            if(currentState != null) {
                currentState.DoLateUpdate(owner);
            }
        }
    }


    public interface IState<T> {
        void DoEnter(T owner);
        void DoExit(T owner);

        void DoUpdate(T owner);
        void DoFixedUpdate(T owner);
        void DoLateUpdate(T owner);
    }
}