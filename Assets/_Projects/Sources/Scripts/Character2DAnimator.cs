using UnityEngine;


public class Character2DAnimator : MonoBehaviour {
    private Animator thisAnimator;
    private Character2D thisCharacter2D;


    private void Awake() {
        thisAnimator = GetComponent<Animator>();
        thisCharacter2D = GetComponent<Character2D>();
    }

    private void Update() {
        if(!thisCharacter2D.IsMoving(Character2D.Direction.Any) && thisCharacter2D.IsColliding(Character2D.Direction.Down)) {
            thisAnimator.Play("Idle");
        }

        if(thisCharacter2D.thisRigidbody2D.velocity.x != 0 &&
           thisCharacter2D.thisRigidbody2D.velocity.y == 0 &&
           thisCharacter2D.IsColliding(Character2D.Direction.Down)) {
            thisAnimator.Play("Walk");
            thisAnimator.SetFloat("walkSpeedMultiplier",
                2.5f * Mathf.Max(0.1f, Mathf.InverseLerp(0f, thisCharacter2D.maxVelocity.x, Mathf.Abs(thisCharacter2D.thisRigidbody2D.velocity.x))));
        }

        if(thisCharacter2D.thisRigidbody2D.velocity.y != 0 && 
           !thisCharacter2D.IsColliding(Character2D.Direction.Down)) {
            thisAnimator.Play("Jump");
        }
    }
}