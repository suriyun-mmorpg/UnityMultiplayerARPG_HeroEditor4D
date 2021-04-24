using UnityEngine;

namespace MultiplayerARPG.HeroEditor4D
{
    [System.Serializable]
    public struct HeroEditorAnimations
    {
        [Header("Movements while standing")]
        public AnimationClip idleClip;
        [Tooltip("If this <= 0, it will not be used to calculates with animation speed multiplier")]
        public float idleAnimSpeedRate;

        [Header("Movements while standing (move)")]
        public AnimationClip moveClip;
        [Tooltip("If this <= 0, it will not be used to calculates with animation speed multiplier")]
        public float moveAnimSpeedRate;

        [Header("Movements while standing (sprint)")]
        public AnimationClip sprintClip;
        [Tooltip("If this <= 0, it will not be used to calculates with animation speed multiplier")]
        public float sprintAnimSpeedRate;

        [Header("Movements while standing (walk)")]
        public AnimationClip walkClip;
        [Tooltip("If this <= 0, it will not be used to calculates with animation speed multiplier")]
        public float walkAnimSpeedRate;

        [Header("Hurt")]
        public AnimationClip hurtClip;
        [Tooltip("If this <= 0, it will not be used to calculates with animation speed multiplier")]
        public float hurtAnimSpeedRate;

        [Header("Dead")]
        public AnimationClip deadClip;
        [Tooltip("If this <= 0, it will not be used to calculates with animation speed multiplier")]
        public float deadAnimSpeedRate;

        [Header("Pickup")]
        public AnimationClip pickupClip;
        [Tooltip("If this <= 0, it will not be used to calculates with animation speed multiplier")]
        public float pickupAnimSpeedRate;

        [Header("Attack")]
        public ActionAnimation attackAnimation;

        [Header("Skill")]
        public AnimationClip skillCastClip;
        public ActionAnimation skillActivateAnimation;

        [Header("Reload(Gun)")]
        public ActionAnimation reloadAnimation;
    }
}
