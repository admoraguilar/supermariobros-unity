using UnityEngine;


public class SandboxInteractionMover : MonoBehaviour {
	private void OnTriggerEnter2D(Collider2D collision) {
		Debug.Log(string.Format("{0} entered trigger {1}", collision.name, name));
	}
}
