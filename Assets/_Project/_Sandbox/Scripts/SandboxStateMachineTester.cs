using UnityEngine;
using WishfulDroplet;
using WishfulDroplet.Extensions;


public class SandboxStateMachineTester : MonoBehaviour {
	public One oneBaseState = new One();
	public Two twoBaseState = new Two();

	public StateMachineController<string> stateMachineController = new StateMachineController<string>();
	public StateMachine<SandboxStateMachineTester, BaseState> firstStateMachine = new StateMachine<SandboxStateMachineTester, BaseState>();
	public StateMachine<SandboxStateMachineTester, BaseState> secondStateMachine = new StateMachine<SandboxStateMachineTester, BaseState>();


	private void Start() {
		//Debug.Log("StateMachine test start");

		//stateMachine.SetOwner(this);

		//stateMachine.OnPushState += (state) => {
		//	Debug.Log("Pushed on state machine");
		//};

		//stateMachine.SetState(oneBaseState);

		//IStateMachine temp = stateMachine;


		//temp.SetState(twoBaseState);

		//Debug.Log("StateMachine test start");

		//// Specs
		//Debug.Log(string.Format("State Machine - State Count: {0}", stateMachine.stateCount));

		stateMachineController.AddStateMachine(this, "1", firstStateMachine, oneBaseState);
		stateMachineController.AddStateMachine(this, "2", secondStateMachine, twoBaseState);
	}

	private void Update() {
		if(Input.GetKeyDown(KeyCode.Alpha0)) {
			stateMachineController.RemoveStateMachine("1");
		}

		stateMachineController.Update();
	}


	public class BaseState : State<SandboxStateMachineTester> {

	}

	public class One : BaseState {
		public override void DoEnter(SandboxStateMachineTester owner) {
			Debug.Log("1");
		}

		public override void DoUpdate(SandboxStateMachineTester owner) {
			Debug.Log("Updating 1");
		}
	}

	public class Two : BaseState {
		public override void DoEnter(SandboxStateMachineTester owner) {
			Debug.Log("2");
		}

		public override void DoUpdate(SandboxStateMachineTester owner) {
			Debug.Log("Updating 2");
		}
	}
}
