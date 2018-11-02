using UnityEngine;


[CreateAssetMenu(menuName = "Actors/Powerup/Brains/Flower")]
public class FlowerPowerupBrain : PowerupActor.PowerupBrain {
	public override bool DoInteracted(PowerupActor powerupActor, GameObject interactor) {
		CharacterActor otherCharacterActor = interactor.transform.root.GetComponent<CharacterActor>();
		if(otherCharacterActor) {
			// Empower buffables upon contact
			if(powerupActor.IsBrainBuffable(otherCharacterActor.brain)) {
				if(otherCharacterActor.formStateMachine.currentState == otherCharacterActor.smallFormState) {
					otherCharacterActor.SetForm(otherCharacterActor.bigFormState);
					Destroy(powerupActor.gameObject);
					return true;
				} else if(otherCharacterActor.formStateMachine.currentState == otherCharacterActor.bigFormState) {
					otherCharacterActor.SetForm(otherCharacterActor.powerFormState);
					Destroy(powerupActor.gameObject);
					return true;
				}
			}
		}

		return false;
	}
}
