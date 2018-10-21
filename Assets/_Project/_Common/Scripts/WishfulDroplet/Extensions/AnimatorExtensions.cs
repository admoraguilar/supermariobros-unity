using UnityEngine;


namespace WishfulDroplet {
    namespace Extensions {
        public static class AnimatorExtensions {
            public static void PlayNoRepeat(this Animator animator, string name, int layer = 0, float normalizedTime = 0f) {
                if(!animator.GetCurrentAnimatorStateInfo(layer).IsName(name)) {
                    animator.Play(name, layer, normalizedTime);
                }
            }
        }
    }
}

