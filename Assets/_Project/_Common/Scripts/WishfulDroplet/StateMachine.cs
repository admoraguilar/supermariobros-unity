using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


namespace WishfulDroplet {
    using Extensions;

    //[Serializable]
    //public class StateController<TOwner, TState>
    //    where TState : State<TOwner> {
    //    private Dictionary<string, StateMachine<TOwner, TState>> stateMachines = new Dictionary<string, StateMachine<TOwner, TState>>();


    //    public StateMachine<TOwner, TState> GetStateMachine(string id) {
    //        StateMachine<TOwner, TState> stateMachine = null;
    //        if(!stateMachines.TryGetValue(id, out stateMachine)) {
    //            Debug.Log(string.Format("StateMachine: {0}, doesn't exists.", id));
    //        }
    //        return stateMachine;
    //    }

    //    public StateMachine<TOwner, TState> AddStateMachine(StateMachine<TOwner, TState> stateMachine, TState firstState = null) {
    //        return AddStateMachine(stateMachine, default(TOwner), firstState);
    //    }

    //    public StateMachine<TOwner, TState> AddStateMachine(StateMachine<TOwner, TState> stateMachine, TOwner owner, TState firstState = null) {
    //        stateMachines[stateMachine.id] = stateMachine;
    //        stateMachine.SetOwner(owner == null ? stateMachine.owner : owner);
    //        stateMachine.SetState(firstState);
    //        return stateMachine;
    //    }

    //    public void RemoveStateMachine(string id) {
    //        StateMachine<TOwner, TState> stateMachine = GetStateMachine(id);
    //        if(stateMachine != null) {
    //            stateMachine.SetState(null);
    //        }
    //        stateMachines.Remove(id);
    //    }

    //    public void Update() {
    //        foreach(var stateMachine in stateMachines) {
    //            stateMachine.Value.Update();
    //        }
    //    }

    //    public void FixedUpdate() {
    //        foreach(var stateMachine in stateMachines) {
    //            stateMachine.Value.FixedUpdate();
    //        }
    //    }

    //    public void LateUpdate() {
    //        foreach(var stateMachine in stateMachines) {
    //            stateMachine.Value.LateUpdate();
    //        }
    //    }
    //}


    [Serializable]
    public class StateController {
        private Dictionary<string, _InternalStateMachine> stateMachines = new Dictionary<string, _InternalStateMachine>();


        public StateMachine<TOwner, TState> GetStateMachine<TOwner, TState>(string id)
            where TState : State<TOwner> {
            return (StateMachine<TOwner, TState>)GetStateMachine(id);
        }

        public _InternalStateMachine GetStateMachine(string id) {
            _InternalStateMachine stateMachine = null;
            if(!stateMachines.TryGetValue(id, out stateMachine)) {
                Debug.Log(string.Format("StateMachine: {0}, doesn't exists.", id));
            }
            return stateMachine;
        }

        public StateMachine<TOwner, TState> AddStateMachine<TOwner, TState>(StateMachine<TOwner, TState> stateMachine, TState firstState = null)
            where TState : State<TOwner> {
            return AddStateMachine(stateMachine, default(TOwner), firstState);
        }

        public StateMachine<TOwner, TState> AddStateMachine<TOwner, TState>(StateMachine<TOwner, TState> stateMachine, TOwner owner, TState firstState = null)
            where TState : State<TOwner> {
            stateMachines[stateMachine.id] = stateMachine;
            stateMachine.SetOwner(owner == null ? stateMachine.owner : owner);
            stateMachine.SetState(firstState);
            return stateMachine;
        }

        public void RemoveStateMachine(string id) {
            _InternalStateMachine stateMachine = GetStateMachine(id);
            if(stateMachine != null) {
                stateMachine.Flush();
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
    public class StateMachine<TOwner, TState> : _InternalStateMachine
        where TState : State<TOwner> {
        public TOwner owner { get; protected set; }

        public TState currentState { get { return stateStack.Count > 0 ? stateStack.Peek() as TState : null; } }
        public TState[] states { get { return stateStack.ToArray(); } }        
        public int stateCount { get { return stateStack.Count; } }

        private Stack<TState> stateStack = new Stack<TState>();


        public StateMachine(string id) : base(id) {
            
        }

        public void SetState(TState state) {
            if(currentState == state) return;

            while(stateStack.Count > 0) {
                PopState();
            }

            PushState(state);
        }

        public void SetOwner(TOwner owner) {
            this.owner = owner;
        }

        public void PushState(TState state) {
            if(currentState == state) return;

            if(stateStack.Count > 0) {
                currentState.DoExit(owner);
            }

            stateStack.Push(state);

            TState temp = stateStack.Peek();
            temp.DoEnter(owner);
        }

        public TState PopState() {
            TState state = null;

            if(stateStack.Count > 0) {
                currentState.DoExit(owner);
                state = stateStack.Pop();
            }

            if(stateStack.Count > 0) {
                currentState.DoEnter(owner);
            }

            return state;
        }

        public override void Flush() {
            SetState(null);
        }

        public override void Update() {
            if(currentState != null) {
                currentState.DoUpdate(owner);
            }
        }

        public override void FixedUpdate() {
            if(currentState != null) {
                currentState.DoFixedUpdate(owner);
            }
        }

        public override void LateUpdate() {
            if(currentState != null) {
                currentState.DoLateUpdate(owner);
            }
        }
    }


    public abstract class State<TOwner> : _InternalState {
        public abstract void DoEnter(TOwner owner);
        public abstract void DoExit(TOwner owner);

        public virtual void DoUpdate(TOwner owner) { }
        public virtual void DoFixedUpdate(TOwner owner) { }
        public virtual void DoLateUpdate(TOwner owner) { }
    }


    public abstract class _InternalStateMachine {
        public string id;


        public _InternalStateMachine(string id) {
            this.id = id;
        }

        public abstract void Flush();

        public abstract void Update();
        public abstract void FixedUpdate();
        public abstract void LateUpdate();
    }


    public abstract class _InternalState { }
}