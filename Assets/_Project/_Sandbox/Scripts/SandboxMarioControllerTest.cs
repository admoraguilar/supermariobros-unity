using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SandboxMarioControllerTest : MonoBehaviour {
	public CharacterActor characterActor;


	private void Update()
	{
		characterActor.inputAxis = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
		characterActor.isSprinting = Input.GetKey(KeyCode.LeftShift);
		characterActor.isJumping = Input.GetKey(KeyCode.Space);
	}
}
