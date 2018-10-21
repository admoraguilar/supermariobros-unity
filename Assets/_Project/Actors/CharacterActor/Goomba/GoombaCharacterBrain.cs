using UnityEngine;


[CreateAssetMenu(menuName = "Actors/CharacterActor/Brains/Goomba")]
public class GoombaCharacterBrain : CharacterActor.CharacterBrain {
    public override void DoTriggerEnter2D(CharacterActor characterActor, Collider2D collision) {
        CharacterActor otherCharacterActor = collision.GetComponent<CharacterActor>();
        if(otherCharacterActor) {
            if(characterActor.IsThisCharactersEnemy(otherCharacterActor.brain)) {
                if(Mathf.Abs(characterActor.thisInteractionCollider2D.bounds.min.y - collision.bounds.min.y) < .01f ||
                   characterActor.thisInteractionCollider2D.bounds.min.y >= collision.bounds.min.y) {
                    if(otherCharacterActor.formStateMachine.currentState != otherCharacterActor.smallFormState) {
                        otherCharacterActor.SetForm(otherCharacterActor.smallFormState);
                    } else {
                        otherCharacterActor.statusStateMachine.SetState(otherCharacterActor.deadStatusState);
                    }
                }
            }
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
