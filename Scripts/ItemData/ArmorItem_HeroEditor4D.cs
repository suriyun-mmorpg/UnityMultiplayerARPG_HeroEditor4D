using MultiplayerARPG.HeroEditor4D;

namespace MultiplayerARPG
{
    public partial class ArmorItem : IHeroEditorItem
    {
        public HeroEditorSpriteData spriteData;
        public HeroEditorSpriteData SpriteData { get { return spriteData; } }
    }
}
