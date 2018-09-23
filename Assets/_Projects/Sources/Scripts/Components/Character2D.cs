using UnityEngine;


public class Character2D : MonoBehaviour {
    public bool                                     IsGrounded { get { return isGrounded; } }
    public bool                                     IsChangingDirection {
        get {
            return (thisRigidbody2D.velocity.x > 0f && IsFacing(ColliderDetector2D.Direction.Left)) ||
                   (thisRigidbody2D.velocity.x < 0f && IsFacing(ColliderDetector2D.Direction.Right));
        }
    }
    public ColliderDetector2D                       ColliderDetector { get { return thisColliderDetector; } }
    public Vector2                                  FaceAxis { get { return faceAxis; } }
    public Vector2                                  MaxVelocity { get { return maxVelocity; } }
    public Vector2                                  Velocity { get { return thisRigidbody2D.velocity; } }

    [SerializeField] private Vector2                maxVelocity = new Vector2(5f, 8f);
    [SerializeField] private bool                   isUpdateFaceAxisOnlyOnGround = true;

    [Header("Debug")]
    [SerializeField] private Vector2                moveDirection;
    [SerializeField] private Vector2                faceAxis;
    [SerializeField] private bool                   isGrounded;

    [Header("References")]
    [SerializeField] private Rigidbody2D            thisRigidbody2D;
    [SerializeField] private ColliderDetector2D     thisColliderDetector;
   


    public bool IsFacing(ColliderDetector2D.Direction direction) {
        switch(direction) {
            case ColliderDetector2D.Direction.Up: return faceAxis.y > 0f;
            case ColliderDetector2D.Direction.Down: return faceAxis.y < 0f;
            case ColliderDetector2D.Direction.Left: return faceAxis.x < 0f;
            case ColliderDetector2D.Direction.Right: return faceAxis.x > 0f;
            default: return true;
        }
    }

    public bool IsMoving(ColliderDetector2D.Direction direction) {
        switch(direction) {
            case ColliderDetector2D.Direction.Up: return thisRigidbody2D.velocity.y > 0f;
            case ColliderDetector2D.Direction.Down: return thisRigidbody2D.velocity.y < 0f;
            case ColliderDetector2D.Direction.Left: return thisRigidbody2D.velocity.x < 0f;
            case ColliderDetector2D.Direction.Right: return thisRigidbody2D.velocity.x > 0f;
            case ColliderDetector2D.Direction.Any: return thisRigidbody2D.velocity != Vector2.zero;
            default: return false;
        }
    }

    public void Init(Rigidbody2D rigidbody2D, ColliderDetector2D colliderDetector2D) {
        thisRigidbody2D = rigidbody2D;
        thisColliderDetector = colliderDetector2D;
    }

    public void Move(Vector2 direction) {
        if(thisColliderDetector.IsColliding(ColliderDetector2D.Direction.Up) && direction.y > 0) {
            direction.y = 0f;
        }
        if(thisColliderDetector.IsColliding(ColliderDetector2D.Direction.Down) && direction.y < 0) {
            direction.y = 0f;
        }
        if(thisColliderDetector.IsColliding(ColliderDetector2D.Direction.Left) && direction.x < 0) {
            direction.x = 0f;
        }
        if(thisColliderDetector.IsColliding(ColliderDetector2D.Direction.Right) && direction.x > 0) {
            direction.x = 0f;
        }

        moveDirection += direction;
    }

    private void ClampVelocity() {
        if(Mathf.Abs(thisRigidbody2D.velocity.x) > maxVelocity.x) {
            thisRigidbody2D.velocity = new Vector2(maxVelocity.x * Mathf.Sign(thisRigidbody2D.velocity.x), thisRigidbody2D.velocity.y);
        }

        if(Mathf.Abs(thisRigidbody2D.velocity.y) > maxVelocity.y) {
            thisRigidbody2D.velocity = new Vector2(thisRigidbody2D.velocity.x, maxVelocity.y * Mathf.Sign(thisRigidbody2D.velocity.y));
        }
    }

    private void Update() {
        // Bugs: thisRigidbody.velocity.y is having some funny values for some reason
        //       when walking or sprinting. Investigate this when you have some time
        //       For now we HOTFIX it by doing a "||" operator instead of an "&&" 
        //       operator
        // FIXED: It was not the code, but the composite collider issues, it seems to be 
        //        a Unity bug where the vertex snapping leaves very small gaps that could
        //        screw aroudn with collisions
        isGrounded = thisColliderDetector.IsColliding(ColliderDetector2D.Direction.Down) && thisRigidbody2D.velocity.y == 0f;
    }

    private void FixedUpdate() {
        if(moveDirection != Vector2.zero) {
            if(isUpdateFaceAxisOnlyOnGround && isGrounded) {
                faceAxis.x = moveDirection.x < 0f ? -1f : moveDirection.x > 0f ? 1f : faceAxis.x;
                faceAxis.y = moveDirection.y < 0f ? -1f : moveDirection.y > 0f ? 1f : faceAxis.y;
            }

            thisRigidbody2D.AddForce(moveDirection, ForceMode2D.Force);
            moveDirection = Vector2.zero;
        }

        ClampVelocity();
    }
}
