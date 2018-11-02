using UnityEngine;
using WishfulDroplet;


[CreateAssetMenu(menuName = "Actors/CharacterActor/Brains/Goomba")]
public class GoombaCharacterBrain : CharacterActor.CharacterBrain {
	public override bool DoInteract(CharacterActor characterActor, GameObject interactor) {
		var actor = interactor.GetComponent<MonoActor>();
		Debug.Log(actor.brain.name);

		CharacterActor otherCharacterActor = interactor.GetComponent<CharacterActor>();
		if(otherCharacterActor) {
			Collider2D collision = otherCharacterActor.thisInteractionCollider2D;

			// Demote enemies to a smaller state or kill them if they are
			// below or the same y distance as Goomba
			if(characterActor.IsBrainEnemy(otherCharacterActor.brain)) {
				if(Utilities.CheckOtherColliderDirection2D(Direction.Down, characterActor.thisInteractionCollider2D, collision)) {
					if(otherCharacterActor.formStateMachine.currentState != otherCharacterActor.smallFormState) {
						otherCharacterActor.SetForm(otherCharacterActor.smallFormState);
						Debug.Log("small");
					} else {
						otherCharacterActor.statusStateMachine.SetState(otherCharacterActor.deadStatusState);
						Debug.Log("dead");
					}
				}
			}
		}

		return false;
	}

	public override void DoTriggerEnter2D(CharacterActor characterActor, Collider2D collision) {
		//Debug.Log(string.Format("{0} entered {1}'s trigger.", collision.name, characterActor.name));

		//CharacterActor otherCharacterActor = collision.transform.root.GetComponent<CharacterActor>();
		//if(otherCharacterActor) {
		//	// Demote enemies to a smaller state or kill them if they are
		//	// below or the same y distance as Goomba
		//	if(characterActor.IsBrainEnemy(otherCharacterActor.brain)) {
		//		if(Utilities.CheckOtherColliderDirection2D(Direction.Down, characterActor.thisInteractionCollider2D, collision)) {
		//			if(otherCharacterActor.formStateMachine.currentState != otherCharacterActor.smallFormState) {
		//				otherCharacterActor.SetForm(otherCharacterActor.smallFormState);
		//				Debug.Log("small");
		//			} else {
		//				otherCharacterActor.statusStateMachine.SetState(otherCharacterActor.deadStatusState);
		//				Debug.Log("dead");
		//			}
		//		}
		//	}
		//}

		Interactable interactable = collision.transform.root.GetComponent<Interactable>();
		if(interactable) {
			interactable.Interact(characterActor.gameObject);
		}
	}

    public override void DoDrawGizmos(CharacterActor characterActor) {
        // Small duck collider
        Gizmos.color = characterActor.smallFormState.duckBoxColliderInfo.gizmoColor;
        Gizmos.DrawWireCube(new Vector2(characterActor.thisTransform.position.x, characterActor.thisTransform.position.y) +
                                        characterActor.smallFormState.duckBoxColliderInfo.offset,
                            characterActor.smallFormState.duckBoxColliderInfo.size);

        // Small transition collider 
        Gizmos.color = characterActor.smallFormState.transitionBoxColliderInfo.gizmoColor;
        Gizmos.DrawWireCube(new Vector2(characterActor.thisTransform.position.x, characterActor.thisTransform.position.y) +
                                        characterActor.smallFormState.transitionBoxColliderInfo.offset,
                            characterActor.smallFormState.transitionBoxColliderInfo.size);

        // Big duck collider
        Gizmos.color = characterActor.bigFormState.duckBoxColliderInfo.gizmoColor;
        Gizmos.DrawWireCube(new Vector2(characterActor.thisTransform.position.x, characterActor.thisTransform.position.y) +
                                        characterActor.bigFormState.duckBoxColliderInfo.offset,
                            characterActor.bigFormState.duckBoxColliderInfo.size);

        // Big transition collider 
        Gizmos.color = characterActor.bigFormState.transitionBoxColliderInfo.gizmoColor;
        Gizmos.DrawWireCube(new Vector2(characterActor.thisTransform.position.x, characterActor.thisTransform.position.y) +
                                        characterActor.bigFormState.transitionBoxColliderInfo.offset,
                            characterActor.bigFormState.transitionBoxColliderInfo.size);

        // Power duck collider
        Gizmos.color = characterActor.powerFormState.duckBoxColliderInfo.gizmoColor;
        Gizmos.DrawWireCube(new Vector2(characterActor.thisTransform.position.x, characterActor.thisTransform.position.y) +
                                        characterActor.powerFormState.duckBoxColliderInfo.offset,
                            characterActor.powerFormState.duckBoxColliderInfo.size);

        // Power transition collider 
        Gizmos.color = characterActor.powerFormState.transitionBoxColliderInfo.gizmoColor;
        Gizmos.DrawWireCube(new Vector2(characterActor.thisTransform.position.x, characterActor.thisTransform.position.y) +
                                        characterActor.powerFormState.transitionBoxColliderInfo.offset,
                            characterActor.powerFormState.transitionBoxColliderInfo.size);
    }


    public override void DoReset(CharacterActor characterActor) {
        characterActor.smallFormState.transitionBoxColliderInfo = new CharacterActor.FormStates.BoxColliderInfo {
            offset = new Vector2(0f, 0.03f),
            size = new Vector2(0.8f, 0.75f),
            gizmoColor = Color.blue
        };

        characterActor.smallFormState.duckBoxColliderInfo = new CharacterActor.FormStates.BoxColliderInfo {
            offset = new Vector2(0f, 0.03f),
            size = new Vector2(0.8f, 0.75f),
            gizmoColor = Color.cyan
        };

        characterActor.bigFormState.transitionBoxColliderInfo = new CharacterActor.FormStates.BoxColliderInfo {
            offset = new Vector2(0f, 0.03f),
            size = new Vector2(0.8f, 0.75f),
            gizmoColor = Color.blue
        };

        characterActor.bigFormState.duckBoxColliderInfo = new CharacterActor.FormStates.BoxColliderInfo {
            offset = new Vector2(0f, 0.03f),
            size = new Vector2(0.8f, 0.75f),
            gizmoColor = Color.cyan
        };

        characterActor.powerFormState.transitionBoxColliderInfo = new CharacterActor.FormStates.BoxColliderInfo {
            offset = new Vector2(0f, 0.03f),
            size = new Vector2(0.8f, 0.75f),
            gizmoColor = Color.blue
        };

        characterActor.powerFormState.duckBoxColliderInfo = new CharacterActor.FormStates.BoxColliderInfo {
            offset = new Vector2(0f, 0.03f),
            size = new Vector2(0.8f, 0.75f),
            gizmoColor = Color.cyan
        };
    }
}
