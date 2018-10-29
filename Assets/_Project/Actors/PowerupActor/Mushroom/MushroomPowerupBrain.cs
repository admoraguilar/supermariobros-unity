using UnityEngine;


[CreateAssetMenu(menuName = "Actors/Powerup/Brains/Mushroom")]
public class MushroomPowerupBrain : PowerupActor.PowerupBrain {
    public override void DoTriggerEnter2D(PowerupActor levelItemActor, Collider2D collision) {
        CharacterActor otherCharacterActor = collision.transform.root.GetComponent<CharacterActor>();
        if(otherCharacterActor) {
            // Empower buffables upon contact
            if(levelItemActor.IsBrainBuffable(otherCharacterActor.brain)) {
                if(otherCharacterActor.formStateMachine.currentState == otherCharacterActor.smallFormState) {
                    otherCharacterActor.SetForm(otherCharacterActor.bigFormState);
                    Destroy(levelItemActor.gameObject);
                }
            }
        }
    }
}
