using UnityEngine;
using System;


[RequireComponent(typeof(Rigidbody2D), typeof(BoxColliderDetector2D), typeof(RayColliderDetector2D))]
public class Character2D : MonoBehaviour {
    public BoxCollider2D GroundCollider { get { return groundCollider; } }
    public BoxCollider2D CharacterCollider { get { return characterCollider; } }
    public BoxColliderDetector2D BoxColliderDetector { get { return thisBoxColliderDetector; } }
    public RayColliderDetector2D RayColliderDetector { get { return thisRayColliderDetector; } }
    public bool IsGrounded { get { return isGrounded; } }
    public bool IsChangingDirection {
        get {
            return (thisRigidbody2D.velocity.x > 0f && IsFacing(Direction.Left)) ||
                   (thisRigidbody2D.velocity.x < 0f && IsFacing(Direction.Right));
        }
    }
    public Vector2 FaceAxis { get { return faceAxis; } }
    public Vector2 MaxVelocity { get { return maxVelocity; } }
    public Vector2 Velocity { get { return thisRigidbody2D.velocity; } }

    [SerializeField] private Vector2 maxVelocity = new Vector2(5f, 8f);
    [SerializeField] private BoxCollider2D groundCollider;
    [SerializeField] private BoxCollider2D characterCollider;

    [SerializeField] private Vector2 moveDirection;
    [SerializeField] private Vector2 faceAxis;
    [SerializeField] private bool isGrounded;

    private BoxColliderDetector2D thisBoxColliderDetector;
    private RayColliderDetector2D thisRayColliderDetector;
    private Rigidbody2D thisRigidbody2D;


    public bool IsFacing(Direction direction) {
        switch(direction) {
            case Direction.Up: return faceAxis.y > 0f;
            case Direction.Down: return faceAxis.y < 0f;
            case Direction.Left: return faceAxis.x < 0f;
            case Direction.Right: return faceAxis.x > 0f;
            default: return true;
        }
    }

    public bool IsMoving(Direction direction) {
        switch(direction) {
            case Direction.Up: return thisRigidbody2D.velocity.y > 0f;
            case Direction.Down: return thisRigidbody2D.velocity.y < 0f;
            case Direction.Left: return thisRigidbody2D.velocity.x < 0f;
            case Direction.Right: return thisRigidbody2D.velocity.x > 0f;
            case Direction.Any: return thisRigidbody2D.velocity != Vector2.zero;
            default: return false;
        }
    }

    public void Move(Vector2 direction) {
        if(thisBoxColliderDetector.IsColliding(Direction.Up) && direction.y > 0) {
            direction.y = 0f;
        }
        if(thisBoxColliderDetector.IsColliding(Direction.Down) && direction.y < 0) {
            direction.y = 0f;
        }
        if(thisBoxColliderDetector.IsColliding(Direction.Left) && direction.x < 0) {
            direction.x = 0f;
        }
        if(thisBoxColliderDetector.IsColliding(Direction.Right) && direction.x > 0) {
            direction.x = 0f;
        }

        moveDirection += direction;
    }

    private void ClampVelocity() {
        if(Mathf.Abs(thisRigidbody2D.velocity.x) > maxVelocity.x)
            thisRigidbody2D.velocity = new Vector2(maxVelocity.x * Mathf.Sign(thisRigidbody2D.velocity.x), thisRigidbody2D.velocity.y);
        if(Mathf.Abs(thisRigidbody2D.velocity.y) > maxVelocity.y)
            thisRigidbody2D.velocity = new Vector2(thisRigidbody2D.velocity.x, maxVelocity.y * Mathf.Sign(thisRigidbody2D.velocity.y));
    }

    private void Awake() {
        thisRigidbody2D = GetComponent<Rigidbody2D>();
        thisBoxColliderDetector = GetComponent<BoxColliderDetector2D>();
        thisRayColliderDetector = GetComponent<RayColliderDetector2D>();
    }

    private void Start() {
        thisBoxColliderDetector.SetReferenceCollider(characterCollider);
        thisRayColliderDetector.SetReferenceCollider(characterCollider);
    }

    private void Update() {
        // Bugs: thisRigidbody.velocity.y is having some funny values for some reason
        //       when walking or sprinting. Investigate this when you have some time
        //       For now we HOTFIX it by doing a "||" operator instead of an "&&" 
        //       operator
        // FIXED: It was not the code, but the composite collider issues, it seems to be 
        //        a Unity bug where the vertex snapping leaves very small gaps that could
        //        screw aroudn with collisions
        isGrounded = thisBoxColliderDetector.IsColliding(Direction.Down) && thisRigidbody2D.velocity.y == 0f;
    }

    private void FixedUpdate() {
        if(moveDirection != Vector2.zero) {
            faceAxis.x = moveDirection.x < 0f ? -1f : moveDirection.x > 0f ? 1f : faceAxis.x;
            faceAxis.y = moveDirection.y < 0f ? -1f : moveDirection.y > 0f ? 1f : faceAxis.y;

            thisRigidbody2D.AddForce(moveDirection, ForceMode2D.Force);
            moveDirection = Vector2.zero;
        }

        ClampVelocity();
    }

    private void OnValidate() {
        Transform tr = GetComponent<Transform>();
        BoxColliderDetector2D boxColliderDetector = GetComponent<BoxColliderDetector2D>();
        RayColliderDetector2D rayColliderDetector = GetComponent<RayColliderDetector2D>();

        if(groundCollider == null) {
            groundCollider = this.SpawnIfNoExistingChildren<BoxCollider2D>("GroundCollider", tr);
        }

        if(characterCollider == null) {
            characterCollider = this.SpawnIfNoExistingChildren<BoxCollider2D>("CharacterCollider", tr);
        }

        if(boxColliderDetector) {
            if(characterCollider) boxColliderDetector.SetReferenceCollider(characterCollider);
            else boxColliderDetector.SetReferenceCollider(GetComponent<Collider2D>());
        }

        if(rayColliderDetector) {
            if(characterCollider) rayColliderDetector.SetReferenceCollider(characterCollider);
            else rayColliderDetector.SetReferenceCollider(GetComponent<Collider2D>());
        }
    }
}
