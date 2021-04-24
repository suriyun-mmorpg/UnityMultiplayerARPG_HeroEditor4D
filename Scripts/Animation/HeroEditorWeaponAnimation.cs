namespace MultiplayerARPG.HeroEditor4D
{
    [System.Serializable]
    public struct HeroEditorWeaponAnimation : IWeaponAnims
    {
        public WeaponType weaponType;
        public HeroEditorActionAnimation rightHandAttackAnimation;
        public HeroEditorActionAnimation leftHandAttackAnimation;
        public HeroEditorActionAnimation rightHandReloadAnimation;
        public HeroEditorActionAnimation leftHandReloadAnimation;
        public bool isTwoHanded;
        public int weaponTypeNum;
        public WeaponType Data { get { return weaponType; } }
    }
}
