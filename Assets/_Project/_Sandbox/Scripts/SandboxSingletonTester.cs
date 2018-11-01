using UnityEngine;
using WishfulDroplet;


public class SandboxSingletonTester : MonoBehaviour {
	private SandboxSingletonTester thisSandboxSingletonTester;


	public void Log() {
		Debug.Log(thisSandboxSingletonTester.name);
	}

	private void Awake() {
		thisSandboxSingletonTester = Singleton.Get<SandboxSingletonTester>();
		thisSandboxSingletonTester.Log();
	}
}
