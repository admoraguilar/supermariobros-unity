using UnityEngine;
using WishfulDroplet;
using WishfulDroplet.Extensions;


public class PowerUpController : MonoBehaviour {
    //[SerializeField] private Type type;

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
    //        switch(type) {
    //            case Type.Mushroom:
    //                if(mario.CurFormStateMachineData.Id == MarioController.Ids.FormId.Small) {
    //                    mario.SetForm(MarioController.Ids.FormId.Big);
    //                }
    //                break;
    //            case Type.Flower:
    //                if(mario.CurFormStateMachineData.Id == MarioController.Ids.FormId.Small) {
    //                    mario.SetForm(MarioController.Ids.FormId.Big);
    //                } else if(mario.CurFormStateMachineData.Id == MarioController.Ids.FormId.Big) {
    //                    mario.SetForm(MarioController.Ids.FormId.Power);
    //                }
    //                break;
    //        }
    //    }
    //}


    //public enum Type {
    //    Mushroom,
    //    Flower
    //}
}
