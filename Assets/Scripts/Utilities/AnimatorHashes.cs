using UnityEngine;

namespace Utilities
{
    public static class AnimatorHashes
    {
        public static readonly int IsRunning = Animator.StringToHash("isRunning");
        public static readonly int IsJumping = Animator.StringToHash("isJumping");
        public static readonly int IsFiring = Animator.StringToHash("isFiring");
        public static readonly int IsGrounded = Animator.StringToHash("isGrounded");

        public static readonly int Jump = Animator.StringToHash("Jump");
        public static readonly int Throw = Animator.StringToHash("Throw");
        public static readonly int Fall = Animator.StringToHash("Fall");

        public static readonly int Idle = Animator.StringToHash("Idle");
        public static readonly int BombBossIntro = Animator.StringToHash("BombBossIntro");
    }
}

