using UnityEngine;


[RequireComponent(typeof(Camera))]
public class Camera2DController : MonoBehaviour {
    [SerializeField] private Transform      target;
    [SerializeField] private Vector2        minFollow = new Vector2(0f, 0f);
    [SerializeField] private Vector2        maxFollow = new Vector2(10f, 0f);
    [SerializeField] private Vector2        minViewportBounds = new Vector2(0.1f, 0.1f);
    [SerializeField] private Vector2        maxViewportBounds = new Vector2(0.1f, 0.1f);
    [SerializeField] private bool           isFreezeXPos = false;
    [SerializeField] private bool           isFreezeYPos = false;
    [SerializeField] private bool           isFreezeZPos = true;

    private Vector3                         orgCameraPos;
    private bool                            isFollowing;
    

    [Header("References")]
    [SerializeField] private Transform      thisTransform;
    [SerializeField] private Camera         thisCamera;


    public void SetTarget(Transform target) {
        this.target = target;
    }

    private Vector3 GetViewportBoundsContact(Transform transform) {
        Vector3 transformViewportPos = thisCamera.WorldToViewportPoint(transform.position);

        if(transformViewportPos.x < minViewportBounds.x ||
           transformViewportPos.x > 1f - maxViewportBounds.x ||
           transformViewportPos.y < minViewportBounds.y ||
           transformViewportPos.y > 1f - maxViewportBounds.y) {

            transformViewportPos.x = Mathf.Clamp(transformViewportPos.x, minViewportBounds.x, 1f - maxViewportBounds.x);
            transformViewportPos.y = Mathf.Clamp(transformViewportPos.y, minViewportBounds.y, 1f - maxViewportBounds.y);

            return transformViewportPos;
        } else {
            return Vector3.zero;
        }
    }

    private void Start() {
        orgCameraPos = thisTransform.position;
    }

    private void Update() {
        isFollowing = GetViewportBoundsContact(target) != Vector3.zero;
    }

    private void FixedUpdate() {
        if(isFollowing) {
            Vector3 move = target.position + (thisCamera.ViewportToWorldPoint(new Vector3(0.5f, 0f)) - thisCamera.ViewportToWorldPoint(GetViewportBoundsContact(target)));

            move.x = Mathf.Clamp(move.x, minFollow.x, maxFollow.x);
            move.y = Mathf.Clamp(move.y, minFollow.y, maxFollow.y);

            thisTransform.position = new Vector3(
                    isFreezeXPos ? orgCameraPos.x : move.x,
                    isFreezeYPos ? orgCameraPos.y : move.y,
                    isFreezeZPos ? orgCameraPos.z : move.z
                );
        }
    }

    private void OnDrawGizmos() {
        // Draw follow bounds
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube((maxFollow - minFollow) * .5f, maxFollow);

        // Draw viewport bounds (Viewport's (0, 0) is at lower-left corner of the camera bounds)
        Gizmos.color = Color.yellow;

        // Up
        Gizmos.DrawLine(ModifyPoint(thisCamera.ViewportToWorldPoint(new Vector3(0f, 1f - maxViewportBounds.y, thisTransform.position.z))),
                        ModifyPoint(thisCamera.ViewportToWorldPoint(new Vector3(1f, 1f - maxViewportBounds.y, thisTransform.position.z))));
        // Down
        Gizmos.DrawLine(ModifyPoint(thisCamera.ViewportToWorldPoint(new Vector3(0f, minViewportBounds.y, thisTransform.position.z))),
                        ModifyPoint(thisCamera.ViewportToWorldPoint(new Vector3(1f, minViewportBounds.y, thisTransform.position.z))));

        // Left
        Gizmos.DrawLine(ModifyPoint(thisCamera.ViewportToWorldPoint(new Vector3(minViewportBounds.x, 0f, thisTransform.position.z))),
                        ModifyPoint(thisCamera.ViewportToWorldPoint(new Vector3(minViewportBounds.x, 1f, thisTransform.position.z))));
        // Right
        Gizmos.DrawLine(ModifyPoint(thisCamera.ViewportToWorldPoint(new Vector3(1f - maxViewportBounds.x, 0f, thisTransform.position.z))),
                        ModifyPoint(thisCamera.ViewportToWorldPoint(new Vector3(1f - maxViewportBounds.x, 1f, thisTransform.position.z))));
    }

    private Vector3 ModifyPoint(Vector3 point) {
        point.z = 0f;
        return point;
    }

    private void Reset() {
        thisTransform = GetComponent<Transform>();

        /*
         * Setup Camera
         */
        thisCamera = this.AddOrGetComponent<Camera>();
        thisCamera.orthographic = true;
        thisCamera.orthographicSize = 5;
    }
}
