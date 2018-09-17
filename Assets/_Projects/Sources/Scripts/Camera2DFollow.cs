using UnityEngine;


public class Camera2DFollow : MonoBehaviour {
    [SerializeField] private Transform target;
    [SerializeField] private Vector2 minFollow, maxFollow;
    [SerializeField] private Vector2 minViewportBounds, maxViewportBounds;
    [SerializeField] private bool isFreezeXPos, isFreezeYPos, isFreezeZPos = false;

    private bool isFollowing;
    private Vector3 orgCameraPos;

    private Transform thisTransform;
    private Camera thisCamera;


    public void SetTarget(Transform target) {
        this.target = target;
    }

    private bool IsTransformInViewportBounds(Transform transform) {
        Vector3 transformViewportPos = thisCamera.WorldToViewportPoint(transform.position);
        return (transformViewportPos.x < minViewportBounds.x || transformViewportPos.x > maxViewportBounds.x || 
                transformViewportPos.y < minViewportBounds.y || transformViewportPos.y > maxViewportBounds.y);
    }

    private void Awake() {
        thisTransform = GetComponent<Transform>();
        thisCamera = GetComponent<Camera>();
    }

    private void Start() {
        orgCameraPos = thisTransform.position;
    }

    private void Update() {
        isFollowing = IsTransformInViewportBounds(target);
    }

    private void FixedUpdate() {
        if(isFollowing) {
            Vector3 move = target.position + (thisCamera.ViewportToWorldPoint(new Vector3(0.5f, 0f)) - thisCamera.ViewportToWorldPoint(maxViewportBounds));

            if(move.x < minFollow.x) move.x = minFollow.x;
            if(move.x > maxFollow.x) move.x = maxFollow.x;
            if(move.y < minFollow.y) move.y = minFollow.y;
            if(move.y > maxFollow.y) move.y = maxFollow.y;

            thisTransform.position = new Vector3(
                    isFreezeXPos ? orgCameraPos.x : move.x,
                    isFreezeYPos ? orgCameraPos.y : move.y,
                    isFreezeZPos ? orgCameraPos.z : move.z
                );
        }
    }
}
