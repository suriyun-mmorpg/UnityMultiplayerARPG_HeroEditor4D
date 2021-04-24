using UnityEngine;

namespace MultiplayerARPG.HeroEditor4D
{
    [System.Serializable]
    public struct HeroEditorAnimations
    {
        [Header("Movements while standing")]
        public AnimationClip idleClip;

        [Header("Movements while standing (move)")]
        public AnimationClip moveClip;

        [Header("Movements while standing (sprint)")]
        public AnimationClip sprintClip;

        [Header("Dead")]
        public AnimationClip deadClip;

        [Header("Attack")]
        public ActionAnimation attackAnimation;

        [Header("Skill")]
        public AnimationClip skillCastClip;
        public ActionAnimation skillActivateAnimation;

        [Header("Reload(Gun)")]
        public ActionAnimation reloadAnimation;
    }
}
