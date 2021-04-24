using UnityEngine;

namespace MultiplayerARPG.HeroEditor4D
{
    [System.Serializable]
    public struct HeroEditorAnimations
    {
        [Header("Attack")]
        public HeroEditorActionAnimation attackAnimation;

        [Header("Skill")]
        public HeroEditorActionAnimation skillActivateAnimation;

        [Header("Reload(Gun)")]
        public HeroEditorActionAnimation reloadAnimation;
    }
}
