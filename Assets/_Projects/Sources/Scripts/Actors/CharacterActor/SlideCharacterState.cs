using UnityEngine;
using WishfulDroplet;
using WishfulDroplet.Extensions;


[CreateAssetMenu(menuName = "Actors/CharacterActor/States/Slide")]
public class SlideCharacterState : CharacterActor.CharacterState {
    public override void OnEnter(CharacterActor characterActor) {
        characterActor.thisAnimator.PlayNoRepeat("Slide");
    }
}
