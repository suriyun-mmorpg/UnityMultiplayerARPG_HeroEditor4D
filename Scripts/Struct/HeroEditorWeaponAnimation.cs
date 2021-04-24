namespace MultiplayerARPG.HeroEditor4D
{
    [System.Serializable]
    public struct HeroEditorWeaponAnimation : IWeaponAnims
    {
        public WeaponType weaponType;
        public ActionAnimation rightHandAttackAnimation;
        public ActionAnimation leftHandAttackAnimation;
        public ActionAnimation rightHandReloadAnimation;
        public ActionAnimation leftHandReloadAnimation;
        public WeaponType Data { get { return weaponType; } }
    }
}
