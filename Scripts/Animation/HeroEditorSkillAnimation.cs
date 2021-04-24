namespace MultiplayerARPG.HeroEditor4D
{
    [System.Serializable]
    public struct HeroEditorSkillAnimation : ISkillAnims
    {
        public BaseSkill skill;
        public SkillActivateAnimationType activateAnimationType;
        [StringShowConditional(nameof(activateAnimationType), nameof(SkillActivateAnimationType.UseActivateAnimation))]
        public HeroEditorActionAnimation activateAnimation;
        public bool isTwoHanded;
        public BaseSkill Data { get { return skill; } }
    }
}
