using UnityEngine;
using WishfulDroplet;
using WishfulDroplet.Extensions;


public class GoombaController : MonoBehaviour {
    //[Header("References")]
    //[SerializeField] private Transform thisTransform;
    //[SerializeField] private BoxCollider2D thisSolidCollider2D;
    //[SerializeField] private BoxCollider2D thisInteractCollider2D;


    //private void Reset() {
    //    thisTransform = GetComponent<Transform>();

    //    thisSolidCollider2D = GetComponent<BoxCollider2D>();

    //    thisInteractCollider2D = Utilities.CreateObject("Interactor", thisTransform).AddOrGetComponent<BoxCollider2D>();
    //    thisInteractCollider2D.isTrigger = true;
    //}

    //private void OnTriggerEnter2D(Collider2D collision) {
    //    MarioController mario = collision.GetComponent<MarioController>();
    //    if(mario) {
    //        if(Mathf.Abs(thisInteractCollider2D.bounds.min.y - collision.bounds.min.y) < .01f || 
    //           thisInteractCollider2D.bounds.min.y >= collision.bounds.min.y) {
    //            if(mario.CurFormStateMachineData.Id != MarioController.Ids.FormId.Small) {
    //                mario.SetForm(MarioController.Ids.FormId.Small);
    //            } else {
    //                mario.Dead(true);
    //            }
    //        }
    //    }
    //}
}
