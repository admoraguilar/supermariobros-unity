using UnityEngine;


[CreateAssetMenu(menuName = "Actors/Powerup/Brains/Flower")]
public class FlowerPowerupBrain : PowerupActor.PowerupBrain {
    public override void DoTriggerEnter2D(PowerupActor levelItemActor, Collider2D collision) {
        CharacterActor otherCharacterActor = collision.GetComponent<CharacterActor>();
        if(otherCharacterActor) {
            // Empower buffables upon contact
            if(levelItemActor.IsBrainBuffable(otherCharacterActor.brain)) {
                if(otherCharacterActor.formStateMachine.currentState == otherCharacterActor.smallFormState) {
                    otherCharacterActor.SetForm(otherCharacterActor.bigFormState);
                    Destroy(levelItemActor.gameObject);
                    return;
                } else if(otherCharacterActor.formStateMachine.currentState == otherCharacterActor.bigFormState) {
                    otherCharacterActor.SetForm(otherCharacterActor.powerFormState);
                    Destroy(levelItemActor.gameObject);
                    return;
                }
            }
        }
    }
}
