using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SandboxMarioControllerTest : MonoBehaviour {
	public CharacterActor characterActor;
	public float horizontalInput = 1f;
	public bool isMoveHorizontal;
	public bool isSprint;
	public bool isJump;

	public bool doFlipInputUpdateState;
	

	private void Update() {
		if(doFlipInputUpdateState) {
			characterActor.brain.isUpdateInput = !characterActor.brain.isUpdateInput;
			doFlipInputUpdateState = false;
		}

		//characterActor.inputAxis = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
		//characterActor.isSprinting = Input.GetKey(KeyCode.LeftShift);
		//characterActor.isJumping = Input.GetKey(KeyCode.Space);

		characterActor.inputAxis = new Vector2(
			isMoveHorizontal ? horizontalInput : 0f,
			0f);
		characterActor.isSprinting = isSprint;
		characterActor.isJumping = isJump;
	}
}
