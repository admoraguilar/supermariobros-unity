using UnityEngine;


public class Camera2DFollow : MonoBehaviour {
    [SerializeField] private Transform target;
    [SerializeField] private Vector2 minFollow, maxFollow;
    [SerializeField] private Vector2 minViewportBounds, maxViewportBounds;
    [SerializeField] private float followSpeed = 10f;
    [SerializeField] private float distanceToStopFollowing = 0.05f;
    [SerializeField] private float lerpOffset = .1f;
    [SerializeField] private bool isFreezeXPos, isFreezeYPos, isFreezeZPos = false;
    

    private bool isFollowing;
    private float curLerpTime = 0f;
    private float lerpTime = 1f;
    private float lerpPercentage = 0f;
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
        if(IsTransformInViewportBounds(target)) {
            isFollowing = true;
        }
    }

    private void FixedUpdate() {
        //Vector3 move = Vector3.zero;
        //if(followSpeed > 0) {
        //    move = Vector3.Lerp(thisTransform.position, target.position, lerpOffset * followSpeed * Time.fixedDeltaTime);
        //}


        //if(isFollowing) {
        //    if(move.x < minFollow.x) move.x = minFollow.x; 
        //    if(move.x > maxFollow.x) move.x = maxFollow.x;
        //    if(move.y < minFollow.y) move.y = minFollow.y;
        //    if(move.y > maxFollow.y) move.y = maxFollow.y;

        //    thisTransform.position = new Vector3(
        //            isFreezeXPos ? orgCameraPos.x : move.x,
        //            isFreezeYPos ? orgCameraPos.y : move.y,
        //            isFreezeZPos ? orgCameraPos.z : move.z
        //        );
        //}

        //if(Vector2.Distance(thisTransform.position, move) <= distanceToStopFollowing) {
        //    isFollowing = false;
        //}

        //Vector3 move = Vector3.zero;
        //float lerpPercentage = 0f;

        //if(followSpeed < 0) {
        //    lerpPercentage = curLerpTime / lerpTime;
        //} else {
        //    lerpPercentage = curLerpTime
        //}

        //move = Vector3.Lerp(thisTransform.position, target.position, lerpPercentage * lerpOffset);

        if(isFollowing) {
            Vector3 move = Vector3.zero;

            // Reset lerping if player moves
            curLerpTime = 0f;
            lerpPercentage = 0f;

            // Calculate lerp
            if(followSpeed < 0) {
                curLerpTime = 1f;
            } else {
                curLerpTime += followSpeed * Time.fixedDeltaTime;
                if(curLerpTime >= lerpTime) {
                    curLerpTime = lerpTime;
                }
            }

            lerpPercentage = curLerpTime / lerpTime;
            move = Vector3.Lerp(thisTransform.position, target.position, lerpPercentage * lerpOffset);

            if(move.x < minFollow.x) move.x = minFollow.x;
            if(move.x > maxFollow.x) move.x = maxFollow.x;
            if(move.y < minFollow.y) move.y = minFollow.y;
            if(move.y > maxFollow.y) move.y = maxFollow.y;

            thisTransform.position = new Vector3(
                    isFreezeXPos ? orgCameraPos.x : move.x,
                    isFreezeYPos ? orgCameraPos.y : move.y,
                    isFreezeZPos ? orgCameraPos.z : move.z
                );

            if(Vector2.Distance(thisTransform.position, move) <= distanceToStopFollowing) {
                isFollowing = false;
            }
        }
    }
}
