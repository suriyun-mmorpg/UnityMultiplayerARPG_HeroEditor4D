using MultiplayerARPG.HeroEditor4D;

namespace MultiplayerARPG
{
    public partial class Item : IHeroEditorItem
    {
        [Category(11, "In-Scene Settings (HeroEditor4D)")]
        public HeroEditorSpriteData spriteData;
        public HeroEditorSpriteData SpriteData { get { return spriteData; } }
    }
}
