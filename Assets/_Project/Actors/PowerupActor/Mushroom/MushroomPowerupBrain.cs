using UnityEngine;


[CreateAssetMenu(menuName = "Actors/Powerup/Brains/Mushroom")]
public class MushroomPowerupBrain : PowerupActor.PowerupBrain {
	public override void DoStart(PowerupActor actor) {
		actor.movementStateMachine.SetState(actor.moveMovementState);
	}

	public override bool DoInteracted(PowerupActor powerupActor, GameObject interactor) {
		CharacterActor otherCharacterActor = interactor.transform.root.GetComponent<CharacterActor>();
		if(otherCharacterActor) {
			// Empower buffables upon contact
			if(powerupActor.IsBrainBuffable(otherCharacterActor.brain)) {
				if(otherCharacterActor.formStateMachine.currentState == otherCharacterActor.smallFormState) {
					otherCharacterActor.SetForm(otherCharacterActor.bigFormState);
					Destroy(powerupActor.gameObject);
					return true;
				}
			}
		}

		return false;
	}
}
