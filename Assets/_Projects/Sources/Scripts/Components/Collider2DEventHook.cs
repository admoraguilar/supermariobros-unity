using UnityEngine;
using System;


public class Collider2DEventHook : MonoBehaviour {
    public event Action<Collision2D>    AOnCollisionEnter2D = delegate { };
    public event Action<Collision2D>    AOnCollisionStay2D = delegate { };
    public event Action<Collision2D>    AOnCollisionExit2D = delegate { };
    public event Action<Collider2D>     AOnTriggerEnter2D = delegate { };
    public event Action<Collider2D>     AOnTriggerStay2D = delegate { };
    public event Action<Collider2D>     AOnTriggerExit2D = delegate { };


    private void OnCollisionEnter2D(Collision2D collision) {
        AOnCollisionEnter2D(collision);
    }

    private void OnCollisionStay2D(Collision2D collision) {
        AOnCollisionStay2D(collision);
    }

    private void OnCollisionExit2D(Collision2D collision) {
        AOnCollisionExit2D(collision);
    }

    private void OnTriggerEnter2D(Collider2D collider) {
        AOnTriggerEnter2D(collider);
    }

    private void OnTriggerStay2D(Collider2D collider) {
        AOnTriggerStay2D(collider);
    }

    private void OnTriggerExit2D(Collider2D collider) {
        AOnTriggerExit2D(collider);
    }
}
