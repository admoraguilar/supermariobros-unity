using UnityEngine;
using System;


public class StateMachineTest : MonoBehaviour {
    [SerializeField] private StateMachineTestStateMachine states = new StateMachineTestStateMachine();
    [SerializeField] private FirstState firstState = new FirstState();
    [SerializeField] private SecondState secondState = new SecondState();
    [SerializeField] private ThirdState thirdState = new ThirdState();

    private void Start() {
        states.Init(this);
    }

    private void Update() {
        if(Input.GetKeyDown(KeyCode.Alpha1)) {
            states.SetState(firstState);
        }

        if(Input.GetKeyDown(KeyCode.Alpha2)) {
            states.SetState(secondState);
        }

        if(Input.GetKeyDown(KeyCode.Alpha3)) {
            states.SetState(thirdState);
        }

        if(Input.GetKeyDown(KeyCode.Q)) {
            states.PushState(firstState);
        }

        if(Input.GetKeyDown(KeyCode.W)) {
            states.PushState(secondState);
        }

        if(Input.GetKeyDown(KeyCode.E)) {
            states.PopState();
        }

        states.Update();
    }


    [Serializable]
    public class StateMachineTestStateMachine : StateMachine<StateMachineTest> {

    }

    [Serializable]
    public class FirstState : State<StateMachineTest> {
        public string ExtraMessage;


        public override void OnEnter(StateMachineTest owner) {
            Debug.Log(string.Format("{0}: OnEnter | {1}", GetType().Name, ExtraMessage));
        }

        public override void OnUpdate(StateMachineTest owner) {
            //Debug.Log(string.Format("{0}: OnUpdate", GetType().Name));
        }

        public override void OnExit(StateMachineTest owner) {
            Debug.Log(string.Format("{0}: OnExit", GetType().Name));
        }
    }

    [Serializable]
    public class SecondState : State<StateMachineTest> {
        public override void OnEnter(StateMachineTest owner) {
            Debug.Log(string.Format("{0}: OnEnter", GetType().Name));
        }

        public override void OnUpdate(StateMachineTest owner) {
            //Debug.Log(string.Format("{0}: OnUpdate", GetType().Name));
        }

        public override void OnExit(StateMachineTest owner) {
            Debug.Log(string.Format("{0}: OnExit", GetType().Name));
        }
    }

    [Serializable]
    public class ThirdState : State<StateMachineTest> {
        public override void OnEnter(StateMachineTest owner) {
            Debug.Log(string.Format("{0}: OnEnter", GetType().Name));
        }

        public override void OnUpdate(StateMachineTest owner) {
            //Debug.Log(string.Format("{0}: OnUpdate", GetType().Name));
        }

        public override void OnExit(StateMachineTest owner) {
            Debug.Log(string.Format("{0}: OnExit", GetType().Name));
        }
    }
}
