using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


namespace WishfulDroplet {
    using Extensions;
	

	[Serializable]
	public class StateMachineController<TIdType> {
		private Dictionary<TIdType, IStateMachine> stateMachines = new Dictionary<TIdType, IStateMachine>();


		public IStateMachine GetStateMachine(TIdType id) {
			IStateMachine stateMachine = null;
			if(!stateMachines.TryGetValue(id, out stateMachine)) {
				Debug.Log(string.Format("StateMachine: {0}, doesn't exists.", id));
			}
			return stateMachine;
		}

		public void AddStateMachine(object owner, TIdType id, IStateMachine stateMachine, IState firstState = null) {
			stateMachines[id] = stateMachine;
			stateMachine.SetOwner(owner);
			stateMachine.SetState(firstState);
		}

		public void RemoveStateMachine(TIdType id) {
			IStateMachine stateMachine = GetStateMachine(id);
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
	public class StateMachine<TOwner, TState> : IStateMachine
		where TState : State<TOwner> {
		public event Action OnPushState = delegate { };
		public event Action OnPopState = delegate { };

		public TOwner owner { get; protected set; }
		
		public TState currentState { get { return stateStack.Count > 0 ? stateStack.Peek() as TState : default(TState); } }
		public TState[] states { get { return stateStack.ToArray(); } }
		public int stateCount { get { return stateStack.Count; } }

		private Stack<TState> stateStack = new Stack<TState>();


		public void SetOwner(TOwner owner) {
			this.owner = owner;
		}

		public void PushState(TState state) {
			if(currentState == state) return;

			if(stateStack.Count > 0) {
				currentState.DoExit(owner);
			}

			stateStack.Push(state);
			OnPushState();

			TState temp = stateStack.Peek();
			temp.DoEnter(owner);
		}

		public TState PopState() {
			TState state = null;

			if(stateStack.Count > 0) {
				currentState.DoExit(owner);
				state = stateStack.Pop();
				OnPopState();
			}

			if(stateStack.Count > 0) {
				currentState.DoEnter(owner);
			}

			return state;
		}

		public void SetState(TState state) {
			if(currentState == state) return;

			while(stateStack.Count > 0) {
				PopState();
			}

			PushState(state);
		}

		public void Flush() {
			SetState(null);
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


		event Action IStateMachine.OnPushState {
			add { OnPushState += value; }
			remove { OnPushState -= value; }
		}

		event Action IStateMachine.OnPopState {
			add { OnPopState += value; }
			remove { OnPopState -= value; }
		}

		object IStateMachine.owner {
			get { return owner; }
			set { owner = (TOwner)value; }
		}

		IState IStateMachine.currentState {
			get { return currentState; }
		}

		IState[] IStateMachine.states {
			get { return states; }
		}

		int IStateMachine.stateCount {
			get { return stateCount; }
		}

		void IStateMachine.SetOwner(object owner) {
			SetOwner((TOwner)owner);
		}

		void IStateMachine.PushState(IState state) {
			PushState((TState)state);
		}

		IState IStateMachine.PopState() {
			return PopState();
		}

		void IStateMachine.SetState(IState state) {
			SetState((TState)state);
		}

		void IStateMachine.Flush() {
			Flush();
		}

		void IStateMachine.Update() {
			Update();
		}

		void IStateMachine.FixedUpdate() {
			FixedUpdate();
		}

		void IStateMachine.LateUpdate() {
			LateUpdate();
		}
	}


	[Serializable]
	public abstract class State<TOwner> : IState {
		public virtual void DoEnter(TOwner owner) { }
		public virtual void DoExit(TOwner owner) { }

		public virtual void DoUpdate(TOwner owner) { }
		public virtual void DoFixedUpdate(TOwner owner) { }
		public virtual void DoLateUpdate(TOwner owner) { }


		void IState.DoEnter(object owner) { }
		void IState.DoExit(object owner) { }
	}


	public interface IStateMachine {
		event Action OnPushState;
		event Action OnPopState;

		object owner { get; set; }
		IState currentState { get; }
		IState[] states { get; }
		int stateCount { get; }

		void SetOwner(object owner);
		void PushState(IState state);
		IState PopState();
		void SetState(IState state);
		void Flush();

		void Update();
		void FixedUpdate();
		void LateUpdate();
	}


	public interface IState {
		void DoEnter(object owner);
		void DoExit(object owner);
	}
}