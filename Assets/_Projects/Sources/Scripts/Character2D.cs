using UnityEngine;
using System;


public class Character2D : MonoBehaviour {
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float airMoveSpeed = 1f;
    [SerializeField] private float jumpSpeed = 5f;
    [SerializeField] private float maxJumpHeight = 5f;
    [SerializeField] private Vector2 maxVelocity = new Vector2(4f, 14f);
    [SerializeField] private BoxCastInfo upBoxCast;
    [SerializeField] private BoxCastInfo downBoxCast;
    [SerializeField] private BoxCastInfo leftBoxCast;
    [SerializeField] private BoxCastInfo rightBoxCast;

    private Vector2 inputAxis;
    private Vector2 lastPositionBeforeJumping;
    private bool isGrounded;
    private bool isJumping;

    private SpriteRenderer thisSpriteRenderer;
    private Transform thisTransform;
    private Rigidbody2D thisRigidbody2D;


    public Collider2D IsColliding(Direction direction) {
        BoxCastInfo boxCastInfo = MakeBoxCast(thisSpriteRenderer, direction);
        RaycastHit2D hit = Physics2D.BoxCast(boxCastInfo.Center, boxCastInfo.Size, 0f, boxCastInfo.Direction, boxCastInfo.Distance);
        boxCastInfo.Collider = hit.collider;
        return boxCastInfo.Collider;
    }

    public void Move(Vector2 direction) {
        if(IsColliding(Direction.Up) && direction.y < 0) {
            direction.y = 0f;
        }
        if(IsColliding(Direction.Left) && direction.x < 0) {
            direction.x = 0f;
        }
        if(IsColliding(Direction.Right) && direction.x > 0) {
            direction.x = 0f;
        }

        thisRigidbody2D.AddForce(direction, ForceMode2D.Force);
    }

    private BoxCastInfo MakeBoxCast(SpriteRenderer spriteRenderer, Direction direction) {
        switch(direction) {
            case Direction.Up:
                upBoxCast.Center = new Vector3(spriteRenderer.bounds.center.x, spriteRenderer.bounds.max.y, 0f);
                upBoxCast.Size = new Vector3(spriteRenderer.bounds.size.x * upBoxCast.SizeMultiplier, spriteRenderer.bounds.size.y * .2f, 0f);
                upBoxCast.Direction = Vector2.up;
                return upBoxCast;
            case Direction.Down:
                downBoxCast.Center = new Vector3(spriteRenderer.bounds.center.x, spriteRenderer.bounds.min.y, 0f);
                downBoxCast.Size = new Vector3(spriteRenderer.bounds.size.x * downBoxCast.SizeMultiplier, spriteRenderer.bounds.size.y * .2f, 0f);
                downBoxCast.Direction = Vector2.down;
                return downBoxCast;
            case Direction.Left:
                leftBoxCast.Center = new Vector3(spriteRenderer.bounds.min.x, spriteRenderer.bounds.center.y, 0f);
                leftBoxCast.Size = new Vector3(spriteRenderer.bounds.size.x * .2f, spriteRenderer.bounds.size.y * leftBoxCast.SizeMultiplier, 0f);
                leftBoxCast.Direction = Vector2.left;
                return leftBoxCast;
            case Direction.Right:
                rightBoxCast.Center = new Vector3(spriteRenderer.bounds.max.x, spriteRenderer.bounds.center.y, 0f);
                rightBoxCast.Size = new Vector3(spriteRenderer.bounds.size.x * .2f, spriteRenderer.bounds.size.y * rightBoxCast.SizeMultiplier, 0f);
                rightBoxCast.Direction = Vector2.right;
                return rightBoxCast;
            default:
                return new BoxCastInfo();
        }
    }

    private void ClampVelocity() {
        if(Mathf.Abs(thisRigidbody2D.velocity.x) > maxVelocity.x)
            thisRigidbody2D.velocity = new Vector2(maxVelocity.x * Mathf.Sign(thisRigidbody2D.velocity.x), thisRigidbody2D.velocity.y);
        if(Mathf.Abs(thisRigidbody2D.velocity.y) > maxVelocity.y)
            thisRigidbody2D.velocity = new Vector2(thisRigidbody2D.velocity.x, maxVelocity.y * Mathf.Sign(thisRigidbody2D.velocity.y));
    }

    private void Awake() {
        thisSpriteRenderer = GetComponent<SpriteRenderer>();
        thisTransform = GetComponent<Transform>();
        thisRigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void Update() {
        inputAxis = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        isGrounded = IsColliding(Direction.Down);

        if(isGrounded) {
            if(Input.GetKeyDown(KeyCode.Space)) {
                lastPositionBeforeJumping = thisTransform.localPosition;
                isJumping = true;
            }
        }

        if(Mathf.Abs(thisTransform.localPosition.y - lastPositionBeforeJumping.y) > maxJumpHeight ||
           Input.GetKeyUp(KeyCode.Space) ||
           IsColliding(Direction.Up)) {
            isJumping = false;
        }

        ClampVelocity();
    }

    private void FixedUpdate() {
        if(inputAxis != Vector2.zero) {
            float speed = isGrounded ? moveSpeed : airMoveSpeed;
            Move(new Vector2(inputAxis.x, 0f) * speed * Time.fixedDeltaTime);
        }

        if(isJumping) {
            Move(new Vector2(0f, 1f) * jumpSpeed * Time.fixedDeltaTime);
        }
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        BoxCastInfo boxCastInfo = new BoxCastInfo();

        boxCastInfo = MakeBoxCast(sr, Direction.Up);
        Gizmos.DrawWireCube(boxCastInfo.Center, boxCastInfo.Size);

        boxCastInfo = MakeBoxCast(sr, Direction.Down);
        Gizmos.DrawWireCube(boxCastInfo.Center, boxCastInfo.Size);

        boxCastInfo = MakeBoxCast(sr, Direction.Left);
        Gizmos.DrawWireCube(boxCastInfo.Center, boxCastInfo.Size);

        boxCastInfo = MakeBoxCast(sr, Direction.Right);
        Gizmos.DrawWireCube(boxCastInfo.Center, boxCastInfo.Size);
    }


    [Serializable]
    public class BoxCastInfo {
        [HideInInspector] public Vector3 Center;
        [HideInInspector] public Vector3 Size;
        [HideInInspector] public Vector2 Direction;
        public Collider2D Collider;
        public float SizeMultiplier = 1f;
        public float Distance = 1f;
    }

    public enum Direction {
        Up,
        Down,
        Left,
        Right,
    }
}
