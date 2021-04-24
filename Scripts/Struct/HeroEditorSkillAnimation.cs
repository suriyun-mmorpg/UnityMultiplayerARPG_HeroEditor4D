using UnityEngine;

namespace MultiplayerARPG.HeroEditor4D
{
    [System.Serializable]
    public struct HeroEditorSkillAnimation : ISkillAnims
    {
        public BaseSkill skill;
        public AnimationClip castClip;
        public SkillActivateAnimationType activateAnimationType;
        [StringShowConditional(nameof(activateAnimationType), nameof(SkillActivateAnimationType.UseActivateAnimation))]
        public ActionAnimation activateAnimation;
        public BaseSkill Data { get { return skill; } }
    }
}
